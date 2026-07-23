using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public class EditorSubgraphNode : IEditorNode<RuntimeSubgraphNode>, IEditorNodeOwner<RuntimeSubgraphNode>
    {
        private ISubgraphNode _subgraphNode;
        private EditorNodeModel<RuntimeSubgraphNode> _nodeModel;

        public EditorSubgraphNode(ISubgraphNode subgraphNode)
        {
            _subgraphNode = subgraphNode;
        }

        private EditorNodeModel<RuntimeSubgraphNode> NodeModel
        {
            get {
                if (_nodeModel == null) _nodeModel = new EditorNodeModel<RuntimeSubgraphNode>(this);
                return _nodeModel;
            }
        }
        
        // IEditorNodeOwner definition
        public Hash128 ID => _subgraphNode.ID;
        public IEnumerable<IPort> GetInputPorts() => _subgraphNode.GetInputPorts();
        public IEnumerable<IPort> GetOutputPorts() => _subgraphNode.GetOutputPorts();

        public void InitializeRuntimeNode(GraphImportContext context, RuntimeSubgraphNode node)
        {
            node.name = "SubgraphNode";
            
            Graph subgraph = _subgraphNode.GetSubgraph();
            var subgraphGuid = subgraph.AssetGuid;
            node.subgraphType = subgraphGuid==default ? SubgraphType.Local : SubgraphType.Asset;
            
            // Get subgraph
            if (subgraph is not EditorGraph editorGraph) 
                throw new NotSupportedException($"Subgraph is not EditorGraph {_subgraphNode.ID}");

            if (node.subgraphType == SubgraphType.Local)
            {
                var localGraph = editorGraph.CreateGraph(context);
                localGraph.name = _subgraphNode.Title;
                node.subgraph = localGraph;
            }
            else if (node.subgraphType == SubgraphType.Asset)
            {
                var assetGraph = AssetDatabase.LoadAssetByGUID<RuntimeGraph>(subgraph.AssetGuid);
                if (assetGraph == null || !assetGraph.IsValid) 
                    throw new Exception($"Could not reference Graph {subgraph.AssetGuid}");
                node.subgraph = assetGraph;
            }
            else throw new NotSupportedException();
        }
        
        // IEditorNode definitions
        public bool IsCreated => NodeModel.IsCreated;
        public void ClearData() => NodeModel.ClearData();
        public RuntimeSubgraphNode GetRuntimeNode(GraphImportContext context) => NodeModel.GetRuntimeNode(context);
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);
    }
}