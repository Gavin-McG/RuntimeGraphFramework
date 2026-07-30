using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public class RuntimeVariable : IRuntimeVariable
    {
        [SerializeField] private string _name;
        [SerializeField] private RuntimeGraph _graph;
        [SerializeField] private Hash128 _id;
        [SerializeField] private RuntimeVariableKind _kind;
        [SerializeReference] private ValueWrapper _valueWrapper;
        [SerializeField] private List<RuntimeNode> _nodes = new();

        private void SetValueWrapper(object value)
        {
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

        public RuntimeVariable(string name, RuntimeGraph graph, object defaultValue, RuntimeVariableKind kind)
        {
            _name = name;
            _graph = graph;
            _id = default; //TODO
            _kind = kind;
            
            SetValueWrapper(defaultValue);
        }
        
        public RuntimeVariable(string name, RuntimeGraph graph, object defaultValue, RuntimeVariableKind kind, Hash128 id)
        : this(name, graph, defaultValue, kind)
        {
            _id = id;
        }
        
        public string Name => _name;
        public Type DataType => _valueWrapper?.DataType;
        public RuntimeGraph Graph => _graph;
        public Hash128 ID => _id;
        public RuntimeVariableKind VariableKind => _kind;
        public int NodeCount => _nodes.Count;
        
        // Public methods
        public IEnumerable<IRuntimeNode> GetNodes() => _nodes;

        public bool TryGetDefaultValue<T>(out T defaultValue)
        {
            return _valueWrapper.TryGetValue(out defaultValue);
        }

        // Internal methods
        internal bool TrySetDefaultValue<T>(ref T defaultValue)
        {
            SetValueWrapper(defaultValue);
            return true;
        }
        
        internal void AddNode(RuntimeNode node) => _nodes.Add(node);

        internal void RemoveFromGraph(bool forceRemove)
        {
            Graph.RemoveVariable(this, forceRemove);
        }
    }
}