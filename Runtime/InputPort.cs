using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum InputPortKind
    {
        Constant,
        Variable,
        Connected,
        Untyped
    }
    
    [Serializable]
    public abstract class InputPort : RuntimePort
    {
        public abstract OutputPortReference PortReference { get; }
        public abstract InputPortKind PortKind { get; }
        
        protected InputPort(string name, Hash128 id, RuntimeNode node) : base(name, id, node) {}
        
        public abstract TInput GetValue<TInput>(IQueryContext context);
    }

    [Serializable]
    public class ConstantInputPort<TOutput, TGraph> : InputPort
    {
        [SerializeField] private TOutput _value;
        
        public override Type DataType => typeof(TOutput);
        public override OutputPortReference PortReference => null;
        public override InputPortKind PortKind => InputPortKind.Constant;
        
        public ConstantInputPort(string name, Hash128 id, RuntimeNode node, TOutput value) : base(name, id, node)
        {
            _value = value;
        }

        public override TInput GetValue<TInput>(IQueryContext context)
        {
            return PortTypeCastManager.GetCastedValue<TOutput, TInput, TGraph>(_value);
        }
    }

    [Serializable]
    public class VariableInputPort<TOutput, TGraph> : InputPort
    {
        [SerializeField] private string _variableName;
        
        public override Type DataType => typeof(TOutput);
        public override OutputPortReference PortReference => null;
        public override InputPortKind PortKind => InputPortKind.Variable;

        public VariableInputPort(string name, Hash128 id, RuntimeNode node, string variableName) : base(name, id, node)
        {
            _variableName = variableName;
        }

        public override TInput GetValue<TInput>(IQueryContext context)
        {
            if (context.TryGetVariable<TOutput>(_variableName, out var value))
            {
                return PortTypeCastManager.GetCastedValue<TOutput, TInput, TGraph>(value);
            }
            return default;
        }
    }

    [Serializable]
    public class ConnectedInputPort<TOutput, TGraph> : InputPort
    {
        [SerializeField] private OutputPortReference _connectedPort;
        
        public override Type DataType => typeof(TOutput);
        public override OutputPortReference PortReference => _connectedPort;
        public override InputPortKind PortKind => InputPortKind.Connected;

        public ConnectedInputPort(string name, Hash128 id, RuntimeNode node, OutputPortReference connectedPort) : base(name, id, node)
        {
            _connectedPort = connectedPort;
        }

        public override TInput GetValue<TInput>(IQueryContext context)
        {
            var value = PortReference.GetOutputPort().GetValue<TOutput>(context);
            return PortTypeCastManager.GetCastedValue<TOutput, TInput, TGraph>(value);
        }
    }

    [Serializable]
    public class UntypedInputPort : InputPort
    {
        public override Type DataType => null;
        public override OutputPortReference PortReference => null;
        public override InputPortKind PortKind => InputPortKind.Untyped;
        
        public UntypedInputPort(string name, Hash128 id, RuntimeNode node) : base(name, id, node) {}

        public override TInput GetValue<TInput>(IQueryContext context)
        {
            throw new InvalidCastException($"Value should not be retrieved from an UnTyped Input Port");
        }
    }
}