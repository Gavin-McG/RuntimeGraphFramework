using System;
using System.Collections.Generic;
using System.Linq;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework
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
    }
}