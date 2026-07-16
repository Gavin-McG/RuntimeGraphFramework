using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public abstract class EditorBlockNode<TRuntimeNode, TGraph> : BlockNode, IEditorNode<TRuntimeNode>
        where TRuntimeNode : RuntimeBlockNode
        where TGraph : RuntimeGraph
    {
        private Dictionary<Hash128, TRuntimeNode> _nodes = new();

        public void ClearData()
        {
            _nodes.Clear();
        }
        
        private TRuntimeNode GetRegisteredNode(DialogueImportContext context)
        {
            Hash128 nodeKey = default;
            if (context.currentSubgraph != null) nodeKey = context.currentSubgraph.ID;
            return _nodes.GetValueOrDefault(nodeKey);
        }

        private void RegisterNode(DialogueImportContext context, TRuntimeNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            Hash128 nodeKey = default;
            if (context.currentSubgraph != null) nodeKey = context.currentSubgraph.ID;
            _nodes[nodeKey] = node;
        }

        public TRuntimeNode GetRuntimeNode(DialogueImportContext context)
        {
            // Check if already Initialized
            var currentNode = GetRegisteredNode(context);
            if (currentNode != null) return currentNode;
            
            // Create instance
            var newNode = ScriptableObject.CreateInstance<TRuntimeNode>();
            RegisterNode(context, newNode);

            // Set ID
            newNode.nodeID = ID;
            if (context.currentSubgraph != null) 
                newNode.nodeID.Append(context.currentSubgraph.ID.ToString());
            
            // set ContextNode info
            if (ContextNode is IEditorNode<RuntimeContextNode> contextNode)
                newNode.contextNode = contextNode.GetRuntimeNode(context);
            
            // Initialize Node Object
            InitializeRuntimeNode(context, ref newNode);
            return newNode;
        }
    }
}