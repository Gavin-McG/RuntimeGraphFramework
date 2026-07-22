using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public class EditorConstantNode : IEditorNode<RuntimeConstantNode>, IEditorNodeOwner<RuntimeConstantNode>
    {
        private IConstantNode _constantNode;
        private EditorNodeModel<RuntimeConstantNode> _nodeModel;

        public EditorConstantNode(IConstantNode constantNode)
        {
            _constantNode = constantNode;
        }

        private EditorNodeModel<RuntimeConstantNode> NodeModel
        {
            get {
                if (_nodeModel == null) _nodeModel = new EditorNodeModel<RuntimeConstantNode>(this);
                return _nodeModel;
            }
        }
        
        // IEditorNodeOwner definition
        public Hash128 ID => _constantNode.ID;
        public IEnumerable<IPort> GetInputPorts() => _constantNode.GetInputPorts();
        public IEnumerable<IPort> GetOutputPorts() => _constantNode.GetOutputPorts();

        public void InitializeRuntimeNode(GraphImportContext context, RuntimeConstantNode node)
        {
            node.name = "ConstantNode";
            
            node._outputPort = _constantNode.GetOutputPort(0).GetRuntimePortReference(context);
            
            _constantNode.TryGetValue(out object value);
            var wrapperType = typeof(ValueWrapper<>).MakeGenericType(value.GetType());
            node._valueWrapper = (ValueWrapper)Activator.CreateInstance(wrapperType, value);
        }
        
        // IEditorNode definitions
        public bool IsCreated => NodeModel.IsCreated;
        public void ClearData() => NodeModel.ClearData();
        public RuntimeConstantNode GetRuntimeNode(GraphImportContext context) => NodeModel.GetRuntimeNode(context);
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);
    }
}