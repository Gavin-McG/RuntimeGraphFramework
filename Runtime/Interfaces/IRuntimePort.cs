using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum RuntimePortDirection
    {
        Input,
        Output,
    }
    
    public interface IRuntimePort
    {
        string Name { get; }
        Hash128 ID { get; }
        RuntimePortDirection Direction { get; }
        bool IsConnected { get; }
        IRuntimePort FirstConnectedPort { get; }
        
        IRuntimeNode GetNode();
        RuntimePortReference GetPortReference();
        
        bool TryGetValue<T>(out T value);
        // bool TrySetValue<T>(T value);
        
        bool TryGetNodeInput<T>(IQueryContext context, out T value);
        bool TrySetNodeOutput<T>(IQueryContext context, T value);
        
        IEnumerable<IRuntimePort> GetConnectedPorts();
    }
}
