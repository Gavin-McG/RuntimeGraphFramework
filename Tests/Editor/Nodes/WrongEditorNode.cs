using System;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Serializable]
    public class WrongNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<float>("Input").Build();
        }
    }
}