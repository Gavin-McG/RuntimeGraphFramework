using System;
using UnityEngine;

namespace BehavioralDialogue.Nodes
{
    [Serializable]
    public class OutputPortReference
    {
        [SerializeField] private RuntimeNode node;
        [SerializeField] private int outputPortIndex;

        public OutputPortReference(RuntimeNode node, int outputPortIndex)
        {
            this.node = node;
            this.outputPortIndex = outputPortIndex;
        }
        
        public OutputPort GetOutputPort()
        {
            return node.outputPorts[outputPortIndex];
        }
        
        public RuntimeNode Node => node;
        public Type DataType => GetOutputPort().DataType;
        
        public T GetValue<T>(IDialogueContext context) => GetOutputPort().GetValue<T>(context);
        public void SetValue<T>(T value) => GetOutputPort().SetValue(value);
    }
}