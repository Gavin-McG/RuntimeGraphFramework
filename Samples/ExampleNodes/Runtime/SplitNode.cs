using System;
using UnityEngine;

namespace RuntimeGraphFramework.ExampleNodes
{
    public class SplitNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference inputPort;
        
        [SerializeField] public RuntimePortReference outputPort_x;
        [SerializeField] public RuntimePortReference outputPort_y;
        [SerializeField] public RuntimePortReference outputPort_z;
        [SerializeField] public RuntimePortReference outputPort_w;

        protected override bool TryUpdateOutputs(IQueryContext context)
        {
            Type vectorType = inputPort.DataType;

            if (vectorType == typeof(Vector2))
            {
                if (!inputPort.TryGetNodeInput(context, out Vector2 vector)) return false;
                return outputPort_x.TrySetNodeOutput(context, vector.x) &&
                       outputPort_y.TrySetNodeOutput(context, vector.y);
            }

            if (vectorType == typeof(Vector3))
            {
                if (!inputPort.TryGetNodeInput(context, out Vector3 vector)) return false;
                return outputPort_x.TrySetNodeOutput(context, vector.x) &&
                       outputPort_y.TrySetNodeOutput(context, vector.y) &&
                       outputPort_z.TrySetNodeOutput(context, vector.z);
            }

            if (vectorType == typeof(Vector4))
            {
                if (!inputPort.TryGetNodeInput(context, out Vector4 vector)) return false;
                return outputPort_x.TrySetNodeOutput(context, vector.x) &&
                       outputPort_y.TrySetNodeOutput(context, vector.y) &&
                       outputPort_z.TrySetNodeOutput(context, vector.z) &&
                       outputPort_w.TrySetNodeOutput(context, vector.w);
            }
            
            throw new NotSupportedException("Invalid input vector type");
        }
    }
}