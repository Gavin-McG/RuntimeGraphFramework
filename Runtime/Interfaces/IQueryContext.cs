using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IQueryContext
    {
        public RuntimeGraph MainGraph { get; }
        public RuntimeGraph CurrentGraph { get; }
        
        public void EnterGraph(RuntimeSubgraphNode subgraphNode);
        public void ExitGraph();
        
        void SetInput(string inputName, object value);
        bool TryGetInput(string inputName, out object value);
        
        void SetVariable(string variableName, object value);
        bool TryGetVariable(string variableName, out object value);
        
        void SetPortOutput(Hash128 portID, object value);
        bool TryGetPortOutput(Hash128 portID, out object value);
    }
}