using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeGraphFramework
{
    [Serializable]
    public class Parameter
    {
        [Serializable]
        private abstract class ValueWrapper
        {
            public abstract Type DataType { get; }
            public abstract T GetValue<T>();
            public abstract void SetValue<T>(T value);
        }

        [Serializable]
        private class ValueWrapper<U> : ValueWrapper
        {
            [SerializeField] private U _value;
            
            public override Type DataType => typeof(U);
            
            public ValueWrapper(U value)
            {
                _value = value;
            }
            
            public override T GetValue<T>()
            {
                if (typeof(T) == typeof(U)) return (T)(object)_value;
                throw new ArgumentException($"Type {typeof(T)} did not match parameter type {typeof(T)}");
            }

            public override void SetValue<T>(T value)
            {
                if (typeof(T) == typeof(U)) _value = (U)(object)value;
                else throw new ArgumentException($"Type {typeof(T)} did not match parameter type {typeof(T)}");
            }
        }
        
        [SerializeReference] private ValueWrapper valueWrapper;

        private Parameter(ValueWrapper valueWrapper)
        {
            this.valueWrapper = valueWrapper;
        }

        public static Parameter CreateParameter<T>(T value)
        {
            return new Parameter(new ValueWrapper<T>(value));
        }
        
        public Type DataType => valueWrapper.DataType;

        public T GetValue<T>()
        {
            if (typeof(T) == valueWrapper.DataType) return valueWrapper.GetValue<T>();
            throw new ArgumentException();
        }

        public void SetValue<T>(T value)
        {
            if (typeof(T) == valueWrapper.DataType) valueWrapper.SetValue(value);
            else throw new ArgumentException();
        }
    }
}