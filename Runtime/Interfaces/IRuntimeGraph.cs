using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IRuntimeGraph
    {
        string Name { get; }
        Hash128 ID { get; }
        
        // Node Methods
        int NodeCount { get; }
        // void AddNode(RuntimeNode node);
        // void RemoveNode(IRuntimeNode node);
        IEnumerable<IRuntimeNode> GetNodes();
        IRuntimeNode GetNode(int index);
        
        // Variable Methods
        int VariableCount { get; }
        // IRuntimeVariable CreateVariable(string name, Type valueType, object defaultValue, RuntimeVariableKind kind);
        // void RemoveVariable(IRuntimeVariable variable, bool forceRemove);
        IEnumerable<IRuntimeVariable> GetVariables();
        IRuntimeVariable GetVariable(int index);
        
        // Built-in Node methods
        // RuntimeSubgraphNode AddSubgraphNode(RuntimeGraph subgraph);
        // RuntimeSubgraphNode AddLocalSubgraphNode(Type subgraphType);
        // void AddVariableNode(IRuntimeVariable variable);
        // RuntimeConstantNode CreateConstantNode(Type type, object value);
        
        // Connection methods
        // bool Connect(IRuntimePort output, IRuntimePort input);
        // bool Disconnect(IRuntimePort output, IRuntimePort input);
    }
}
