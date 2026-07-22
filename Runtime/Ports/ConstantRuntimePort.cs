using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class ConstantRuntimePort : RuntimePort
    {
        protected ConstantRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
    }
    
    [Serializable]
    public class ConstantRuntimePort<TConstant, TGraph> : ConstantRuntimePort
    {
        [SerializeField] private TConstant _value;
        
        public override Type DataType => typeof(TConstant);
        public override bool IsConnected => false;
        public override IRuntimePort FirstConnectedPort => null;

        public ConstantRuntimePort(
            string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node, TConstant value) 
            : base(name, index, id, direction, node)
        {
            _value = value;
        }

        public override bool TryGetValue<T>(IQueryContext context, out T value)
        {
            return PortTypeCastManager.TryCastValue<TConstant, T, TGraph>(_value, out value);
        }

        public override bool TrySetValue<T>(IQueryContext context, T value)
        {
            return false;
        }
    }
}