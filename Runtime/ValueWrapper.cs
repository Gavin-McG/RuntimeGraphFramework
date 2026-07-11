using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class ValueWrapper
    {
        public abstract Type DataType { get; }
        public abstract T GetValue<T>();
        public abstract void SetValue<T>(T value);
    }

    [Serializable]
    public class ValueWrapper<U> : ValueWrapper
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
}