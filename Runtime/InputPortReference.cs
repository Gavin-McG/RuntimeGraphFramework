using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public class InputPortReference
    {
        [SerializeField] private RuntimeNode node;
        [SerializeField] private int inputPortIndex;

        public InputPortReference()
        {
            node = null;
            inputPortIndex = -1;
        }

        public InputPortReference(RuntimeNode node, int inputPortIndex)
        {
            this.node = node;
            this.inputPortIndex = inputPortIndex;
        }
        
        public InputPort GetInputPort()
        {
            return node?.inputPorts[inputPortIndex];
        }
        
        public RuntimeNode Node => node;
        public Type DataType => GetInputPort().DataType;
        
        public TInput GetValue<TInput>(IQueryContext context) => GetInputPort().GetValue<TInput>(context);
    }
}