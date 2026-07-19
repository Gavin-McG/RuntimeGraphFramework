using System;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public class BlankQueryContext : IQueryContext
    {
        private static readonly System.Random rng = new();

        public GameObject gameObject => null;
        public Hash128 QueryID { get; private set; }

        public void RefreshQueryID()
        {
            QueryID = new Hash128(
                (uint)rng.Next(),
                (uint)rng.Next(),
                (uint)rng.Next(),
                (uint)rng.Next()
            );
        }

        public Type GetVariableType(string variableName) => null;

        public void ClearVariables()
        {
        }
        
        public bool TryGetVariable<T>(string variableName, out T value)
        {
            value = default;
            return true;
        }
        
        public bool TrySetVariable<T>(string variableName, T value)
        {
            return false;
        }
    }
}