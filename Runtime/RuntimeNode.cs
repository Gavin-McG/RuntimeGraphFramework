using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public abstract class RuntimeNode : ScriptableObject
    {
        [SerializeField] public Hash128 nodeID;
        [SerializeReference] public List<InputPort> inputPorts = new();
        [SerializeReference] public List<OutputPort> outputPorts = new();
        
        private Hash128 previousQueryID;

        public OutputPort GetOutputPort(int index) => outputPorts[index];

        public virtual bool IsConstantNode()
        {
            return inputPorts.All(port => port.PortType == InputPortType.Constant);
        }
        
        public void QueryNode(IDialogueContext context)
        {
            var currentQueryID = context.QueryID;
            if (previousQueryID == currentQueryID) return;
            previousQueryID = currentQueryID;
            
            PopulateOutputs(context);
        }
        
        protected virtual void PopulateOutputs(IDialogueContext context) {}
    }
}