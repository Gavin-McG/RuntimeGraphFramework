using UnityEngine;

namespace RuntimeGraphFramework
{
    public abstract class RuntimeBlockNode : RuntimeNode, IRuntimeBlockNode
    {
        [SerializeField] public RuntimeContextNode contextNode;
        [SerializeField] public int index;
        
        public IRuntimeContextNode ContextNode => contextNode;
        public int Index => index;
        
        public sealed override bool IsConstantNode() => false;
    }
}