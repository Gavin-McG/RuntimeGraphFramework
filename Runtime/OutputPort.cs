using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class OutputPort : RuntimePort
    {
        public abstract void SetValue<U>(U value);
        public abstract U GetValue<U>(IQueryContext context);

        protected OutputPort(string name, Hash128 id, RuntimeNode node) : base(name, id, node) {}
    }
    
    [Serializable]
    public class OutputPort<T> : OutputPort
    {
        private T currentValue;
        
        public OutputPort(string name, Hash128 id, RuntimeNode node) : base(name, id, node) {}

        public override Type DataType => typeof(T);

        public override void SetValue<U>(U value)
        {
            if (!typeof(U).IsAssignableFrom(typeof(T))) throw new ArgumentException("Incorrect OutputPort Type");
            currentValue = (T)(object)value;
        }

        public override U GetValue<U>(IQueryContext context)
        {
            if (!typeof(U).IsAssignableFrom(typeof(T))) throw new ArgumentException("Incorrect OutputPort Type");
            Node.UpdateNode(context);
            return (U)(object)currentValue;
        }
    }
}