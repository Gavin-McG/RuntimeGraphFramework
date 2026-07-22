using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
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
        }
        
        // IEditorNode definitions
        public bool IsCreated => NodeModel.IsCreated;
        public void ClearData() => NodeModel.ClearData();
        public RuntimeSubgraphNode GetRuntimeNode(GraphImportContext context) => NodeModel.GetRuntimeNode(context);
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);
    }
}