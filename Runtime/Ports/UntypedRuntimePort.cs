using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public class UntypedRuntimePort : RuntimePort
    {
        [SerializeField] private List<RuntimePortReference> _connections = new();
        
        public UntypedRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}

        public override Type DataType => null;
        public override bool IsConnected => _connections.Count != 0;
        public override RuntimePort FirstConnectedPort => _connections.FirstOrDefault().GetPort();
        
        public void Connect(RuntimePortReference portReference)
        {
            _connections.Add(portReference);
        }
        
        public override bool TryGetValue<T>(IQueryContext context, out T value)
        {
            value = default;
            return false;
        }

        public override bool TrySetValue<T>(IQueryContext context, T value)
        {
            return false;
        }
    }
}