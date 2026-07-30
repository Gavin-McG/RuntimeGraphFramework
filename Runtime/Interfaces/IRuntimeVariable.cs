using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum RuntimeVariableKind
    {
        Local,
        Input,
        Output,
    }
    
    public interface IRuntimeVariable
    {
        string Name { get; }
        Type DataType { get; }
        RuntimeGraph Graph { get; }
        Hash128 ID { get; }
        RuntimeVariableKind VariableKind { get; }
        int NodeCount { get; }
        
        IEnumerable<IRuntimeNode> GetNodes();
        
        bool TryGetDefaultValue<T>(out T defaultValue);
        internal bool TrySetDefaultValue<T>(T defaultValue);
        
        // void RemoveFromGraph(bool forceRemove);
    }
}