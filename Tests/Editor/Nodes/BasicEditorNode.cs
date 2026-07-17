using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Serializable]
    public class BasicEditorNode : EditorNode<BasicNode>
    {
        private IPort _prevPort;
        private IPort _nextPort;
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _prevPort = context.AddInputPort("Previous").Build();
            _nextPort = context.AddOutputPort("Next").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, BasicNode node)
        {
            node.nextNode = _nextPort.GetConnectedRuntimeNode<RuntimeNode>(context);
        }
    }
}