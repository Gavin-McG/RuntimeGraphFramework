using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IRuntimePort
    {
        string Name { get; }
        Hash128 ID { get; }
        Type DataType { get; }
        RuntimePortDirection Direction { get; }
        bool IsConnected { get; }
        RuntimePort FirstConnectedPort { get; }
        
        RuntimeNode GetNode();
        
        bool TryGetValue<T>(IQueryContext context, out T value);
        bool TrySetValue<T>(IQueryContext context, T value);
    }
}
