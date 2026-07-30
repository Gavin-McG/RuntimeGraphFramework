using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public class GraphImportContext
    {
        private class ImportStackEntry
        {
            private RuntimeGraph graph;
            private readonly Dictionary<Hash128, EditorConstantNode> constantNodes = new();
            private readonly Dictionary<Hash128, EditorVariableNode> variableNodes = new();
            private readonly Dictionary<Hash128, EditorSubgraphNode> subgraphNodes = new();
            private readonly Dictionary<Hash128, EditorMissingNode> missingNodes = new();

            public ImportStackEntry(RuntimeGraph graph)
            {
                this.graph = graph;
            }

            public RuntimeGraph Graph => graph;
            
            public IEnumerable<EditorConstantNode> ConstantNodes => constantNodes.Values;
            public IEnumerable<EditorVariableNode> VariableNodes => variableNodes.Values;
            public IEnumerable<EditorSubgraphNode> SubgraphNodes => subgraphNodes.Values;
            public IEnumerable<EditorMissingNode> MissingNodes => missingNodes.Values;
            
            public EditorConstantNode GetConstantNode(IConstantNode constantNode)
            {
                Hash128 nodeID = constantNode.ID;
                if (constantNodes.TryGetValue(nodeID, out EditorConstantNode editorNode)) return editorNode;
            
                // Create new EditorConstantNode
                var newNode = new EditorConstantNode(constantNode);
                constantNodes.Add(nodeID, newNode);
                return newNode;
            }

            public EditorVariableNode GetVariableNode(IVariableNode variableNode)
            {
                Hash128 nodeID = variableNode.ID;
                if (variableNodes.TryGetValue(nodeID, out EditorVariableNode editorNode)) return editorNode;
            
                // Create new EditorVariableNode
                var newNode = new EditorVariableNode(variableNode);
                variableNodes.Add(nodeID, newNode);
                return newNode;
            }

            public EditorSubgraphNode GetSubgraphNode(ISubgraphNode subgraphNode)
            {
                Hash128 nodeID = subgraphNode.ID;
                if (subgraphNodes.TryGetValue(nodeID, out EditorSubgraphNode editorNode)) return editorNode;
            
                // Create new EditorSubgraphNode
                var newNode = new EditorSubgraphNode(subgraphNode);
                subgraphNodes.Add(nodeID, newNode);
                return newNode;
            }

            public EditorMissingNode GetMissingNode(INode node)
            {
                Hash128 nodeID = node.ID;
                if (missingNodes.TryGetValue(nodeID, out EditorMissingNode editorNode)) return editorNode;
            
                // Create new EditorMissingNode
                var newNode = new EditorMissingNode(node);
                missingNodes.Add(nodeID, newNode);
                return newNode;
            }
        }
        
        public AssetImportContext assetContext;
        private readonly Stack<ImportStackEntry> graphStack = new();
        private readonly List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
        
        // Public Methods
        public RuntimeGraph Graph => graphStack.Peek().Graph;
        public Type GraphType => graphStack.Peek().GetType();
        
        public void AddAsset(UnityEngine.Object asset)
        {
            assets.Add(asset);
        }

        // Internal Methods
        internal IEnumerable<EditorConstantNode> ConstantNodes => graphStack.Peek().ConstantNodes;
        internal IEnumerable<EditorVariableNode> VariableNodes => graphStack.Peek().VariableNodes;
        internal IEnumerable<EditorSubgraphNode> SubgraphNodes => graphStack.Peek().SubgraphNodes;
        internal IEnumerable<EditorMissingNode> MissingNodes => graphStack.Peek().MissingNodes;
        
        internal IEnumerable<UnityEngine.Object> Assets => assets;
        
        private bool HasEnteredGraph(RuntimeGraph graph)
        {
            foreach (var entry in graphStack)
            {
                if (entry.Graph == graph) return true;
            }
            return false;
        }

        internal void EnterGraph(RuntimeGraph graph)
        {
            if (HasEnteredGraph(graph)) throw new Exception("Graph recursion is not supported");
            graphStack.Push(new ImportStackEntry(graph));
        }

        internal void ExitGraph()
        {
            graphStack.Pop();
        }

        internal EditorConstantNode GetConstantNode(IConstantNode constantNode) => 
            graphStack.Peek().GetConstantNode(constantNode);
        
        internal EditorVariableNode GetVariableNode(IVariableNode variableNode) =>
            graphStack.Peek().GetVariableNode(variableNode);

        internal EditorSubgraphNode GetSubgraphNode(ISubgraphNode subgraphNode) =>
            graphStack.Peek().GetSubgraphNode(subgraphNode);

        internal EditorMissingNode GetMissingNode(INode node) =>
            graphStack.Peek().GetMissingNode(node);
    }
}