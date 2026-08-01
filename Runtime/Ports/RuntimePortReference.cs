using System;
using System.Collections.Generic;
using System.Linq;
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
        
        internal RuntimePort GetPort()
        {
            if (node == null) return null;
            if (direction == RuntimePortDirection.Input) return node.inputPorts.ElementAtOrDefault(index);
            if (direction == RuntimePortDirection.Output) return node.outputPorts.ElementAtOrDefault(index);
            return null;
        }

        public bool IsValid => GetPort() != null;
        public string Name => GetPort()?.Name ?? String.Empty;
        public Hash128 ID => GetPort()?.ID ?? default;
        public RuntimePortDirection Direction => direction;
        public bool IsConnected => GetPort()?.IsConnected ?? false;
        public IRuntimePort FirstConnectedPort => GetPort()?.FirstConnectedPort;
        public int Index => index;
        
        public IRuntimeNode GetNode() => node;
        
        public RuntimePortReference GetPortReference() => this;

        public bool TryGetValue<T>(out T value)
        {
            var port = GetPort();
            if (port != null) return port.TryGetValue(out value);
            
            value = default;
            return false;
        }
        
        public bool TryGetNodeInput<T>(IQueryContext context, out T value)
        {
            var port = GetPort();
            if (port != null) return port.TryGetNodeInput(context, out value);
            
            value = default;
            return false;
        } 
        
        internal bool TrySetValue<T>(T value)
        {
            var port = GetPort();
            if (port != null) return port.TrySetValue(value);
            
            return false;
        }

        public bool TrySetNodeOutput<T>(IQueryContext context, T value)
        {
            var port = GetPort();
            if (port != null) return port.TrySetNodeOutput(context, value);
            
            return false;
        }


        public IEnumerable<IRuntimePort> GetConnectedPorts()
        {
            var port = GetPort();
            if (port != null) return port.GetConnectedPorts();
            
            return new List<IRuntimePort>();
        }
    }
}