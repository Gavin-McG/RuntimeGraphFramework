using System;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public static class PortExtensions
    {
        public static InputPort CreateConstantInputPort<TGraph>(Hash128 portId, Type type, object value)
        {
            if (type == typeof(int))
                return new InputPort<int, TGraph>(value is int i ? i : 0, portId);

            if (type == typeof(float))
                return new InputPort<float, TGraph>(value is float f ? f : 0, portId);

            if (type == typeof(string))
                return new InputPort<string, TGraph>(value as string, portId);

            if (type == typeof(bool))
                return new InputPort<bool, TGraph>(value is bool b && b, portId);
            
            if (type == typeof(GameObject))
                return new InputPort<GameObject, TGraph>(value is GameObject g ? g : null, portId);
            
            if (type == typeof(Color))
                return new InputPort<Color, TGraph>(value is Color c ? c : Color.white, portId);

            throw new NotSupportedException($"Unsupported InputPort type '{type}'");
        }
        
        private static InputPort CreateParameterInputPort<TGraph>(Hash128 portId, Type type, string parameterName)
        {
            if (type == typeof(int))
                return new InputPort<int, TGraph>(portId, parameterName);

            if (type == typeof(float))
                return new InputPort<float, TGraph>(portId, parameterName);

            if (type == typeof(string))
                return new InputPort<string, TGraph>(portId, parameterName);

            if (type == typeof(bool))
                return new InputPort<bool, TGraph>(portId, parameterName);
            
            if (type == typeof(GameObject))
                return new InputPort<GameObject, TGraph>(portId, parameterName);
            
            if (type == typeof(Color))
                return new InputPort<Color, TGraph>(portId, parameterName);

            throw new NotSupportedException($"Unsupported InputPort type '{type}'");
        }

        private static InputPort CreatePortReferenceInputPort<TGraph>(Hash128 portId, Type type, OutputPortReference portReference)
        {
            if (type == typeof(int))
                return new InputPort<int, TGraph>(portId, portReference);

            if (type == typeof(float))
                return new InputPort<float, TGraph>(portId, portReference);

            if (type == typeof(string))
                return new InputPort<string, TGraph>(portId, portReference);

            if (type == typeof(bool))
                return new InputPort<bool, TGraph>(portId, portReference);
            
            if (type == typeof(GameObject))
                return new InputPort<GameObject, TGraph>(portId, portReference);
            
            if (type == typeof(Color))
                return new InputPort<Color, TGraph>(portId, portReference);

            throw new NotSupportedException($"Unsupported InputPort type '{type}'");
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
                return CreateConstantInputPort<TGraph>(port.ID, value.GetType(), value);
            }

            // Get the node connected to the port
            IPort connectedPort = port.FirstConnectedPort;
            INode node = connectedPort.GetNode();
            
            // Constant
            if (node is IConstantNode constantNode)
            {
                constantNode.TryGetValue(out object value);
                return CreateConstantInputPort<TGraph>(port.ID, value.GetType(), value);
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
                        return CreateParameterInputPort<TGraph>(port.ID, variable.DataType, variable.Name);
                    }
                    
                    // Convert Variable to constant if Variable isn't valid
                    variable.TryGetDefaultValue(out object defaultValue);
                    return CreateConstantInputPort<TGraph>(port.ID, variable.DataType, defaultValue);
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
            if (node is IEditorNode<RuntimeNode> dataNode)
            {
                OutputPortReference portReference = dataNode.GetRuntimeOutputPortReference(context, connectedPort);
                return CreatePortReferenceInputPort<TGraph>(port.ID, portReference.DataType, portReference);
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

            throw new ArgumentException("Could not resolve InputPort of port");
        }
    }
}