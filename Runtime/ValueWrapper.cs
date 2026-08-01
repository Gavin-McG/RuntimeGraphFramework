using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    internal abstract class ValueWrapper
    {
        public abstract Type DataType { get; }
        public abstract bool TryGetValue<T>(out T value);
        public abstract bool TrySetValue<T>(T value);

        public static ValueWrapper CreateWrapper(object value)
        {
            if (value == null) return null;
            var wrapperType = typeof(ValueWrapper<>).MakeGenericType(value.GetType());
            var wrapper = (ValueWrapper)Activator.CreateInstance(wrapperType, value);
            return wrapper;
        }
    }

    [Serializable]
    internal class ValueWrapper<TValue> : ValueWrapper
    {
        [SerializeField] private TValue _value;
            
        public override Type DataType => typeof(TValue);
            
        public ValueWrapper(TValue value)
        {
            _value = value;
        }
            
        public override bool TryGetValue<T>(out T value)
        {
            if (typeof(T).IsAssignableFrom(typeof(TValue)))
            {
                value = (T)(object)_value;
                return true;
            }
            
            value = default;
            return false;
        }

        public override bool TrySetValue<T>(T value)
        {
            if (typeof(TValue).IsAssignableFrom(typeof(T)))
            {
                _value = (TValue)(object)value;
                return true;
            }
            
            return false;
        }
    }
}