using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes.Editor
{
    [Serializable]
    public class ComparisonEditorNode : EditorNode<ComparisonNode>
    {
        private INodeOption _operationOption;
        
        private IPort _inputPort1;
        private IPort _inputPort2;
        private IPort _outputPort;

        public override void OnEnable()
        {
            Title = "Comparison";
            DefaultColor = Color.aquamarine;
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            _operationOption = context.AddOption<ComparisonNode.Operation>("op").Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _inputPort1 = context.AddInputPort<float>("lhs").Build();
            _inputPort2 = context.AddInputPort<float>("rhs").Build();
            _outputPort = context.AddOutputPort<bool>("out").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, ComparisonNode node)
        {
            _operationOption.TryGetValue(out node.operation);
            
            node.inputPort1 = _inputPort1.GetRuntimePortReference(context);
            node.inputPort2 = _inputPort2.GetRuntimePortReference(context);
            node.outputPort = _outputPort.GetRuntimePortReference(context);
        }
    }
}