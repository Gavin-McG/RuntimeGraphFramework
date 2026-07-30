using System;
using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes
{
    public class LogicNode : RuntimeNode
    {
        public enum Operation
        {
            Not,
            And,
            Or,
            Nand,
            Nor,
            Xor,
            Xnor
        }
        
        [SerializeField] public Operation operation;
        [SerializeField] public RuntimePortReference inputPort1;
        [SerializeField] public RuntimePortReference inputPort2;
        [SerializeField] public RuntimePortReference outputPort;
        
        public static bool HasSecondInput(Operation op) => op != Operation.Not;

        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            
            if (!inputPort1.TryGetNodeInput(context, out bool input1)) return false;
            
            bool input2 = false;
            if (HasSecondInput(operation) && !inputPort2.TryGetNodeInput(context, out input2)) return false;

            var result = operation switch
            {
                Operation.Not => !input1,
                Operation.And => input1 && input2,
                Operation.Or => input1 || input2,
                Operation.Nand => !(input1 && input2),
                Operation.Nor => !(input1 || input2),
                Operation.Xor => input1 == input2,
                Operation.Xnor => input1 != input2,
               _ => throw new NotSupportedException("Unknown operation type"),
            };
            
            return outputPort.TrySetNodeOutput(context, result);
        }
    }
}