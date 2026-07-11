using System;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public static class PortExtensions
    {
        public static InputPort CreateConstantInputPort<TGraph>(Hash128 portId, object value)
        {
            var portType = value.GetType();
            var inputPortType = typeof(InputPort<,>).MakeGenericType(portType, typeof(TGraph));
            var constructorArguments = new object[] { value, portId };
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }
        
        private static InputPort CreateVariableInputPort<TGraph>(Hash128 portId, IVariable variable)
        {
            var portType = variable.DataType;
            var inputPortType = typeof(InputPort<,>).MakeGenericType(portType, typeof(TGraph));
            var constructorArguments = new object[] { portId, variable.Name };
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }

        private static InputPort CreatePortReferenceInputPort<TGraph>(Hash128 portId, OutputPortReference portReference)
        {
            var portType = portReference.DataType;
            var inputPortType = typeof(InputPort<,>).MakeGenericType(portType, typeof(TGraph));
            var constructorArguments = new object[] { portId, portReference };
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }

        public static OutputPort CreateRuntimeOutputPort(this IPort port, RuntimeNode node)
        {
            if (port == null) return null;
            if (port.Direction == PortDirection.Input)
                throw new ArgumentException("Port must be an Output port");

            Type portType = port.DataType;

            if (portType == typeof(int)) return new OutputPort<int>(node);
            if (portType == typeof(float)) return new OutputPort<float>(node);
            if (portType == typeof(string)) return new OutputPort<string>(node);
            if (portType == typeof(bool)) return new OutputPort<bool>(node);
            if (portType == typeof(GameObject)) return new OutputPort<GameObject>(node);
            if (portType == typeof(Color)) return new OutputPort<Color>(node);
            throw new ArgumentException($"Unsupported OutputPort type '{port.DataType}'");
        }

        public static InputPort CreateRuntimeInputPort<TGraph>(this IPort port, DialogueImportContext context)
        {
            if (port == null) return null;
            if (port.Direction == PortDirection.Output)
                throw new ArgumentException("Port must be an Input port");
            
            // Use value assigned on input port
            if (!port.IsConnected)
            {
                port.TryGetValue(out object value);
                return CreateConstantInputPort<TGraph>(port.ID, value);
            }

            // Get the node connected to the port
            IPort connectedPort = port.FirstConnectedPort;
            INode node = connectedPort.GetNode();
            
            // Check for error Node
            if (node == null)
            {
                throw new ArgumentException("A missing node is connected in the Graph. Please resolve before saving");
            }
            
            // Constant
            if (node is IConstantNode constantNode)
            {
                constantNode.TryGetValue(out object value);
                return CreateConstantInputPort<TGraph>(port.ID, value);
            }

            // Variable
            if (node is IVariableNode variableNode)
            {
                IVariable variable = variableNode.Variable;

                if (variable.VariableKind == VariableKind.Local)
                {
                    if (context.validVariables.Contains(variable))
                    {
                        // Create Variable port if Variable is valid
                        return CreateVariableInputPort<TGraph>(port.ID, variable);
                    }
                    
                    // Convert Variable to constant if Variable isn't valid
                    variable.TryGetDefaultValue(out object defaultValue);
                    return CreateConstantInputPort<TGraph>(port.ID, defaultValue);
                }
                else if (variable.VariableKind == VariableKind.Input)
                {
                    // Get the Connected Port of the Subgraph
                    var currentSubgraph = context.currentSubgraph;
                    if (currentSubgraph == null) throw new Exception("Incorrectly attempting to Return from Subgraph");
                    var subgraphInputPort = currentSubgraph.GetInputPortOfVariable(variable);
                    
                    // Get the DataNode InputPort of the Connected Port
                    context.currentSubgraph = null;
                    var output = subgraphInputPort.CreateRuntimeInputPort<TGraph>(context);
                    context.currentSubgraph = currentSubgraph;
                    
                    return output;
                }
            }

            // Custom node
            if (node is IEditorNode<RuntimeNode>)
            {
                OutputPortReference portReference = connectedPort.GetRuntimeOutputPortReference(context);
                if (portReference != null) 
                    return CreatePortReferenceInputPort<TGraph>(port.ID, portReference);
                else
                    return CreateConstantInputPort<TGraph>(port.ID, string.Empty);
            }
            
            // Subgraph
            if (node is ISubgraphNode subgraphNode)
            {
                if (context.currentSubgraph != null) throw new Exception("Attempting to Enter Nested Subgraph");
                var outputVariable = subgraphNode.GetOutputVariableOfPort(connectedPort);
                var outputVariableNode = outputVariable.GetNodes().FirstOrDefault(node => node.IsConnected);
                var variablePort = outputVariableNode?.GetInputPort(0);
                
                // Get the Connected Control Node within the Subgraph
                context.currentSubgraph = subgraphNode;
                var output = variablePort.CreateRuntimeInputPort<TGraph>(context);
                context.currentSubgraph = null;
                
                return output;
            }

            throw new ArgumentException($"Could not resolve InputPort of port for node {node.GetType().Name}");
        }
        
        public static ControlNode GetConnectedControlNode(this IPort port, DialogueImportContext context)
        {
            if (port == null) return null;
            if (!port.IsConnected) return null;
            var connectedPort = port.FirstConnectedPort;
            INode connectedNode = connectedPort.GetNode();
            
            // Direct Connection
            if (connectedNode is IEditorNode<ControlNode> controlNode)
            {
                return controlNode.GetRuntimeNode(context);
            }
            
            // Connection into Subgraph
            if (connectedNode is ISubgraphNode subgraphNode)
            {
                // Get the Connected Port within the Subgraph
                if (context.currentSubgraph != null) throw new Exception("Attempting to Enter Nested Subgraph");
                var inputVariable = subgraphNode.GetInputVariableOfPort(connectedPort);
                var inputvariableNode = inputVariable.GetNodes().FirstOrDefault(node => node.IsConnected);
                var variablePort = inputvariableNode?.GetOutputPort(0);
                
                // Get the Connected Control Node within the Subgraph
                context.currentSubgraph = subgraphNode;
                var output = variablePort.GetConnectedControlNode(context);
                context.currentSubgraph = null;
                
                return output;
            }
            
            // Connection out of Subgraph
            if (connectedNode is IVariableNode outputVariableNode)
            {
                // Get the Connected Port out of the Subgraph
                var currentSubgraph = context.currentSubgraph;
                if (currentSubgraph == null) throw new Exception("Incorrectly attempting to Return from Subgraph");
                var outputVariable = outputVariableNode.Variable;
                var subgraphOutputPort = currentSubgraph.GetOutputPortOfVariable(outputVariable);
                
                // Get the Connected Control Node out of the Subgraph
                context.currentSubgraph = null;
                var output = subgraphOutputPort.GetConnectedControlNode(context);
                context.currentSubgraph = currentSubgraph;
                
                return output;
            }
            
            return null;
        }
        
        public static OutputPortReference GetRuntimeOutputPortReference(this IPort port, DialogueImportContext context)
        {
            if (port == null) return null;
            
            // Check that the port is of the correct type
            if (port.Direction != PortDirection.Output)
                throw new ArgumentException("GetRuntimeOutputPortReference: Port direction must be Output");
            
            // Get port reference from EditorNode
            var node = port.GetNode();
            if (node is IEditorNode<RuntimeNode> editorNode)
            {
                if (!editorNode.TryGetOutputPortIndex(port, out int portIndex)) return null;
                var runtimeNode = editorNode.GetRuntimeNode(context);
                return new OutputPortReference(runtimeNode, portIndex);
            }
            
            // Get port reference from EditorContextNode
            if (node is BlockNode blockNode && blockNode.ContextNode is IEditorNode<RuntimeNode> editorContextNode)
            {
                if (!editorContextNode.TryGetOutputPortIndex(port, out int portIndex)) return null;
                var runtimeNode = editorContextNode.GetRuntimeNode(context);
                return new OutputPortReference(runtimeNode, portIndex);
            }
            
            // Could not get port reference
            throw new ArgumentException("GetRuntimeOutputPortReference: Port must belong to an IEditorNode");
        }

        public static InputPortReference GetRuntimeInputPortReference(this IPort port, DialogueImportContext context)
        {
            if (port == null) return null;
            
            // Check that the port is of the correct type
            if (port.Direction != PortDirection.Input)
                throw new ArgumentException("GetRuntimeInputPortReference: Port direction must be Input");
            
            // Get port reference from EditorNode
            var node = port.GetNode();
            if (node is IEditorNode<RuntimeNode> editorNode)
            {
                if (!editorNode.TryGetInputPortIndex(port, out int portIndex)) return null;
                var runtimeNode = editorNode.GetRuntimeNode(context);
                return new InputPortReference(runtimeNode, portIndex);
            }
            
            // Get port reference from EditorContextNode
            if (node is BlockNode blockNode && blockNode.ContextNode is IEditorNode<RuntimeNode> editorContextNode)
            {
                if (!editorContextNode.TryGetInputPortIndex(port, out int portIndex)) return null;
                var runtimeNode = editorContextNode.GetRuntimeNode(context);
                return new InputPortReference(runtimeNode, portIndex);
            }
            
            // Could not get port reference
            throw new ArgumentException("GetRuntimeInputPortReference: Port must belong to an IEditorNode");
        }
    }
}