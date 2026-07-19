using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public static class PortExtensions
    {
        private static RuntimePortDirection GetRuntimeDirection(this IPort port) =>
            port.Direction switch
            {
                PortDirection.Input => RuntimePortDirection.Input,
                PortDirection.Output => RuntimePortDirection.Output,
                _ => RuntimePortDirection.Input
            };

        private static int GetIndex(this IPort port)
        {
            var node = port.GetNode();
            switch (port.Direction)
            {
                case PortDirection.Input:
                    return node.GetInputPorts().ToList().IndexOf(port);
                case PortDirection.Output:
                    return node.GetOutputPorts().ToList().IndexOf(port);
                default:
                    return -1;
            }
        }
        
        private static UntypedRuntimePort CreateUntypedPort(this IPort port, GraphImportContext context, RuntimeNode node)
        {
            var runtimePort = new UntypedRuntimePort(
                port.Name, 
                port.GetIndex(), 
                port.ID, 
                port.GetRuntimeDirection(), 
                node);
            
            return runtimePort;
        }

        private static ConstantRuntimePort CreateConstantPort(this IPort port, GraphImportContext context, RuntimeNode node, object value)
        {
            var portType = typeof(ConstantRuntimePort<,>).MakeGenericType(value.GetType(), context.GraphType);
            var constructorArguments = new object[]
            {
                port.Name, 
                port.GetIndex(), 
                port.ID, 
                RuntimePortDirection.Input, 
                node, 
                value
            };
            
            return Activator.CreateInstance(portType, constructorArguments) as ConstantRuntimePort;
        }

        private static VariableRuntimePort CreateVariablePort(this IPort port, GraphImportContext context, RuntimeNode node, IVariable variable)
        {
            var portType = typeof(VariableRuntimePort<,>).MakeGenericType(variable.GetType(), context.GraphType);
            var constructorArguments = new object[]
            {
                port.Name,
                port.GetIndex(),
                port.ID,
                RuntimePortDirection.Input,
                node,
                variable.Name,
            };
            
            return Activator.CreateInstance(portType, constructorArguments) as VariableRuntimePort;
        }

        private static InputRuntimePort CreateInputPort(this IPort port, GraphImportContext context, RuntimeNode node)
        {
            var portType = typeof(InputRuntimePort<,>).MakeGenericType(port.DataType, context.GraphType);
            var constructorArguments = new object[]
            {
                port.Name,
                port.GetIndex(),
                port.ID,
                RuntimePortDirection.Input,
                node
            };
            
            return Activator.CreateInstance(portType, constructorArguments) as InputRuntimePort;
        }

        private static OutputRuntimePort CreateOutputPort(this IPort port, GraphImportContext context, RuntimeNode node)
        {
            var portType = typeof(OutputRuntimePort<,>).MakeGenericType(port.DataType, context.GraphType);
            var constructorArguments = new object[]
            {
                port.Name,
                port.GetIndex(),
                port.ID,
                RuntimePortDirection.Output,
                node
            };
            
            return Activator.CreateInstance(portType, constructorArguments) as OutputRuntimePort;
        }
        
        public static RuntimePort CreateRuntimePort(this IPort port, GraphImportContext context, RuntimeNode node)
        {
            if (port == null) return null;

            // UnTyped Port
            if (port.DataType == typeof(Untyped))
            {
                return CreateUntypedPort(port, context, node);
            }

            // Typed Port
            switch (port.Direction)
            {
                // Input Ports
                case PortDirection.Input:
                {
                    // Unconnected Constant port
                    if (!port.IsConnected)
                    {
                        port.TryGetValue(out object value);
                        return CreateConstantPort(port, context, node, value);
                    }
                    
                    var connectedPort = port.FirstConnectedPort;
                    var connectedNode = connectedPort?.GetNode();
                    
                    // Connected Constant Port
                    if (connectedNode is IConstantNode constantNode)
                    {
                        constantNode.TryGetValue(out object value);
                        return CreateConstantPort(port, context, node, value);
                    }
                    
                    // Variable Port
                    if (connectedNode is IVariableNode variableNode)
                    {
                        var variable = variableNode.Variable;
                        return variable.VariableKind switch
                        {
                            VariableKind.Local => CreateVariablePort(port, context, node, variable),
                            VariableKind.Input => throw new NotImplementedException(), //TODO
                            _ => throw new ArgumentException("Input port should not be connected to Output Variable")
                        };
                    }
                    
                    // IEditorNode
                    if (connectedNode is IEditorNode<RuntimeNode> editorNode)
                    {
                        var inputPort = CreateInputPort(port, context, node);
                        
                        // Connect Port
                        var connectedRuntimeNode = editorNode.GetRuntimeNode(context);
                        var connectedRuntimePort = connectedRuntimeNode.GetOutputPort(connectedPort.GetIndex());
                        inputPort.Connect(connectedRuntimePort.GetPortReference());
                        
                        return inputPort;
                    }
                    
                    // Subgraph Port
                    if (connectedNode is ISubgraphNode subgraphNode)
                    {
                        //TODO
                    }
                    
                    throw new ArgumentException($"Could not Create Runtime Port for Port connected to {connectedNode?.GetType()}");
                }
                
                // Output Ports
                case PortDirection.Output:
                {
                    // Output DataPort
                    return CreateOutputPort(port, context, node);
                }
                
                default: throw new ArgumentException("Could not Create Runtime Port for Port of unknown Direction");
            }
        }

        public static RuntimePortReference GetRuntimePortReference(this IPort port, GraphImportContext context)
        {
            if (port == null) return default;
            
            // Get  Node
            var node = port.GetNode();
            switch (port.Direction)
            {
                // Input Port Reference
                case PortDirection.Input:
                {
                    // Port on IEditorNode
                    if (node is IEditorNode<RuntimeNode> editorNode)
                    {
                        var runtimeNode = editorNode.GetRuntimeNode(context);
                        var runtimePort = runtimeNode.GetInputPort(port.GetIndex());
                        return runtimePort.GetPortReference();
                    }
                    
                    return default;
                }
                    
                // Output Port Reference
                case PortDirection.Output:
                {
                    // Port on IEditorNode
                    if (node is IEditorNode<RuntimeNode> editorNode)
                    {
                        var runtimeNode = editorNode.GetRuntimeNode(context);
                        var runtimePort = runtimeNode.GetOutputPort(port.GetIndex());
                        return runtimePort.GetPortReference();
                    }
                    
                    return default;
                }
                
                default: return default;
            }
        }
        
        public static T GetConnectedRuntimeNode<T>(this IPort port, GraphImportContext context)
        where T : IRuntimeNode
        {
            if (port == null) return default;
            if (!port.IsConnected) return default;
            var connectedPort = port.FirstConnectedPort;
            var connectedNode = connectedPort.GetNode();
            
            // IEditorNode
            if (connectedNode is IEditorNode<T> controlNode)
            {
                return controlNode.GetRuntimeNode(context);
            }
            
            // ISubgraph
            if (connectedNode is ISubgraphNode subgraphNode)
            {
                // TODO
            }
            
            // Connection out of Subgraph
            if (connectedNode is IVariableNode variableNode)
            {
                var variable = variableNode.Variable;
                return variable.VariableKind switch
                {
                    VariableKind.Local => default,
                    VariableKind.Input => throw new NotImplementedException(), //TODO
                    VariableKind.Output => throw new NotImplementedException(), //TODO
                    _ => default
                };
            }
            
            return default;
        }
    }
}