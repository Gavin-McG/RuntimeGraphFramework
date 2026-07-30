using System;
using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes
{
    public class MathNode : RuntimeNode
    {
        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Pow,
        }
        
        [SerializeField] public Operation operation;
        [SerializeField] public RuntimePortReference inputPort1;
        [SerializeField] public RuntimePortReference inputPort2;
        [SerializeField] public RuntimePortReference outputPort;

        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            if (!inputPort1.TryGetNodeInput(context, out float input1)) return false;
            if (!inputPort2.TryGetNodeInput(context, out float input2)) return false;

            var result = operation switch
            {
                Operation.Add => input1 + input2,
                Operation.Subtract => input1 - input2,
                Operation.Multiply => input1 * input2,
                Operation.Divide => input1 / input2,
                Operation.Pow => Mathf.Pow(input1, input2),
                _ => throw new NotSupportedException("Unknown operation type")
            };
            
            return outputPort.TrySetNodeOutput(context, result);
        }
    }
}