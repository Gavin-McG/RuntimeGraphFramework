using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum SubgraphType
    {
        Local,
        Asset
    }
    
    public class RuntimeSubgraphNode : RuntimeNode
    {
        [SerializeField] public SubgraphType subgraphType;
        [SerializeField] public RuntimeGraph subgraph;
        
        private readonly Dictionary<string, object> inputValues = new();
        private readonly Dictionary<string, object> outputValues = new();
        
        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            // Get the Inputs of the Subgraph node
            inputValues.Clear();
            foreach (var port in inputPorts)
            {
                if (!port.TryGetNodeInput(context, out object inputValue)) return false;
                inputValues[port.Name] = inputValue;
            }
            
            context.EnterGraph(this);
            {
                // Assign the inputs within the subgraph context
                foreach (var entry in inputValues)
                {
                    context.SetInput(entry.Key, entry.Value);
                }
                
                // Get all the outputs of the Subgraph
                outputValues.Clear();
                foreach (var port in outputPorts)
                {
                    var variable = subgraph.GetVariables().FirstOrDefault(variable => variable.ID.ToString() == port.Name);
                    if (variable == null) return false;
                    if (!subgraph.TryGetGraphOutput(context, variable.Name, out object outputValue)) return false;
                    outputValues[port.Name] = outputValue;
                }
            }
            context.ExitGraph();

            // Assign the outputs to the ports of the node
            foreach (var port in outputPorts)
            {
                if (!port.TrySetNodeOutput(context, outputValues[port.Name])) return false;
            }
            
            return true;
        }
    }
}