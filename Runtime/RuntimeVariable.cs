using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public class RuntimeVariable : IRuntimeVariable
    {
        [SerializeField] private string _name;
        [SerializeField] private RuntimeGraph _graph;
        [SerializeField] private Hash128 _id;
        [SerializeField] private RuntimeVariableKind _variableKind;
        [SerializeReference] private ValueWrapper _valueWrapper;

        public RuntimeVariable(string name, RuntimeGraph graph, Hash128 id, RuntimeVariableKind variableKind, object value)
        {
            _name = name;
            _graph = graph;
            _id = id;
            _variableKind = variableKind;
            
            if (value == null)
            {
                _valueWrapper = null;
            }
            else
            {
                var wrapperType = typeof(ValueWrapper<>).MakeGenericType(value.GetType());
                _valueWrapper = Activator.CreateInstance(wrapperType, value) as ValueWrapper;
            }
        }

        public string Name => _name;
        public Type DataType => _valueWrapper?.DataType;
        public RuntimeGraph Graph => _graph;
        public Hash128 ID => _id;
        public RuntimeVariableKind VariableKind => _variableKind;

        public bool TryGetDefaultValue<T>(out T defaultValue)
        {
            if (DataType == typeof(T))
            {
                defaultValue = default;
                return false;
            }
            
            return _valueWrapper.TryGetValue<T>(out defaultValue);
        }
    }
}