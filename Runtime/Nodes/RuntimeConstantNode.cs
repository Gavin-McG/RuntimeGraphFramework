using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeConstantNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference _outputPort;
        [SerializeReference] public ValueWrapper _valueWrapper;
        
        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            if (!_valueWrapper.TryGetValue(out object value)) return false;
            return _outputPort.TrySetNodeOutput(context, value);
        }
    }
}