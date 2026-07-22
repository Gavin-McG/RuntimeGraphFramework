using System.Collections.Generic;

namespace RuntimeGraphFramework
{
    public interface IRuntimeGraph
    {
        string Name { get; }
        
        int NodeCount { get; }
        IEnumerable<IRuntimeNode> GetNodes();
        IRuntimeNode GetNode(int index);
        
        int VariableCount { get; }
        IEnumerable<IRuntimeVariable> GetVariables();
        IRuntimeVariable GetVariable(int index);
    }
}
