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
        [SerializeReference] public List<InputPort> inputPorts = new();
        [SerializeReference] public List<OutputPort> outputPorts = new();
        
        private Hash128 previousQueryID;
        
        public RuntimeGraph Graph => graph;
        public Hash128 ID => nodeID;
        public int InputPortCount => inputPorts.Count;
        public int OutputPortCount => outputPorts.Count;
        
        public OutputPort GetOutputPort(int index) => outputPorts[index];
        public InputPort GetInputPort(int index) => inputPorts[index];

        public virtual bool IsConstantNode() => inputPorts.All(port => port.PortSource == InputPortSource.Constant);
        
        protected virtual void UpdateNodeOutputs(IQueryContext context) {}
        
        public void UpdateNode(IQueryContext context)
        {
            var currentQueryID = context.QueryID;
            if (previousQueryID == currentQueryID) return;
            previousQueryID = currentQueryID;
            
            UpdateNodeOutputs(context);
        }
    }
}