using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    internal class QueryStackEntry
    {
        private readonly RuntimeSubgraphNode _node;
        private readonly Dictionary<string, object> _inputs = new();
        
        public RuntimeSubgraphNode Node => _node;
        public RuntimeGraph Graph => _node.Graph;
        public Hash128 QueryID { get; set; }

        public QueryStackEntry(RuntimeSubgraphNode node, Hash128 queryID)
        {
            _node = node;
            QueryID = queryID;
        }

        public bool TrySetInput(string inputName, object value)
        {
            _inputs[inputName] = value;
            return true;
        }
        
        public bool TryGetInput(string inputName, out object value)
        {
            return _inputs.TryGetValue(inputName, out value);
        }
    }
    
    public class QueryContext : IQueryContext
    {
        private static readonly System.Random rng = new();
        
        private RuntimeGraph _mainGraph;
        private Hash128 _mainQueryID;
        private Stack<QueryStackEntry> stack = new();
        private Dictionary<string, object> variables = new();

        public RuntimeGraph MainGraph => _mainGraph;
        public RuntimeGraph CurrentGraph => stack.Count>0 ? stack.Peek().Graph : _mainGraph;
        public Hash128 QueryID => stack.Count>0 ? stack.Peek().QueryID : _mainQueryID;

        public QueryContext(RuntimeGraph mainGraph)
        {
            _mainGraph = mainGraph;
            _mainQueryID = GenerateQueryID();
        }

        private Hash128 GenerateQueryID()
        {
            return new Hash128(
                (uint)rng.Next(),
                (uint)rng.Next(),
                (uint)rng.Next(),
                (uint)rng.Next()
            );
        }

        public void RefreshQueryID()
        {
            _mainQueryID = GenerateQueryID();
            foreach (var entry in stack)
            {
                entry.QueryID = GenerateQueryID();
            }
        }
        
        public void EnterGraph(RuntimeSubgraphNode subgraphNode)
        {
            stack.Push(new QueryStackEntry(subgraphNode, GenerateQueryID()));
        }

        public void ExitGraph()
        {
            stack.Pop();
        }
        
        public bool TryGetVariable<T>(string variableName, out T value)
        {
            RefreshQueryID();
            
            value = default;

            if (!variables.TryGetValue(variableName, out object variableValue)) return false;
            if (!typeof(T).IsAssignableFrom(variableValue.GetType())) return false;
            
            value = (T)variableValue;
            return true;
        }
        
        public bool TrySetVariable<T>(string variableName, T value)
        {
            variables[variableName] = value;
            return true;
        }

        public bool TryGetInput<T>(string inputName, out T value)
        {
            value = default;
            if (stack.Count == 0) return false;
            
            if (!stack.Peek().TryGetInput(inputName, out var inputValue)) return false;
            if (!typeof(T).IsAssignableFrom(inputValue.GetType())) return false;
            
            value = (T)inputValue;
            return true;
        }

        public bool TrySetInput<T>(string inputName, T value)
        {
            RefreshQueryID();

            if (stack.Count == 0) return false;
            return stack.Peek().TrySetInput(inputName, value);
        }
    }
}