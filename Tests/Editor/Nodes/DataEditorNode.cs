using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Serializable]
    public class DataEditorNode : EditorNode<DataNode>
    {
        private IPort _prevPort;
        private IPort _nextPort;
        private IPort _dataPort;
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _prevPort = context.AddInputPort("Previous").Build();
            _nextPort = context.AddOutputPort("Next").Build();
            _dataPort = context.AddInputPort<float>("Data").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, DataNode node)
        {
            node.nextNode = _nextPort.FirstConnectedPort.GetNode().GetRuntimeNode(context);
            node.dataInput = _dataPort.GetRuntimePortReference(context);
        }
    }
}