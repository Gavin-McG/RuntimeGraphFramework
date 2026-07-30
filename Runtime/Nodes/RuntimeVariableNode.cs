using System;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeVariableNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference outputPort;
        [SerializeField] public RuntimeVariableKind variableKind;
        [SerializeField] public string variableName;
        
        public IRuntimeVariable Variable => Graph.GetVariable(variableName);
        
        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            // Get Variable
            if (variableKind == RuntimeVariableKind.Local)
            {
                if (!context.TryGetVariable(variableName, out object value))
                {
                    var variable = Graph.GetVariable(variableName);
                    if (variable == null || !variable.TryGetDefaultValue(out value)) return false;
                }
                return outputPort.TrySetNodeOutput(context, value);
            }
            
            // Get Input
            if (variableKind == RuntimeVariableKind.Input)
            {
                if (!context.TryGetInput(variableName, out IVariable input)) return false;
                return outputPort.TrySetNodeOutput(context, input);
            }
            
            // Output ports cant act as inputs
            return false;
        }
    }
}