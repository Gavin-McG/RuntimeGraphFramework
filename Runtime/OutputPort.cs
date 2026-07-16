using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class OutputPort
    {
        public abstract RuntimeNode Node { get; }
        public abstract Type DataType { get; }

        public abstract void SetValue<U>(U value);
        public abstract U GetValue<U>(IQueryContext context);
    }
    
    [Serializable]
    public class OutputPort<T> : OutputPort
    {
        [SerializeField] public RuntimeNode node;
        
        private T currentValue;
        
        public OutputPort(RuntimeNode node)
        {
            this.node = node;
        }

        public override RuntimeNode Node => node;
        public override Type DataType => typeof(T);

        public override void SetValue<U>(U value)
        {
            if (!typeof(U).IsAssignableFrom(typeof(T))) throw new ArgumentException("Incorrect OutputPort Type");
            currentValue = (T)(object)value;
        }

        public override U GetValue<U>(IQueryContext context)
        {
            if (!typeof(U).IsAssignableFrom(typeof(T))) throw new ArgumentException("Incorrect OutputPort Type");
            node.UpdateNode(context);
            return (U)(object)currentValue;
        }
    }
}