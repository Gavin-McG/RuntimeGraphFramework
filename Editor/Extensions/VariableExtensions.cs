using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public static class VariableExtensions
    {
        public static IEnumerable<IVariableNode> GetNodes(this IVariable variable)
        {
            var variableNodes = new List<IVariableNode>();
            variable.GetNodes(variableNodes);
            return variableNodes;
        }

        public static RuntimeVariableKind GetRuntimeVariableKind(this IVariable variable) =>
            variable.VariableKind switch
            {
                VariableKind.Local => RuntimeVariableKind.Local,
                VariableKind.Input => RuntimeVariableKind.Input,
                VariableKind.Output => RuntimeVariableKind.Output,
                _ => throw new NotSupportedException()
            };
        
        public static RuntimeVariable CreateRuntimeVariable(this IVariable variable, GraphImportContext context)
        {
            variable.TryGetDefaultValue(out object defaultValue);
            
            return new RuntimeVariable(
                variable.Name, 
                context.Graph, 
                variable.ID, 
                variable.GetRuntimeVariableKind(), 
                defaultValue);
        }
    }
}