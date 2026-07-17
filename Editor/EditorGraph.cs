using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorGraph<TGraph> : Graph where TGraph : RuntimeGraph
    {
        public bool CanTypesConnect(Type outputType, Type inputType)
        {
            return PortTypeCastManager.CanTypesCast<TGraph>(outputType, inputType);
        }
        
        public override bool IsConnectionAllowed(IPort output, IPort input)
        {
            // Check for Types
            Type outputType = output.DataType;
            Type inputType = input.DataType;
            if (!CanTypesConnect(outputType, inputType)) return false;
            
            // Check for Recursion of data ports
            INode outputNode = output.GetNode();
            INode inputNode = input.GetNode();
            if (outputNode == null || inputNode == null || !outputNode.WouldConnectionCreateCycle(inputNode)) return false;
            
            // Prevent Multiple Untyped input Variable connections
            if (outputNode is IVariableNode variableNode)
            {
                IVariable outputVariable = variableNode.Variable;
                if (outputVariable.VariableKind == VariableKind.Input && outputVariable.DataType == typeof(Untyped))
                {
                    var variableNodes = new List<IVariableNode>();
                    outputVariable.GetNodes(variableNodes);
                    return !variableNodes.Any(node => node.IsConnected);
                }
            }

            // Allow by Default
            return true;
        }
        
        public IEnumerable<IGrouping<string, IVariable>> GetVariableGroups()
        {
            return GetVariables(SortMethod.Creation).GroupBy(variable => variable.Name);
        }

        public HashSet<IVariable> GetValidVariables(IEnumerable<IGrouping<string, IVariable>> variableGroups)
        {
            return variableGroups
                .Where(group => group.Count() == 1 && group.First().DataType != typeof(Untyped))
                .Select(group => group.First())
                .ToHashSet();
        }

        public HashSet<IVariable> GetValidVariables()
        {
            return GetValidVariables(GetVariableGroups());
        }
    }
}