using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("RuntimeGraphFramework.Editor")]

namespace RuntimeGraphFramework
{
    public class RuntimeGraph : ScriptableObject, IRuntimeGraph
    {
        [SerializeField] internal Hash128 graphID;
        
        [SerializeField] internal List<RuntimeNode> nodes = new();
        [SerializeField] internal List<RuntimeVariable> variables = new();
        [SerializeField] internal Dictionary<string, int> variableNames = new();
        
        [SerializeField] internal bool valid = true;
        [SerializeField] internal string importMessage;
        
        public string Name => name;
        public Hash128 ID => graphID;
        public bool IsValid => valid;
        
        #region Node Methods

        /// <summary>
        /// The number of nodes currently owned by the Graph
        /// </summary>
        public int NodeCount => nodes.Count;

        /// <summary>
        /// Add a Node to this Graph. The Node must not already belong to a graph
        /// </summary>
        internal void AddNode(RuntimeNode node)
        {
            if (node.graph != null) 
                throw new Exception("Cannot add Node: Node already belongs to a graph.");
            
            // Add node to graph
            node.graph = this;
            nodes.Add(node);
        }

        /// <summary>
        /// Removes a node from this graph, disconnecting all its ports. The Node must belong to this graph
        /// </summary>
        internal void RemoveNode(IRuntimeNode node)
        {
            if (node.Graph != this)
                throw new Exception("Cannot Remove Node: Node must belong to this graph.");
            
            // Remove connections
            var runtimeNode = node as RuntimeNode;
            if (runtimeNode == null) throw new Exception("Cannot Remove Node: Node must inherit from RuntimeNode.");
            
            foreach (var input in runtimeNode.inputPorts)
            {
                foreach (var conectedPort in input.GetConnectedPorts())
                {
                    Disconnect(input, conectedPort);
                }
            }
            
            // Remove node from graph
            nodes.Remove(runtimeNode);
            runtimeNode.graph = null;
        }
        
        /// <summary>
        /// Returns a list of all nodes owned by this Graph
        /// </summary>
        public IEnumerable<IRuntimeNode> GetNodes() => nodes;
        
        /// <summary>
        /// Returns the node as a specific index within the graph
        /// </summary>
        public IRuntimeNode GetNode(int index) => nodes.ElementAtOrDefault(index);
        
        #endregion
        
        #region Variable Methods
        
        /// <summary>
        /// The number of variables currently attached to the graph
        /// </summary>
        public int VariableCount => variables.Count;

        /// <summary>
        /// Creates and Returns a new variable on the Graph
        /// </summary>
        internal IRuntimeVariable CreateVariable(string name, Type valueType, object defaultValue, RuntimeVariableKind kind)
        {
            var newVariable = new RuntimeVariable(name, this, defaultValue, kind);
            
            variables.Add(newVariable);
            
            return newVariable;
        }

        /// <summary>
        /// Remove a variable from the graph, along with any Variable Nodes of the variable
        /// </summary>
        /// <param name="variable">Variable to remove</param>
        /// <param name="forceRemove">Whether to allow removal if variable nodes exist</param>
        internal void RemoveVariable(IRuntimeVariable variable, bool forceRemove)
        {
            if (variable.Graph != this) 
                throw new Exception("Cannot Remove Variable: Variable must belong to this graph.");

            if (!forceRemove && variable.NodeCount > 0) return;

            foreach (var node in variable.GetNodes())
            {
                RemoveNode(node);
            }
        }
        
        public IEnumerable<IRuntimeVariable> GetVariables() => variables;

        public IRuntimeVariable GetVariable(int index)
        {
            return variables.ElementAtOrDefault(index);
        }

        public IRuntimeVariable GetVariable(string name)
        {
            if (!variableNames.TryGetValue(name, out int index)) return null;
            return GetVariable(index);
        }
        
        #endregion
        
        #region Connection Methods

        internal bool Connect(IRuntimePort output, IRuntimePort input)
        {
            var outputPort = output.GetPortReference().GetPort();
            var inputPort = input.GetPortReference().GetPort();

            if (outputPort == null || inputPort == null)
            {
                Debug.LogError($"Cannot Connect {output} from {input}");
                return false;
            }
            
            outputPort.AddConnection(input);
            inputPort.AddConnection(output);
            return true;
        }

        internal bool Disconnect(IRuntimePort output, IRuntimePort input)
        {
            var outputPort = output.GetPortReference().GetPort();
            var inputPort = input.GetPortReference().GetPort();

            if (outputPort == null || inputPort == null)
            {
                Debug.LogError($"Cannot Disconnect {output} from {input}");
                return false;
            }
            
            outputPort.RemoveConnections(input);
            inputPort.RemoveConnections(output);
            return true;
        }
        
        #endregion

        public bool TryGetGraphOutput<T>(IQueryContext context, string outputName, out T value) 
        {
            value = default;

            // Get index of Variable
            if (!variableNames.TryGetValue(outputName, out var i)) return false;

            // Get variable and check for variable kind
            var variable = GetVariable(i);
            if (variable?.VariableKind != RuntimeVariableKind.Output) return false;

            // get the input port of the variable's node
            var variableNode = variable.GetNodes().FirstOrDefault();
            var inputPort = variableNode?.GetInputPort(0);

            // Query the node's input
            return inputPort != null && inputPort.TryGetNodeInput(context, out value);
        }
    }
}