using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IQueryContext
    {
        Hash128 QueryID { get; }
        
        public bool TryGetVariable<T>(string variableName, out T value);
        public bool TryGetInput<T>(string inputName, out T value);
        
        public RuntimeGraph MainGraph { get; }
        public RuntimeGraph CurrentGraph { get; }
        
        public void EnterGraph(RuntimeSubgraphNode subgraphNode);
        public void ExitGraph();
    }
}