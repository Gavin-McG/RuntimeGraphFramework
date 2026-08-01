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
            return new UntypedRuntimePort(
                port.Name, 
                port.GetIndex(), 
                port.ID, 
                port.GetRuntimeDirection(), 
                node);
        }
        
        private static InputRuntimePort CreateInputPort(this IPort port, GraphImportContext context, RuntimeNode node, object value)
        {
            return new InputRuntimePort(
                port.Name,
                port.GetIndex(),
                port.ID,
                RuntimePortDirection.Input,
                node,
                value);
        }

        private static OutputRuntimePort CreateOutputPort(this IPort port, GraphImportContext context, RuntimeNode node)
        {
            return new OutputRuntimePort(
                port.Name,
                port.GetIndex(),
                port.ID,
                RuntimePortDirection.Output,
                node
            );
        }
        
        internal static RuntimePort CreateRuntimePort(this IPort port, GraphImportContext context)
        {
            if (port == null) return null;

            // UnTyped Port
            var node = port.GetNode();
            var runtimeNode = node.AsEditorNode(context).RuntimeNode;
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
                    port.TryGetValue(out object value);
                    return CreateInputPort(port, context, runtimeNode, value);
                }
                
                // Output Ports
                case PortDirection.Output:
                {
                    // Output DataPort
                    return CreateOutputPort(port, context, runtimeNode);
                }
                
                default: throw new NotSupportedException("Unknown port Direction");
            }
        }

        /// <summary>
        /// Retrieves a Reference to the Runtime representation of a GTK Port
        /// </summary>
        public static RuntimePortReference GetRuntimePortReference(this IPort port, GraphImportContext context)
        {
            if (port == null) return default;
            
            // Get  Node
            var node = port.GetNode();
            var runtimeNode = node.AsEditorNode(context).RuntimeNode;
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
                
                default: throw new NotSupportedException("Unknown port Direction");
            }
        }
    }
}