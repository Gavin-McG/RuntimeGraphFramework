using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IRuntimeNode
    {
        public RuntimeGraph Graph { get; }
        public Hash128 ID { get; }
        
        public int InputPortCount { get; }
        public int OutputPortCount { get; }
        
        public RuntimePort GetInputPort(int index);
        public RuntimePort GetOutputPort(int index);
    }
}