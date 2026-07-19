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
        
        public abstract void Connect(RuntimePortReference portReference);
    }
    
    [Serializable]
    public class InputRuntimePort<TInput, TGraph> : InputRuntimePort
    {
        [SerializeField] private RuntimePortReference _connectedPort;
        
        public InputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}

        public override Type DataType => typeof(TInput);
        public override bool IsConnected => _connectedPort.IsValid;
        public override RuntimePort FirstConnectedPort => _connectedPort.GetPort();
        
        public override void Connect(RuntimePortReference portReference)
        {
            _connectedPort = portReference;
        }
        
        public override bool TryGetValue<T>(IQueryContext context, out T value)
        {
            if (IsConnected) return _connectedPort.TryGetValue(context, out value);
            
            value = default;
            return false;
        }

        public override bool TrySetValue<T>(IQueryContext context, T value)
        {
            return false;
        }
    }
}