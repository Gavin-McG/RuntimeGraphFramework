using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public enum OutputPortKind
    {
        Typed,
        Untyped,
    }
    
    [Serializable]
    public abstract class OutputPort : RuntimePort
    {
        public abstract OutputPortKind PortKind { get; }
        
        public abstract void SetValue<U>(U value);
        public abstract U GetValue<U>(IQueryContext context);

        protected OutputPort(string name, Hash128 id, RuntimeNode node) : base(name, id, node) {}
    }
    
    [Serializable]
    public class TypedOutputPort<T> : OutputPort
    {
        private T currentValue;
        
        public TypedOutputPort(string name, Hash128 id, RuntimeNode node) : base(name, id, node) {}

        public override Type DataType => typeof(T);
        public override OutputPortKind PortKind => OutputPortKind.Typed;

        public override void SetValue<U>(U value)
        {
            if (!typeof(U).IsAssignableFrom(typeof(T))) throw new InvalidCastException("Incorrect OutputPort Type");
            currentValue = (T)(object)value;
        }

        public override U GetValue<U>(IQueryContext context)
        {
            if (!typeof(U).IsAssignableFrom(typeof(T))) throw new InvalidCastException("Incorrect OutputPort Type");
            Node.UpdateNode(context);
            return (U)(object)currentValue;
        }
    }

    [Serializable]
    public class UntypedOutputPort : OutputPort
    {
        public UntypedOutputPort(string name, Hash128 id, RuntimeNode node) : base(name, id, node) {}
        
        public override Type DataType => null;
        public override OutputPortKind PortKind => OutputPortKind.Untyped;
        
        public override void SetValue<U>(U value)
        {
            throw new InvalidCastException($"Value should set on an UnTyped Output Port");
        }

        public override U GetValue<U>(IQueryContext context)
        {
            throw new InvalidCastException($"Value should set on an UnTyped Output Port");
        }
    }
}