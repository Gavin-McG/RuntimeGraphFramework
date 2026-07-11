using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public class RuntimeVariable
    {
        [SerializeReference] private ValueWrapper valueWrapper;

        private RuntimeVariable(ValueWrapper valueWrapper)
        {
            this.valueWrapper = valueWrapper;
        }

        public static RuntimeVariable CreateVariable<T>(T value)
        {
            return new RuntimeVariable(new ValueWrapper<T>(value));
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