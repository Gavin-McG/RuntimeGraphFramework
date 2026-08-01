using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    internal class InputRuntimePort : RuntimePort
    {
        [SerializeReference] private ValueWrapper _value;

        public InputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node)
        {
            _value = default;
        }
        
        public InputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node, object value)
            : base(name, index, id, direction, node)
        {
            _value = ValueWrapper.CreateWrapper(value);
        }
        
        internal override bool TrySetValue<T>(T value)
        {
            _value = ValueWrapper.CreateWrapper(value);
            return false;
        }
        
        public override bool TryGetValue<T>(out T value)
        {
            return _value.TryGetValue(out value);
        }
        
        public override bool TrySetNodeOutput<T>(IQueryContext context, T value)
        {
            return false;
        }
        
        public override bool TryGetNodeInput<T>(IQueryContext context, out T value)
        {
            if (!IsConnected) return TryGetValue(out value);
            return FirstConnectedPort.TryGetNodeInput(context, out value);
        }
    }
}