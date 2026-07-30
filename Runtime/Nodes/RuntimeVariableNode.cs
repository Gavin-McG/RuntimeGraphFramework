using System;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeVariableNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference outputPort;
        [SerializeField] public RuntimeVariableKind variableKind;
        [SerializeField] public string variableName;
        
        public IRuntimeVariable Variable => Graph.variables
            .First(variable => variable.Name == variableName);
        
        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            if (variableKind == RuntimeVariableKind.Local)
            {
                if (!context.TryGetVariable(variableName, out object value)) return false;
                return outputPort.TrySetValue(value);
            }
            if (variableKind == RuntimeVariableKind.Input)
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