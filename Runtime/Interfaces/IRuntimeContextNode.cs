using System.Collections.Generic;

namespace RuntimeGraphFramework
{
    public interface IRuntimeContextNode : IRuntimeNode
    {
        public int BlockCount { get; }
        public IEnumerable<RuntimeBlockNode> BlockNodes { get; }
    }
}