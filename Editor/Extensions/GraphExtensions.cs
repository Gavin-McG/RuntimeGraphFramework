using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public static class GraphExtensions
    {
        public static List<Graph> GetAllSubgraphs(this Graph graph)
        {
            List<Graph> subgraphs = new List<Graph>();
            void GetAllSubgraphsHelper(Graph graph, HashSet<Hash128> graphIDs)
            {
                var subgraphNodes = graph.GetNodes().OfType<ISubgraphNode>();
                foreach (var subgraphNode in subgraphNodes)
                {
                    var subgraph = subgraphNode.GetSubgraph();
                    if (graphIDs.Add(subgraph.ID))
                    {
                        subgraphs.Add(subgraph);
                        GetAllSubgraphsHelper(subgraph, graphIDs);
                    }
                }
            }

            HashSet<Hash128> graphIDs = new();
            GetAllSubgraphsHelper(graph, graphIDs);
            return subgraphs;
        }
        
        public static IVariable GetOutputVariableOfPort(this ISubgraphNode subgraphNode, IPort outputPort)
        {
            if (outputPort == null) return null;
            if (outputPort.Direction != PortDirection.Output) return null;
            
            Graph subGraph = subgraphNode.GetSubgraph();
            var outputVariable = subGraph.GetVariables().FirstOrDefault(variable =>
                variable.VariableKind == VariableKind.Output &&
                variable.Name == outputPort.DisplayName);
            return outputVariable;
        }
        
        public static IVariable GetInputVariableOfPort(this ISubgraphNode subgraphNode, IPort inputPort)
        {
            if (inputPort == null) return null;
            if (inputPort.Direction != PortDirection.Input) return null;
            
            Graph subGraph = subgraphNode.GetSubgraph();
            var inputVariable = subGraph.GetVariables().FirstOrDefault(variable =>
                variable.VariableKind == VariableKind.Input &&
                variable.Name == inputPort.DisplayName);
            return inputVariable;
        }
        
        public static IPort GetOutputPortOfVariable(this ISubgraphNode subgraphNode, IVariable variable)
        {
            if (variable == null) return null;
            if (variable.VariableKind != VariableKind.Output) return null;
            
            var outputPort = subgraphNode.GetOutputPorts().FirstOrDefault(port => 
                port.DisplayName == variable.Name);
            return outputPort;
        }

        public static IPort GetInputPortOfVariable(this ISubgraphNode subgraphNode, IVariable variable)
        {
            if (variable == null) return null;
            if (variable.VariableKind != VariableKind.Input) return null;
            
            var inputPort = subgraphNode.GetInputPorts().FirstOrDefault(port => 
                port.DisplayName == variable.Name);
            return inputPort;
        }
    }
}