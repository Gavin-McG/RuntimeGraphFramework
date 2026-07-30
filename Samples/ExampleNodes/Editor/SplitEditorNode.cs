using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes.Editor
{
    [Serializable]
    public class SplitEditorNode : EditorNode<SplitNode>
    {
        private enum VectorType
        {
            Vector2,
            Vector3,
            Vector4,
        }
        
        private INodeOption _vectorTypeOption;
        
        private IPort _inputPort;
        private IPort _outputPort_x;
        private IPort _outputPort_y;
        private IPort _outputPort_z;
        private IPort _outputPort_w;

        public override void OnEnable()
        {
            Title = "Split";
            DefaultColor = Color.aquamarine;
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            _vectorTypeOption = context.AddOption<VectorType>("type").Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _vectorTypeOption.TryGetValue(out VectorType vectorType);
            switch (vectorType)
            {
                case VectorType.Vector2:
                    _inputPort = context.AddInputPort<Vector2>("in").Build();
                    _outputPort_x = context.AddOutputPort<float>("x").Build();
                    _outputPort_y = context.AddOutputPort<float>("y").Build();
                    break;
                case VectorType.Vector3:
                    _inputPort = context.AddInputPort<Vector3>("in").Build();
                    _outputPort_x = context.AddOutputPort<float>("x").Build();
                    _outputPort_y = context.AddOutputPort<float>("y").Build();
                    _outputPort_z = context.AddOutputPort<float>("z").Build();
                    break;
                case VectorType.Vector4:
                    _inputPort = context.AddInputPort<Vector4>("in").Build();
                    _outputPort_x = context.AddOutputPort<float>("x").Build();
                    _outputPort_y = context.AddOutputPort<float>("y").Build();
                    _outputPort_z = context.AddOutputPort<float>("z").Build();
                    _outputPort_w = context.AddOutputPort<float>("w").Build();
                    break;
                default: throw new NotSupportedException("Unknown vector type");
            }
        }

        protected override void DefineRuntimeNode(GraphImportContext context, SplitNode node)
        {
            node.inputPort = _inputPort.GetRuntimePortReference(context);
            
            node.outputPort_x = _outputPort_x.GetRuntimePortReference(context);
            node.outputPort_y = _outputPort_y.GetRuntimePortReference(context);
            node.outputPort_z = _outputPort_z.GetRuntimePortReference(context);
            node.outputPort_w = _outputPort_w.GetRuntimePortReference(context);
        }
    }
}