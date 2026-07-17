using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Serializable]
    public class StartEditorNode : EditorNode<StartNode>
    {
        private IPort _nextPort;
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _nextPort = context.AddOutputPort("Next").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, StartNode node)
        {
            node.nextNode = _nextPort.GetConnectedRuntimeNode<RuntimeNode>(context);
        }
    }
}