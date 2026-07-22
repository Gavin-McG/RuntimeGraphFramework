using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeVariableNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference outputPort;
        [SerializeField] public string variableName;
        
        protected override void UpdateNodeOutputs(IQueryContext context)
        {
            context.TryGetVariable(variableName, out object value);
            outputPort.TrySetValue(context, value);
        }
    }
}