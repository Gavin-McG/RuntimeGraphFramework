using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    internal class UntypedRuntimePort : RuntimePort
    {
        public UntypedRuntimePort(string name, int index, Hash128 id, RuntimePortDirection direction, RuntimeNode node)
            : base(name, index, id, direction, node) {}
        
        public override bool TryGetValue<T>(out T value)
        {
            value = default;
            return false;
        }
        
        public override bool TryGetNodeInput<T>(IQueryContext context, out T value)
        {
            value = default;
            return false;
        }
        
        internal override bool TrySetValue<T>(T value)
        {
            return false;
        }

        public override bool TrySetNodeOutput<T>(IQueryContext context, T value)
        {
            return false;
        }
    }
}