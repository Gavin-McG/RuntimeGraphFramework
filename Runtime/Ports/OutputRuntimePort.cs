using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    internal abstract class OutputRuntimePort : RuntimePort
    {
        protected OutputRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
    }
    
    [Serializable]
    internal class OutputRuntimePort<TOutput, TGraph> : OutputRuntimePort
    {
        private TOutput _value;
        
        public OutputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}

        public override Type DataType => typeof(TOutput);
        
        public override bool TryGetValue<T>(out T value)
        {
            return PortTypeCastManager.TryCastValue<TOutput, T, TGraph>(_value, out value);
        }
        
        public override bool TryGetNodeInput<T>(IQueryContext context, out T value)
        {
            if (!_node.TryUpdateNode(context))
            {
                value = default;
                return false;
            }
            
            return TryGetValue(out value);
        }

        internal override bool TrySetValue<T>(T value)
        {
            return false;
        }

        public override bool TrySetNodeOutput<T>(IQueryContext context, T value)
        {
            // Check if types can assign 
            if (typeof(TOutput).IsAssignableFrom(typeof(T)))
            {
                _value = (TOutput)(object)value;
                return true;
            }
            
            return false;
        }
    }
}