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
        
        public static RuntimePort CreateRuntimePort(this IPort port, GraphImportContext context)
        {
            if (port == null) return null;

            // UnTyped Port
            var node = port.GetNode();
            var runtimeNode = node.GetRuntimeNode(context);
            if (port.DataType == typeof(Untyped))
            {
                return CreateUntypedPort(port, context, runtimeNode);
            }

            // Typed Port
            switch (port.Direction)
            {
                // Input Ports
                case PortDirection.Input:
                {
                    // Unconnected Constant port
                    if (port.IsConnected) return CreateInputPort(port, context, runtimeNode);
                    
                    port.TryGetValue(out object value);
                    return CreateConstantPort(port, context, runtimeNode, value);
                }
                
                // Output Ports
                case PortDirection.Output:
                {
                    // Output DataPort
                    return CreateOutputPort(port, context, runtimeNode);
                }
                
                default: throw new NotImplementedException();
            }
        }

        public static RuntimePortReference GetRuntimePortReference(this IPort port, GraphImportContext context)
        {
            if (port == null) return default;
            
            // Get  Node
            var node = port.GetNode();
            var runtimeNode = node.GetRuntimeNode(context);
            switch (port.Direction)
            {
                // Input Port Reference
                case PortDirection.Input:
                {
                    var runtimePort = runtimeNode.GetInputPort(port.GetIndex());
                    return runtimePort.GetPortReference();
                }
                    
                // Output Port Reference
                case PortDirection.Output:
                {
                    var runtimePort = runtimeNode.GetOutputPort(port.GetIndex());
                    return runtimePort.GetPortReference();
                }
                
                default: throw new NotImplementedException();
            }
        }
    }
}