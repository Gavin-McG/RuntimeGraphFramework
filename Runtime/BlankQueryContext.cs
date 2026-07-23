using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public class BlankQueryContext : IQueryContext
    {
        private static readonly System.Random rng = new();

        public GameObject gameObject => null;
        public Hash128 QueryID { get; private set; }

        public BlankQueryContext()
        {
            RefreshQueryID();
        }

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
            return false;
        }

        public bool TryGetInput<T>(string inputName, out T value)
        {
            throw new NotImplementedException();
        }

        public RuntimeGraph MainGraph { get; }
        public RuntimeGraph CurrentGraph { get; }
        public void EnterGraph(RuntimeSubgraphNode subgraphNode)
        {
            throw new NotImplementedException();
        }

        public void ExitGraph()
        {
            throw new NotImplementedException();
        }

        public bool TrySetVariable<T>(string variableName, T value)
        {
            return false;
        }
    }
}