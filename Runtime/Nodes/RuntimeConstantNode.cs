using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeConstantNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference _outputPort;
        [SerializeReference] public ValueWrapper _valueWrapper;
        
        protected override void UpdateNodeOutputs(IQueryContext context)
        {
            _valueWrapper.TryGetValue(out object value);
            _outputPort.TrySetValue(context, value);
        }
    }
}