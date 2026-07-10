using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

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

        public static Parameter GetParameter(this IVariable variable)
        {
            Type variableType = variable.DataType;
            if (variableType == typeof(int))
            {
                // Integer variable
                variable.TryGetDefaultValue(out int value);
                return Parameter.CreateParameter(value);
            }
            if (variableType == typeof(float))
            {
                // Float variable
                variable.TryGetDefaultValue(out float value);
                return Parameter.CreateParameter(value);
            }
            if (variableType == typeof(string))
            {
                // String variable
                variable.TryGetDefaultValue(out string value);
                return Parameter.CreateParameter(value);
            }
            if (variableType == typeof(bool))
            {
                // GameObject Variable
                variable.TryGetDefaultValue(out bool value);
                return Parameter.CreateParameter(value);
            }
            if (variableType == typeof(GameObject))
            {
                // GameObject Variable
                variable.TryGetDefaultValue(out GameObject value);
                return Parameter.CreateParameter(value);
            }
            if (variableType == typeof(Color))
            {
                variable.TryGetDefaultValue(out Color value);
                return Parameter.CreateParameter(value);
            }
            throw new ArgumentException("Unsupported variable type: " + variableType);
        }
    }
}