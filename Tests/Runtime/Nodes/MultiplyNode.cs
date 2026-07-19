using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class MultiplyNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference input1;
        [SerializeField] public RuntimePortReference input2;
        [SerializeField] public RuntimePortReference output;

        protected override void UpdateNodeOutputs(IQueryContext context)
        {
            input1.TryGetValue<float>(context, out var value1);
            input2.TryGetValue<float>(context, out var value2);
            
            var result = value1 * value2;
            output.TrySetValue(context, result);
        }
    }
}