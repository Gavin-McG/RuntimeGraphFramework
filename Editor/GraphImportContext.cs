using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public class GraphImportContext
    {
        public AssetImportContext assetContext;
        private readonly Stack<RuntimeGraph> graphStack = new Stack<RuntimeGraph>();

        private readonly Dictionary<Hash128, EditorConstantNode> constantNodes = new();
        private readonly Dictionary<Hash128, EditorVariableNode> variableNodes = new();
        private readonly Dictionary<Hash128, EditorSubgraphNode> subgraphNodes = new();
        private readonly Dictionary<Hash128, EditorMissingNode> missingNodes = new();
        
        private readonly List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
        
        public RuntimeGraph Graph => graphStack.Peek();
        public Type GraphType => graphStack.Peek().GetType();
        
        public IEnumerable<EditorConstantNode> ConstantNodes => constantNodes.Values;
        public IEnumerable<EditorVariableNode> VariableNodes => variableNodes.Values;
        public IEnumerable<EditorSubgraphNode> SubgraphNodes => subgraphNodes.Values;
        public IEnumerable<EditorMissingNode> MissingNodes => missingNodes.Values;
        
        public IEnumerable<UnityEngine.Object> Assets => assets;

        public void EnterGraph(RuntimeGraph graph)
        {
            graphStack.Push(graph);
        }

        public void ExitGraph()
        {
            graphStack.Pop();
        }

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

        public void AddAsset(UnityEngine.Object asset)
        {
            assets.Add(asset);
        }
    }
}