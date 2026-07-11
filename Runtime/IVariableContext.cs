using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IVariableContext
    {
        public void ClearVariables();
        public Type GetVariableType(string variableName);
        
        public void SetVariable<T>(string variableName, T value);

        public bool TryGetVariable<T>(string variableName, out T value);
    }
}