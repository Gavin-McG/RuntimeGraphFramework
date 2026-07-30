using System;
using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes
{
    public class ComparisonNode : RuntimeNode
    {
        public enum Operation
        {
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual,
        }
        
        [SerializeField] public Operation operation;
        [SerializeField] public RuntimePortReference inputPort1;
        [SerializeField] public RuntimePortReference inputPort2;
        [SerializeField] public RuntimePortReference outputPort;

        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            if (!inputPort1.TryGetNodeInput<float>(context, out var input1)) return false;
            if (!inputPort2.TryGetNodeInput<float>(context, out var input2)) return false;

            var result = operation switch
            {
                Operation.Equal => input1 == input2,
                Operation.NotEqual => input1 != input2,
                Operation.LessThan => input1 < input2,
                Operation.LessThanOrEqual => input1 <= input2,
                Operation.GreaterThan => input1 > input2,
                Operation.GreaterThanOrEqual => input1 >= input2,
                _ => throw new NotSupportedException("Unknown operation type")
            };
            
            return outputPort.TrySetNodeOutput(context, result);
        }
    }
}