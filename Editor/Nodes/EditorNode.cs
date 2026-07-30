using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorNode<TRuntimeNode> : Node, IEditorNode<TRuntimeNode>, IEditorNodeOwner<TRuntimeNode>
        where TRuntimeNode : RuntimeNode 
    {
        private EditorNodeModel<TRuntimeNode> _nodeModel;

        private IEditorNode<TRuntimeNode> NodeModel
        {
            get {
                if (_nodeModel == null) _nodeModel = new EditorNodeModel<TRuntimeNode>(this);
                return _nodeModel;
            }
        }
        
        public TRuntimeNode RuntimeNode => NodeModel.RuntimeNode;
        
        public void ClearData() => NodeModel.ClearData();
        void IEditorNode<TRuntimeNode>.CreateRuntimeNode(GraphImportContext context) => NodeModel.CreateRuntimeNode(context);
        void IEditorNode<TRuntimeNode>.ConnectRuntimeNode(GraphImportContext context) => NodeModel.ConnectRuntimeNode(context);
        void IEditorNode<TRuntimeNode>.InitializeRuntimeNode(GraphImportContext context) => DefineRuntimeNode(context, RuntimeNode);
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);
        
        protected abstract void DefineRuntimeNode(GraphImportContext context, TRuntimeNode node);
    }
}