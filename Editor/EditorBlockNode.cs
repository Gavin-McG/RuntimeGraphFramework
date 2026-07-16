using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorBlockNode<TRuntimeNode, TGraph> : BlockNode, IEditorBlockNode<TRuntimeNode>, IEditorNodeOwner<TRuntimeNode>
        where TRuntimeNode : RuntimeBlockNode
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

        public void InitializeRuntimeNode(DialogueImportContext context, TRuntimeNode node)
        {
            // Set Context Node
            if (ContextNode is not IEditorContextNode<RuntimeContextNode> editorContextNode)
                Debug.LogError($"Node {ContextNode.GetType().Name} can only contain EditorBlockNodes if it is an EditorContextNode");
            else
                node.contextNode = editorContextNode.GetRuntimeNode(context);
            
            // Set block index
            node.index = Index;
            
            DefineRuntimeNode(context, node);
        }

        protected abstract void DefineRuntimeNode(DialogueImportContext context, TRuntimeNode node);
    }
}