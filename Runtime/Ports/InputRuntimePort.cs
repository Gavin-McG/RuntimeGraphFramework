using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    internal abstract class InputRuntimePort : RuntimePort
    {
        protected InputRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
    }
    
    [Serializable]
    internal class InputRuntimePort<TInput, TGraph> : InputRuntimePort
    {
        [SerializeField] private TInput _value;

        public InputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node)
        {
            _value = default;
        }
        
        public InputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node, TInput value)
            : base(name, index, id, direction, node)
        {
            _value = value;
        }

        public override Type DataType => typeof(TInput);
        
        public override bool TryGetValue<T>(out T value)
        {
            // Check if types can assign 
            if (typeof(T).IsAssignableFrom(_value.GetType()))
            {
                value = (T)(object)_value;
                return true;
            }
    
            value = default;
            return false;
        }
        
        public override bool TryGetNodeInput<T>(IQueryContext context, out T value)
        {
            if (IsConnected && FirstConnectedPort.TryGetNodeInput(context, out value)) return true;
            return TryGetValue(out value);
        }
        
        internal override bool TrySetValue<T>(T value)
        {
            // Check if types can assign 
            if (typeof(TInput).IsAssignableFrom(value.GetType()))
            {
                _value = (TInput)(object)value;
                return true;
            }
            
            return false;
        }

        public override bool TrySetNodeOutput<T>(IQueryContext context, T value)
        {
            return false;
        }
    }
}