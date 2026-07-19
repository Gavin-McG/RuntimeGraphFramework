using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public struct RuntimePortReference : IRuntimePort
    {
        [SerializeField] private RuntimeNode node;
        [SerializeField] private RuntimePortDirection direction;
        [SerializeField] private int index;

        public RuntimePortReference(RuntimeNode node, RuntimePortDirection direction, int index)
        {
            this.node = node;
            this.direction = direction;
            this.index = index;
        }
        
        public RuntimePort GetPort()
        {
            if (node == null) return null;
            if (direction == RuntimePortDirection.Input) return node.GetInputPort(index);
            if (direction == RuntimePortDirection.Output) return node.GetOutputPort(index);
            return null;
        }

        public bool IsValid => GetPort() != null;
        public string Name => GetPort()?.Name ?? String.Empty;
        public Hash128 ID => GetPort()?.ID ?? default;
        public Type DataType => GetPort()?.DataType;
        public RuntimePortDirection Direction => direction;
        public bool IsConnected => GetPort()?.IsConnected ?? false;
        public RuntimePort FirstConnectedPort => GetPort()?.FirstConnectedPort;
        
        public RuntimeNode GetNode() => node;

        public bool TryGetValue<T>(IQueryContext context, out T value)
        {
            var port = GetPort();
            if (port != null) return port.TryGetValue(context, out value);
            
            value = default;
            return false;
        }

        public bool TrySetValue<T>(IQueryContext context, T value)
        {
            var port = GetPort();
            if (port != null) return port.TrySetValue(context, value);
            
            return false;
        }
    }
}