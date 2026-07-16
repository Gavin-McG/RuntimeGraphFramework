using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IRuntimeNode
    {
        public RuntimeGraph Graph { get; }
        public Hash128 ID { get; }
        
        public int InputPortCount { get; }
        public int OutputPortCount { get; }
        
        public InputPort GetInputPort(int index);
        public OutputPort GetOutputPort(int index);
    }
}