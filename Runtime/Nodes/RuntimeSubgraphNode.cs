using System.Collections.Generic;
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

        private Dictionary<string, object> inputTemp = new();
        
        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            // Get inputs to node
            inputTemp.Clear();
            inputPorts.ForEach(port =>
            {
                port.TryGetNodeInput(context, out object input);
                inputTemp.Add(port.Name, input);
            });
            
            context.EnterGraph(this);

            foreach (var input in inputTemp)
            {
                context.TrySetInput(input.Key, input.Value);
            } 

            outputPorts.ForEach(port =>
            {
                subgraph.TryGetGraphOutput(context, port.Name, out object output);
                port.TrySetNodeOutput(context, output);
            });
            
            context.ExitGraph();
            return true;
        }
    }
}