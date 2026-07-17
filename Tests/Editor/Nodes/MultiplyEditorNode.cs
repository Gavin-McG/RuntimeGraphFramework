using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Serializable]
    public class MultiplyEditorNode : EditorNode<MultiplyNode>
    {
        private IPort _input1Port;
        private IPort _input2Port;
        private IPort _outputPort;
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _input1Port = context.AddInputPort<float>("Input 1").Build();
            _input2Port = context.AddInputPort<float>("Input 2").Build();
            _outputPort = context.AddOutputPort<float>("Output").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, MultiplyNode node)
        {
            node.input1 = _input1Port.GetInputPortReference(context);
            node.input2 = _input2Port.GetInputPortReference(context);
            node.output = _outputPort.GetOutputPortReference(context);
        }
    }
}