using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorContextNode<TRuntimeNode> : ContextNode, IEditorContextNode<TRuntimeNode>, IEditorNodeOwner<TRuntimeNode> 
        where TRuntimeNode : RuntimeContextNode
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
            // Add block Nodes
            node.blockNodes = new List<RuntimeBlockNode>(BlockCount);
            foreach (var blockNode in BlockNodes)
            {
                if (blockNode is not IEditorBlockNode<RuntimeBlockNode> editorBlockNode)
                {
                    Debug.LogWarning($"Block {blockNode.GetType().Name} must be derive from EditorBlockNode to be included in EditorContextNode");
                    continue;
                }
                node.blockNodes.Add(editorBlockNode.GetRuntimeNode(context));
            }
            
            DefineRuntimeNode(context, node);
        }

        protected abstract void DefineRuntimeNode(GraphImportContext context, TRuntimeNode node);
    }
}