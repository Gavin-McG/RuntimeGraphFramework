using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public static class GraphExtensions
    {
        public static IEnumerable<T> GetNodes<T>(this Graph graph, bool includeSubgraph = true) where T : class
        {
            IEnumerable<T> GetNodesHelper(INode node)
            {
                // Check Node
                if (node is T tNode)
                {
                    yield return tNode;
                }
                
                // Check BlockNodes
                if (node is ContextNode contextNode)
                {
                    var blockNodes = contextNode.BlockNodes;
                    foreach (var blockNode in blockNodes)
                    {
                        if (blockNode is T tBlockNode) yield return tBlockNode;
                    }
                }
                
                // Check SubGraph
                if (includeSubgraph && node is ISubgraphNode subgraphNode)
                {
                    var subgraph = subgraphNode.GetSubgraph();
                    var subNodes = subgraph.GetNodes<T>();
                    foreach (var subNode in subNodes)
                    {
                        yield return subNode;
                    }
                }
            }
            
            return graph.GetNodes().SelectMany(GetNodesHelper).Distinct();
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