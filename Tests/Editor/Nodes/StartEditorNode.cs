using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Serializable]
    public class StartEditorNode : EditorNode<StartNode>
    {
        private IPort _inputPort;
        private IPort _nextPort;
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _inputPort = context.AddInputPort<string>("Input").Build();
            _nextPort = context.AddOutputPort("Next").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, StartNode node)
        {
            node._inputPort = _inputPort.GetRuntimePortReference(context);
            node.nextNode = _nextPort.FirstConnectedPort.GetNode().GetRuntimeNode(context);
        }
    }
}