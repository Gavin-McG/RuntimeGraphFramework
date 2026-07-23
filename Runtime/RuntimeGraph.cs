using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeGraph : ScriptableObject, IRuntimeGraph
    {
        [SerializeField] public Hash128 graphID;
        
        [SerializeField] public List<RuntimeNode> nodes = new List<RuntimeNode>();
        [SerializeField] public List<RuntimeVariable> variables = new();
        
        [SerializeField] public bool valid = true;
        [SerializeField] public string importMessage;
        
        public string Name => name;
        public bool IsValid => valid;

        public int NodeCount => nodes.Count;
        public IEnumerable<IRuntimeNode> GetNodes() => nodes;
        public IRuntimeNode GetNode(int index) => nodes[index];
        
        public int VariableCount => variables.Count;
        public IEnumerable<IRuntimeVariable> GetVariables() => variables;
        public IRuntimeVariable GetVariable(int index) => variables[index];
    }
}