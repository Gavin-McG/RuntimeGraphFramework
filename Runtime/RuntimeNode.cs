using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public abstract class RuntimeNode : ScriptableObject, IRuntimeNode
    {
        [SerializeField] public RuntimeGraph graph;
        [SerializeField] public Hash128 nodeID;
        [SerializeReference] public List<RuntimePort> inputPorts = new();
        [SerializeReference] public List<RuntimePort> outputPorts = new();
        
        public RuntimeGraph Graph => graph;
        public Hash128 ID => nodeID;
        public int InputPortCount => inputPorts.Count;
        public int OutputPortCount => outputPorts.Count;
        
        public IRuntimePort GetOutputPort(int index) => outputPorts.ElementAtOrDefault(index);
        public IRuntimePort GetInputPort(int index) => inputPorts.ElementAtOrDefault(index);
        
        protected virtual void UpdateNodeOutputs(IQueryContext context) {}
        
        private Hash128 previousQueryID;
        
        public void UpdateNode(IQueryContext context)
        {
            var currentQueryID = context.QueryID;
            if (previousQueryID == currentQueryID) return;
            previousQueryID = currentQueryID;
            
            UpdateNodeOutputs(context);
        }
    }
}