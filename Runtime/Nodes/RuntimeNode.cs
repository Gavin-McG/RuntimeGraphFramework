using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public abstract class RuntimeNode : ScriptableObject, IRuntimeNode
    {
        [SerializeField] internal RuntimeGraph graph;
        [SerializeField] internal Hash128 nodeID;
        [SerializeReference] internal List<RuntimePort> inputPorts = new();
        [SerializeReference] internal List<RuntimePort> outputPorts = new();
        
        public RuntimeGraph Graph => graph;
        public Hash128 ID => nodeID;
        public int InputPortCount => inputPorts.Count;
        public int OutputPortCount => outputPorts.Count;
        
        public IRuntimePort GetOutputPort(int index) => outputPorts.ElementAtOrDefault(index);
        public IRuntimePort GetInputPort(int index) => inputPorts.ElementAtOrDefault(index);

        protected virtual bool TryUpdateOutputs(IQueryContext context) => false;
        
        private Hash128 previousQueryID;
        private bool previousReturn;
        
        public bool TryUpdateNode(IQueryContext context)
        {
            var currentQueryID = context.QueryID;
            if (previousQueryID == currentQueryID) return previousReturn;
            
            previousQueryID = currentQueryID;
            previousReturn = TryUpdateOutputs(context);
            return previousReturn;
        }
    }
}