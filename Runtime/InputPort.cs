using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum InputPortSource
    {
        Constant,
        Variable,
        PortReference,
    }

    [Serializable]
    public abstract class InputPort
    {
        [SerializeField] protected Hash128 portID;
        [SerializeField] protected InputPortSource portSource;
        
        public Hash128 PortID => portID;
        public InputPortSource PortSource => portSource;
        public abstract OutputPortReference PortReference { get; }
        public abstract Type DataType { get; }
        
        public abstract TInput GetValue<TInput>(IQueryContext context);
    }
    
    [Serializable]
    public class InputPort<TOutput, TGraph> : InputPort
    {
        [SerializeField] private TOutput constantValue;
        [SerializeField] protected string variableName;
        [SerializeField] private OutputPortReference portReference;
        
        public override OutputPortReference PortReference => portReference;
        public override Type DataType => typeof(TOutput);
        
        public InputPort(TOutput constantValue, Hash128 portID)
        {
            this.portID = portID;
            portSource = InputPortSource.Constant;
            this.constantValue = constantValue;
        }

        public InputPort(Hash128 portID, string variableName)
        {
            this.portID = portID;
            portSource = InputPortSource.Variable;
            this.variableName = variableName;
        }

        public InputPort(Hash128 portID, OutputPortReference portReference)
        {
            this.portID = portID;
            portSource = InputPortSource.PortReference;
            this.portReference = portReference;
        }

        private TOutput GetVariable(IVariableContext variables)
        {
            if (variables.TryGetVariable<TOutput>(variableName, out var value)) return value;
            return constantValue;
        }
        
        private OutputPort GetConnectedPort()
        {
            return portReference.GetOutputPort();
        }
        
        public override TInput GetValue<TInput>(IQueryContext context)
        {
            var value = portSource switch
            {
                InputPortSource.Constant => constantValue,
                InputPortSource.Variable => GetVariable(context),
                InputPortSource.PortReference => GetConnectedPort().GetValue<TOutput>(context),
                _ => default,
            };

            return PortTypeCastManager.GetCastedValue<TOutput, TInput, TGraph>(value);
        }
    }
}