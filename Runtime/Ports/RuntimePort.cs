using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class RuntimePort : IRuntimePort
    {
        [SerializeField] private string _name;
        [SerializeField] private int _index;
        [SerializeField] private Hash128 _id;
        [SerializeField] private RuntimePortDirection _direction;
        [SerializeField] protected RuntimeNode _node;
        
        public string Name => _name;
        public Hash128 ID => _id;
        public RuntimePortDirection Direction => _direction;

        public abstract Type DataType { get; }
        public abstract bool IsConnected { get; }
        public abstract IRuntimePort FirstConnectedPort { get; }

        protected RuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
        {
            _name = name;
            _index = index;
            _id = id;
            _direction = direction;
            _node = node;
        }
        
        public IRuntimeNode GetNode() => _node;

        public virtual void Connect(RuntimePortReference portReference)
        {
            throw new NotImplementedException();
        }
        
        public RuntimePortReference GetPortReference() => new RuntimePortReference(_node, _direction, _index);
        
        public abstract bool TryGetValue<T>(IQueryContext context, out T value);
        public abstract bool TrySetValue<T>(IQueryContext context, T value);
    }
}