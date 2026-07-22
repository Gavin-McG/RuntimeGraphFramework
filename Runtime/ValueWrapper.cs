using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class ValueWrapper
    {
        public abstract Type DataType { get; }
        public abstract bool TryGetValue<T>(out T value);
        public abstract bool TrySetValue<T>(T value);
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
            
        public override bool TryGetValue<T>(out T value)
        {
            if (typeof(T).IsAssignableFrom(typeof(U)))
            {
                value = (T)(object)_value;
                return true;
            }
            
            value = default;
            return false;
        }

        public override bool TrySetValue<T>(T value)
        {
            if (typeof(U).IsAssignableFrom(typeof(T)))
            {
                _value = (U)(object)value;
                return true;
            }
            
            return false;
        }
    }
}