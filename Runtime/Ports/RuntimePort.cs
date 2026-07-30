using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    internal abstract class RuntimePort : IRuntimePort
    {
        [SerializeField] private string _name;
        [SerializeField] private int _index;
        [SerializeField] private Hash128 _id;
        [SerializeField] private RuntimePortDirection _direction;
        [SerializeField] protected RuntimeNode _node;
        [SerializeField] private List<RuntimePortReference> _connections = new();
        
        public string Name => _name;
        public Hash128 ID => _id;
        public RuntimePortDirection Direction => _direction;

        public abstract Type DataType { get; }
        public bool IsConnected => _connections.Count > 0;
        public IRuntimePort FirstConnectedPort => _connections.ElementAtOrDefault(0);

        protected RuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
        {
            _name = name;
            _index = index;
            _id = id;
            _direction = direction;
            _node = node;
        }
        
        public IRuntimeNode GetNode() => _node;

        internal void AddConnection(IRuntimePort port)
        {
            var portReference = port.GetPortReference();
            if (_connections.Contains(portReference)) return;
            _connections.Add(portReference);
        }

        internal void RemoveConnections(IRuntimePort port)
        {
            var portReference = port.GetPortReference();
            _connections.Remove(portReference);
        }
        
        public RuntimePortReference GetPortReference() => new(_node, _direction, _index);
        
        public abstract bool TryGetValue<T>(out T value);
        public abstract bool TryGetNodeInput<T>(IQueryContext context, out T value);
        
        internal abstract bool TrySetValue<T>(T value);
        public abstract bool TrySetNodeOutput<T>(IQueryContext context, T value);

        public IEnumerable<IRuntimePort> GetConnectedPorts()
        {
            return _connections.Cast<IRuntimePort>();
        }
    }
}