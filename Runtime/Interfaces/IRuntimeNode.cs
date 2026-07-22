using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IRuntimeNode
    {
        public RuntimeGraph Graph { get; }
        public Hash128 ID { get; }
        
        public int InputPortCount { get; }
        public int OutputPortCount { get; }
        
        public IRuntimePort GetInputPort(int index);
        public IRuntimePort GetOutputPort(int index);
    }
}