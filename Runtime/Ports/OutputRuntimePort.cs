using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class OutputRuntimePort : RuntimePort
    {
        protected OutputRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
        
        public abstract void Connect(RuntimePortReference portReference);
    }
    
    [Serializable]
    public class OutputRuntimePort<TOutput, TGraph> : OutputRuntimePort
    {
        [SerializeField] private List<RuntimePortReference> _connections = new();
        private TOutput savedValue;
        
        public OutputRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}

        public override Type DataType => typeof(TOutput);
        public override bool IsConnected => _connections.Count != 0;
        public override RuntimePort FirstConnectedPort => _connections.FirstOrDefault().GetPort();
        
        public override void Connect(RuntimePortReference portReference)
        {
            _connections.Add(portReference);
        }
        
        public override bool TryGetValue<T>(IQueryContext context, out T value)
        {
            _node.UpdateNode(context);
            return PortTypeCastManager.TryCastValue<TOutput, T, TGraph>(savedValue, out value);
        }

        public override bool TrySetValue<T>(IQueryContext context, T value)
        {
            return PortTypeCastManager.TryCastValue<T, TOutput, TGraph>(value, out savedValue);
        }
    }
}