using System;
using UnityEngine;

namespace RuntimeGraphFramework
{
    [Serializable]
    public abstract class RuntimePort
    {
        [SerializeField] private string name;
        [SerializeField] private Hash128 id;
        [SerializeField] private RuntimeNode node;
        
        public string Name => name;
        public Hash128 ID => id;
        public RuntimeNode Node => node;
        public abstract Type DataType { get; }

        protected RuntimePort(string name, Hash128 id, RuntimeNode node)
        {
            this.name = name;
            this.id = id;
            this.node = node;
        }
    }
}