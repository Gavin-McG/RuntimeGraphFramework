using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorNode<TRuntimeNode, TGraph> : Node, IEditorNode<TRuntimeNode>, IEditorNodeOwner<TRuntimeNode>
        where TRuntimeNode : RuntimeNode 
        where TGraph : RuntimeGraph
    {
        private EditorNodeModel<TRuntimeNode, TGraph> _nodeModel;

        private EditorNodeModel<TRuntimeNode, TGraph> NodeModel
        {
            get {
                if (_nodeModel == null) _nodeModel = new EditorNodeModel<TRuntimeNode, TGraph>(this);
                return _nodeModel;
            }
        }
        
        public void ClearData() => NodeModel.ClearData();
        public TRuntimeNode GetRuntimeNode(DialogueImportContext context) => NodeModel.GetRuntimeNode(context);
        public IEnumerable<TRuntimeNode> GetRuntimeNodes() => NodeModel.GetRuntimeNodes();
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);
        public void InitializeRuntimeNode(DialogueImportContext context, TRuntimeNode node) => DefineRuntimeNode(context, node);
        
        protected abstract void DefineRuntimeNode(DialogueImportContext context, TRuntimeNode node);
    }
}