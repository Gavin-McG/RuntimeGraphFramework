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
        
        protected override void UpdateNodeOutputs(IQueryContext context)
        {
            //TODO
        }
    }
}