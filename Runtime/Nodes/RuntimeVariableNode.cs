using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeVariableNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference outputPort;
        [SerializeField] public RuntimeVariableKind variableKind;
        [SerializeField] public string variableName;
        
        protected override void UpdateNodeOutputs(IQueryContext context)
        {
            if (variableKind == RuntimeVariableKind.Local)
            {
                context.TryGetVariable(variableName, out object value);
                outputPort.TrySetValue(context, value);
            }
            else if (variableKind == RuntimeVariableKind.Input)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}