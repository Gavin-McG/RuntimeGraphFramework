using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class VariableRuntimePort : RuntimePort
    {
        protected VariableRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
    }
    
    
    [Serializable]
    public class VariableRuntimePort<TVariable, TGraph> : VariableRuntimePort
    {
        [SerializeField] private string _variableName;
        
        public override Type DataType => typeof(TVariable);
        public override bool IsConnected => false;
        public override IRuntimePort FirstConnectedPort => null;

        public VariableRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node, string variableName) 
            : base(name, index, id, direction, node)
        {
            _variableName = variableName;
        }

        public override bool TryGetValue<T>(IQueryContext context, out T value)
        {
            if (!context.TryGetVariable(_variableName, out TVariable variableValue))
            {
                value = default;
                return false;
            }
            
            return PortTypeCastManager.TryCastValue<TVariable, T, TGraph>(variableValue, out value);
        }

        public override bool TrySetValue<T>(IQueryContext context, T value)
        {
            return context.TrySetVariable(_variableName, value);
        }
    }
}