using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum InputPortType
    {
        Constant,
        Parameter,
        PortReference,
    }

    [Serializable]
    public abstract class InputPort
    {
        [SerializeField] protected Hash128 portID;
        [SerializeField] protected InputPortType portType;
        
        public Hash128 PortID => portID;
        public InputPortType PortType => portType;
        public abstract OutputPortReference PortReference { get; }
        public abstract Type DataType { get; }
        
        public abstract TInput GetValue<TInput>(IDialogueContext context);
    }
    
    [Serializable]
    public class InputPort<TOutput, TGraph> : InputPort
    {
        [SerializeField] private TOutput constantValue;
        [SerializeField] protected string parameterName;
        [SerializeField] private OutputPortReference portReference;
        
        public override OutputPortReference PortReference => portReference;
        public override Type DataType => typeof(TOutput);
        
        public InputPort(TOutput constantValue, Hash128 portID)
        {
            this.portID = portID;
            portType = InputPortType.Constant;
            this.constantValue = constantValue;
        }

        public InputPort(Hash128 portID, string parameterName)
        {
            this.portID = portID;
            portType = InputPortType.Parameter;
            this.parameterName = parameterName;
        }

        public InputPort(Hash128 portID, OutputPortReference portReference)
        {
            this.portID = portID;
            portType = InputPortType.PortReference;
            this.portReference = portReference;
        }

        private TOutput GetParameter(IParameterContext parameters)
        {
            if (parameters.TryGetValue<TOutput>(parameterName, out var value)) return value;
            return constantValue;
        }
        
        private OutputPort GetConnectedPort()
        {
            return portReference.GetOutputPort();
        }
        
        public override TInput GetValue<TInput>(IDialogueContext context)
        {
            var value = portType switch
            {
                InputPortType.Constant => constantValue,
                InputPortType.Parameter => GetParameter(context),
                InputPortType.PortReference => GetConnectedPort().GetValue<TOutput>(context),
                _ => default,
            };

            return PortTypeCastManager.GetCastedValue<TOutput, TInput, TGraph>(value);
        }
    }
}