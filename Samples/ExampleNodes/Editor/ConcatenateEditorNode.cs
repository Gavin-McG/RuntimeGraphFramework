using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.ExampleNodes.Editor
{
    [Serializable]
    public class ConcatenateEditorNode : EditorNode<ConcatenateNode>
    {
        private IPort _lhsPort;
        private IPort _rhsPort;
        private IPort _outputPort;

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _lhsPort = context.AddInputPort<string>("lhs").Build();
            _rhsPort = context.AddInputPort<string>("rhs").Build();
            
            _outputPort = context.AddOutputPort<string>("output").Build();
        }

        protected override void DefineRuntimeNode(GraphImportContext context, ConcatenateNode node)
        {
            node.lhsPort = _lhsPort.GetRuntimePortReference(context);
            node.rhsPort = _rhsPort.GetRuntimePortReference(context);
            
            node.outputPort = _outputPort.GetRuntimePortReference(context);
        }
    }
}