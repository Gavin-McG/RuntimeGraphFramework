using System;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public static class PortExtensions
    {
        public static RuntimeNode GetRuntimeNode(this IPort port, GraphImportContext context)
        {
            var node = port.GetNode();
            if (node is not IEditorNode<RuntimeNode> editorNode) 
                throw new ArgumentException("Port must be on an EditorNode");
            return editorNode.GetRuntimeNode(context);
        } 
        
        public static InputPort CreateConstantInputPort(this IPort port, GraphImportContext context)
        {
            port.TryGetValue(out object value);
            var inputPortType = typeof(ConstantInputPort<,>).MakeGenericType(port.DataType, context.GraphType);
            var constructorArguments = new object[] { port.Name, port.ID, port.GetRuntimeNode(context), value };
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }
        
        public static InputPort CreateConstantInputPort(this IPort port, GraphImportContext context, object value)
        {
            var inputPortType = typeof(ConstantInputPort<,>).MakeGenericType(value.GetType(), context.GraphType);
            var constructorArguments = new object[] { port.Name, port.ID, port.GetRuntimeNode(context), value};
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }
        
        public static InputPort CreateConstantInputPort(this InputPort inputPort, GraphImportContext context, object value)
        {
            var inputPortType = typeof(ConstantInputPort<,>).MakeGenericType(value.GetType(), context.GraphType);
            var constructorArguments = new object[] { inputPort.Name, inputPort.ID, inputPort.Node, value };
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }
        
        private static InputPort CreateVariableInputPort(this IPort port, GraphImportContext context, IVariable variable)
        {
            var portType = variable.DataType;
            var inputPortType = typeof(VariableInputPort<,>).MakeGenericType(portType, context.GraphType);
            var constructorArguments = new object[] { port.Name, port.ID, port.GetRuntimeNode(context), variable.Name };
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }

        private static InputPort CreateConnectedInputPort(this IPort port, GraphImportContext context, OutputPortReference portReference)
        {
            var portType = portReference.DataType;
            var inputPortType = typeof(ConnectedInputPort<,>).MakeGenericType(portType, context.GraphType);
            var constructorArguments = new object[] { port.Name, port.ID, port.GetRuntimeNode(context), portReference };
            return Activator.CreateInstance(inputPortType, constructorArguments) as InputPort;
        }

        public static OutputPort CreateRuntimeOutputPort(this IPort port, RuntimeNode node)
        {
            if (port == null) return null;
            if (port.Direction == PortDirection.Input)
                throw new ArgumentException("Port must be an Output port");

            Type portType = port.DataType;
            var outputPortType = typeof(OutputPort<>).MakeGenericType(portType);
            var constructorArguments = new object[] { port.Name, port.ID, node };
            return Activator.CreateInstance(outputPortType, constructorArguments) as OutputPort;
        }

        public static InputPort CreateRuntimeInputPort(this IPort port, GraphImportContext context)
        {
            if (port == null) return null;
            if (port.Direction != PortDirection.Input) 
                throw new ArgumentException("Port must be an Input port");
            
            // Use value assigned on input port
            if (!port.IsConnected)
            {
                return port.CreateConstantInputPort(context);
            }

            // Get the node connected to the port
            IPort connectedPort = port.FirstConnectedPort;
            INode connectedNode = connectedPort.GetNode();
            
            // Check for error Node
            if (connectedNode == null)
            {
                throw new ArgumentException("A missing node is connected in the Graph. Please resolve before saving");
            }
            
            // Constant
            if (connectedNode is IConstantNode constantNode)
            {
                constantNode.TryGetValue(out object value);
                return port.CreateConstantInputPort(context, value);
            }

            // Variable
            if (connectedNode is IVariableNode variableNode)
            {
                IVariable variable = variableNode.Variable;

                if (variable.VariableKind == VariableKind.Local)
                {
                    if (context.validVariables.Contains(variable))
                    {
                        // Create Variable port if Variable is valid
                        return port.CreateVariableInputPort(context, variable);
                    }
                    
                    // Convert Variable to constant if Variable isn't valid
                    variable.TryGetDefaultValue(out object defaultValue);
                    return port.CreateConstantInputPort(context, defaultValue);
                }
                else if (variable.VariableKind == VariableKind.Input)
                {
                    // Get the Connected Port of the Subgraph
                    var currentSubgraph = context.currentSubgraph;
                    if (currentSubgraph == null) throw new Exception("Incorrectly attempting to Return from Subgraph");
                    var subgraphInputPort = currentSubgraph.GetInputPortOfVariable(variable);
                    
                    // Get the DataNode InputPort of the Connected Port
                    context.currentSubgraph = null;
                    var output = subgraphInputPort.CreateRuntimeInputPort(context);
                    context.currentSubgraph = currentSubgraph;
                    
                    return output;
                }
            }

            // Custom node
            if (connectedNode is IEditorNode<RuntimeNode>)
            {
                OutputPortReference portReference = connectedPort.GetOutputPortReference(context);
                if (portReference != null) 
                    return port.CreateConnectedInputPort(context, portReference);
                else
                    return port.CreateConstantInputPort(context, 0f);
            }
            
            // Subgraph
            if (connectedNode is ISubgraphNode subgraphNode)
            {
                if (context.currentSubgraph != null) throw new Exception("Attempting to Enter Nested Subgraph");
                var outputVariable = subgraphNode.GetOutputVariableOfPort(connectedPort);
                var outputVariableNode = outputVariable.GetNodes().FirstOrDefault(node => node.IsConnected);
                var variablePort = outputVariableNode?.GetInputPort(0);
                
                // Get the Connected Control Node within the Subgraph
                context.currentSubgraph = subgraphNode;
                var output = variablePort.CreateRuntimeInputPort(context);
                context.currentSubgraph = null;
                
                return output;
            }

            throw new ArgumentException($"Could not resolve InputPort of port for node {connectedNode.GetType().Name}");
        }
        
        public static T GetConnectedRuntimeNode<T>(this IPort port, GraphImportContext context)
        where T : RuntimeNode
        {
            if (port == null) return null;
            if (!port.IsConnected) return null;
            var connectedPort = port.FirstConnectedPort;
            INode connectedNode = connectedPort.GetNode();
            
            // Direct Connection
            if (connectedNode is IEditorNode<T> controlNode)
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
                var output = variablePort.GetConnectedRuntimeNode<T>(context);
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
                var output = subgraphOutputPort.GetConnectedRuntimeNode<T>(context);
                context.currentSubgraph = currentSubgraph;
                
                return output;
            }
            
            return null;
        }
        
        public static OutputPortReference GetOutputPortReference(this IPort port, GraphImportContext context)
        {
            if (port == null) return null;
            
            // Check that the port is of the correct type
            if (port.Direction != PortDirection.Output)
                throw new ArgumentException("GetOutputPortReference: Port direction must be Output");
            
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
            throw new ArgumentException("GetOutputPortReference: Port must belong to an IEditorNode");
        }

        public static InputPortReference GetInputPortReference(this IPort port, GraphImportContext context)
        {
            if (port == null) return null;
            
            // Check that the port is of the correct type
            if (port.Direction != PortDirection.Input)
                throw new ArgumentException("GetInputPortReference: Port direction must be Input");
            
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
            throw new ArgumentException("GetInputPortReference: Port must belong to an IEditorNode");
        }
    }
}