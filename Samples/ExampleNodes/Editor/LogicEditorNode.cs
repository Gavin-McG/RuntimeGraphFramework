using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes.Editor
{
    [Serializable]
    public class LogicEditorNode : EditorNode<LogicNode>
    {
        private INodeOption _operationOption;
        
        private IPort _inputPort1;
        private IPort _inputPort2;
        private IPort _outputPort;

        public override void OnEnable()
        {
            Title = "Logic";
            DefaultColor = Color.aquamarine;
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            _operationOption = context.AddOption<LogicNode.Operation>("op").Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _operationOption.TryGetValue(out LogicNode.Operation operation);
            
            _inputPort1 = context.AddInputPort<bool>("lhs").Build();
            _inputPort2 = LogicNode.HasSecondInput(operation) ? 
                context.AddInputPort<bool>("rhs").Build() :
                null;
            _outputPort = context.AddOutputPort<bool>("out").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, LogicNode node)
        {
            _operationOption.TryGetValue(out node.operation);
            
            node.inputPort1 = _inputPort1.GetRuntimePortReference(context);
            node.inputPort2 = _inputPort2.GetRuntimePortReference(context);
            node.outputPort = _outputPort.GetRuntimePortReference(context);
        }
    }
}