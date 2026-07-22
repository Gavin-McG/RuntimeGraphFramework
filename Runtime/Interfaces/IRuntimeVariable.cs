using System;
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
        
        bool TryGetDefaultValue<T>(out T defaultValue);
    }
}