using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum SubgraphType
    {
        Local,
        Asset
    }
    
    public class RuntimeSubgraphNode : RuntimeNode
    {
        [SerializeField] public SubgraphType subgraphType;
        [SerializeField] public RuntimeGraph subgraph;
        
        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            context.EnterGraph(this);
            
            context.ExitGraph();
            return true;
        }
    }
}