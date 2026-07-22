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
        public RuntimeGraph runtimeGraph;
        public HashSet<IVariable> validVariables;

        private readonly Dictionary<Hash128, EditorConstantNode> constantNodes = new();
        
        public Type GraphType => runtimeGraph.GetType();
        public IEnumerable<EditorConstantNode> ConstantNodes => constantNodes.Values;

        public EditorConstantNode GetConstantNode(IConstantNode constantNode)
        {
            Hash128 nodeID = constantNode.ID;
            if (constantNodes.TryGetValue(nodeID, out EditorConstantNode editorNode)) return editorNode;
            
            // Create new EditorConstantNode
            var newNode = new EditorConstantNode(constantNode);
            constantNodes.Add(nodeID, newNode);
            return newNode;
        }
    }
}