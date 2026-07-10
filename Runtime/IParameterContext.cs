using System;
using UnityEngine;

namespace BehavioralDialogue
{
    public interface IParameterContext
    {
        public void ClearParameters();
        public Type GetParameterType(string parameterName);
        
        public void SetValue<T>(string parameterName, T value);

        public bool TryGetValue<T>(string parameterName, out T value);
    }
}