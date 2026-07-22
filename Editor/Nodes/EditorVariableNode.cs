using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public class EditorVariableNode : IEditorNode<RuntimeVariableNode>, IEditorNodeOwner<RuntimeVariableNode>
    {
        private IVariableNode _variableNode;
        private EditorNodeModel<RuntimeVariableNode> _nodeModel;

        public EditorVariableNode(IVariableNode variableNode)
        {
            _variableNode = variableNode;
        }

        private EditorNodeModel<RuntimeVariableNode> NodeModel
        {
            get {
                if (_nodeModel == null) _nodeModel = new EditorNodeModel<RuntimeVariableNode>(this);
                return _nodeModel;
            }
        }
        
        // IEditorNodeOwner definition
        public Hash128 ID => _variableNode.ID;
        public IEnumerable<IPort> GetInputPorts() => _variableNode.GetInputPorts();
        public IEnumerable<IPort> GetOutputPorts() => _variableNode.GetOutputPorts();

        public void InitializeRuntimeNode(GraphImportContext context, RuntimeVariableNode node)
        {
            node.name = "VariableNode";

            var variable = _variableNode.Variable;
            var editorPort = variable.VariableKind switch
            {
                VariableKind.Local => _variableNode.GetOutputPort(0),
                VariableKind.Input => _variableNode.GetOutputPort(0),
                VariableKind.Output => _variableNode.GetInputPort(0),
                _ => throw new NotSupportedException()
            };
            node.outputPort = editorPort.GetRuntimePortReference(context);
            node.variableName = _variableNode.Variable.Name;
        }
        
        // IEditorNode definitions
        public bool IsCreated => NodeModel.IsCreated;
        public void ClearData() => NodeModel.ClearData();
        public RuntimeVariableNode GetRuntimeNode(GraphImportContext context) => NodeModel.GetRuntimeNode(context);
        public bool TryGetInputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetInputPortIndex(port, out portIndex);
        public bool TryGetOutputPortIndex(IPort port, out int portIndex) => NodeModel.TryGetOutputPortIndex(port, out portIndex);
    }
}