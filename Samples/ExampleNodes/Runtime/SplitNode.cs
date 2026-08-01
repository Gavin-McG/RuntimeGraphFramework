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
            if (!inputPort.TryGetNodeInput(context, out object input)) return false;
            Type vectorType = input.GetType();

            if (vectorType == typeof(Vector2))
            {
                var vector = (Vector2)input;
                return outputPort_x.TrySetNodeOutput(context, vector.x) &&
                       outputPort_y.TrySetNodeOutput(context, vector.y);
            }

            if (vectorType == typeof(Vector3))
            {
                var vector = (Vector3)input;
                return outputPort_x.TrySetNodeOutput(context, vector.x) &&
                       outputPort_y.TrySetNodeOutput(context, vector.y) &&
                       outputPort_z.TrySetNodeOutput(context, vector.z);
            }

            if (vectorType == typeof(Vector4))
            {
                var vector = (Vector4)input;
                return outputPort_x.TrySetNodeOutput(context, vector.x) &&
                       outputPort_y.TrySetNodeOutput(context, vector.y) &&
                       outputPort_z.TrySetNodeOutput(context, vector.z) &&
                       outputPort_w.TrySetNodeOutput(context, vector.w);
            }
            
            return false;
        }
    }
}