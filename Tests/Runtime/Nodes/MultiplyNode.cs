using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class MultiplyNode : RuntimeNode
    {
        [SerializeField] public InputPortReference input1;
        [SerializeField] public InputPortReference input2;
        [SerializeField] public OutputPortReference output;

        protected override void UpdateNodeOutputs(IQueryContext context)
        {
            var value1 = input1.GetValue<float>(context);
            var value2 = input2.GetValue<float>(context);
            
            var result = value1 * value2;
            output.SetValue(result);
        }
    }
}