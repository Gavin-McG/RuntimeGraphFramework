using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    internal class OutputRuntimePort : RuntimePort
    {
        public OutputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
        
        internal override bool TrySetValue<T>(T value)
        {
            return false;
        }
        
        public override bool TryGetValue<T>(out T value)
        {
            value = default;
            return false;
        }
        
        public override bool TrySetNodeOutput<T>(IQueryContext context, T value)
        {
            context.SetPortOutput(ID, value);
            return true;
        }
        
        public override bool TryGetNodeInput<T>(IQueryContext context, out T value)
        {
            value = default;
            if (!context.TryGetPortOutput(ID, out object objValue))
            {
                if (!_node.TryUpdateNode(context)) return false;
                if (!context.TryGetPortOutput(ID, out objValue)) return false;
            }
            return PortTypeCastManager.TryCastValue(_node.Graph.GetType(), objValue, out value);
        }
    }
}