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

        private static readonly MethodInfo TryGetDefaultValueMethod =
            typeof(IVariable).GetMethod(nameof(IVariable.TryGetDefaultValue));

        private static readonly MethodInfo CreateVariableMethod =
            typeof(RuntimeVariable).GetMethod(nameof(RuntimeVariable.CreateVariable));
        
        public static RuntimeVariable GetRuntimeVariable(this IVariable variable)
        {
            Type type = variable.DataType;
            if (type == typeof(Untyped)) return null;
            
            // Get default value of variable
            MethodInfo tryGetMethod = TryGetDefaultValueMethod.MakeGenericMethod(type);
            object[] args = { null };
            bool success = (bool)tryGetMethod.Invoke(variable, args);

            if (!success) throw new InvalidOperationException($"Could not get default value for {type}.");

            // Create variable from value
            object value = args[0];
            MethodInfo createMethod = CreateVariableMethod.MakeGenericMethod(type);
            return (RuntimeVariable)createMethod.Invoke(null, new[] { value });
        }
    }
}