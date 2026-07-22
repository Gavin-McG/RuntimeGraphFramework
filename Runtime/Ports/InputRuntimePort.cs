using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class InputRuntimePort : RuntimePort
    {
        protected InputRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
    }
    
    [Serializable]
    public class InputRuntimePort<TInput, TGraph> : InputRuntimePort
    {
        [SerializeField] private List<RuntimePortReference> _connections = new();

        public InputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}

        public override Type DataType => typeof(TInput);
        public override bool IsConnected => _connections.Count > 0;
        public override IRuntimePort FirstConnectedPort => _connections.FirstOrDefault();
        
        public override void Connect(RuntimePortReference portReference)
        {
            _connections.Add(portReference);
        }
        
        public override bool TryGetValue<T>(IQueryContext context, out T value)
        {
            if (IsConnected) return FirstConnectedPort.TryGetValue(context, out value);
            
            value = default;
            return false;
        }

        public override bool TrySetValue<T>(IQueryContext context, T value)
        {
            return false;
        }
    }
}