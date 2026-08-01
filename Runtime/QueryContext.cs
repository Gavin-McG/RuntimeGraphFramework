using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    internal class QueryStackEntry
    {
        private readonly RuntimeSubgraphNode _node;
        private readonly RuntimeGraph _graph;
        private readonly Dictionary<string, object> _inputs = new();
        private readonly Dictionary<Hash128, object> _portValues = new();
        
        public RuntimeSubgraphNode Node => _node;
        public RuntimeGraph Graph => _graph;

        public QueryStackEntry(RuntimeSubgraphNode node)
        {
            _node = node;
            _graph = node.Graph;
        }

        public QueryStackEntry(RuntimeGraph graph)
        {
            _node = null;
            _graph = graph;
        }

        public void SetInput(string inputName, object value)
        {
            _inputs[inputName] = value;
        }
        
        public bool TryGetInput(string inputName, out object value)
        {
            return _inputs.TryGetValue(inputName, out value);
        }

        public void ClearPortOutputs()
        {
            _portValues.Clear();
        }

        public void SetPortOutput(Hash128 portID, object value)
        {
            _portValues[portID] = value;
        }

        public bool TryGetPortOutput(Hash128 portID, out object value)
        {
            return _portValues.TryGetValue(portID, out value);
        }
    }
    
    public class QueryContext : IQueryContext
    {
        private RuntimeGraph _mainGraph;
        private Stack<QueryStackEntry> stack = new();
        private Dictionary<string, object> variables = new();

        public RuntimeGraph MainGraph => _mainGraph;
        public RuntimeGraph CurrentGraph => stack.Peek().Graph;

        public QueryContext(RuntimeGraph mainGraph)
        {
            _mainGraph = mainGraph;
            stack.Push(new QueryStackEntry(mainGraph));
        }
        
        public void EnterGraph(RuntimeSubgraphNode subgraphNode)
        {
            stack.Push(new QueryStackEntry(subgraphNode));
        }

        public void ExitGraph()
        {
            stack.Pop();
        }
        
        public void SetVariable(string variableName, object value)
        {
            ClearPortOutputs();
            variables[variableName] = value;
        }
        
        public bool TryGetVariable(string variableName, out object value)
        {
            return variables.TryGetValue(variableName, out value);
        }
        
        public void SetInput(string inputName, object value)
        {
            stack.Peek().SetInput(inputName, value);
        }

        public bool TryGetInput(string inputName, out object value)
        {
            return stack.Peek().TryGetInput(inputName, out value);
        }

        public void ClearPortOutputs()
        {
            foreach (var entry in stack)
            {
                entry.ClearPortOutputs();
            }
        }

        public void SetPortOutput(Hash128 portID, object value)
        {
            stack.Peek().SetPortOutput(portID, value);
        }

        public bool TryGetPortOutput(Hash128 portID, out object value)
        {
            return stack.Peek().TryGetPortOutput(portID, out value);
        }
    }
}