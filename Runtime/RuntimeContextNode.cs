using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeGraphFramework
{
    public class RuntimeContextNode : RuntimeNode, IRuntimeContextNode
    {
        [SerializeField] public List<RuntimeBlockNode> blockNodes;
        
        public int BlockCount => blockNodes.Count;
        public IEnumerable<RuntimeBlockNode> BlockNodes => blockNodes;

        public sealed override bool IsConstantNode() => false;
    }
}