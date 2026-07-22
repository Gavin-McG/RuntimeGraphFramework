using System;
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
        Type DataType { get; }
        RuntimePortDirection Direction { get; }
        bool IsConnected { get; }
        IRuntimePort FirstConnectedPort { get; }
        
        IRuntimeNode GetNode();
        RuntimePortReference GetPortReference();
        
        bool TryGetValue<T>(IQueryContext context, out T value);
        bool TrySetValue<T>(IQueryContext context, T value);
    }
}
