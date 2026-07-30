using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public class EditorMissingNode : IEditorNode<RuntimeMissingNode>, IEditorNodeOwner<RuntimeMissingNode>
    {
        private INode _node;
        private EditorNodeModel<RuntimeMissingNode> _nodeModel;

        public EditorMissingNode(INode node)
        {
            _node = node;
        }

        private IEditorNode<RuntimeMissingNode> NodeModel
        {
            get {
                if (_nodeModel == null) _nodeModel = new EditorNodeModel<RuntimeMissingNode>(this);
                return _nodeModel;
            }
        }
        
        // IEditorNodeOwner definition
        public Hash128 ID => _node.ID;
        public IEnumerable<IPort> GetInputPorts() => _node.GetInputPorts();
        public IEnumerable<IPort> GetOutputPorts() => _node.GetOutputPorts();

        void IEditorNode<RuntimeMissingNode>.InitializeRuntimeNode(GraphImportContext context)
        {
            var node = NodeModel.RuntimeNode;
            node.name = "MissingNode";
        }
        
        // IEditorNode definitions
        public RuntimeMissingNode RuntimeNode => NodeModel.RuntimeNode;
        public void ClearData() => NodeModel.ClearData();
        void IEditorNode<RuntimeMissingNode>.CreateRuntimeNode(GraphImportContext context) => NodeModel.CreateRuntimeNode(context);
        void IEditorNode<RuntimeMissingNode>.ConnectRuntimeNode(GraphImportContext context) => NodeModel.ConnectRuntimeNode(context);
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);
    }
}