using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes
{
    public class ConcatenateNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference lhsPort;
        [SerializeField] public RuntimePortReference rhsPort;
        [SerializeField] public RuntimePortReference outputPort;

        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            if (!lhsPort.TryGetNodeInput(context, out string lhs)) return false;
            if (!rhsPort.TryGetNodeInput(context, out string rhs)) return false;

            var output = lhs + rhs;
            return outputPort.TrySetNodeOutput(context, output);
        }
    }
}