using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorBlockNode<TRuntimeNode> : BlockNode, IEditorBlockNode<TRuntimeNode>, IEditorNodeOwner<TRuntimeNode>
        where TRuntimeNode : RuntimeBlockNode
    {
        private EditorNodeModel<TRuntimeNode> _nodeModel;

        private EditorNodeModel<TRuntimeNode> NodeModel
        {
            get {
                if (_nodeModel == null) _nodeModel = new EditorNodeModel<TRuntimeNode>(this);
                return _nodeModel;
            }
        }
        
        public bool IsCreated => NodeModel.IsCreated;
        public void ClearData() => NodeModel.ClearData();
        public TRuntimeNode GetRuntimeNode(GraphImportContext context) => NodeModel.GetRuntimeNode(context);
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);

        public void InitializeRuntimeNode(GraphImportContext context, TRuntimeNode node)
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

        protected abstract void DefineRuntimeNode(GraphImportContext context, TRuntimeNode node);
    }
}