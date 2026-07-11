using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorNode<TRuntimeNode, TGraph> : Node, IEditorNode<TRuntimeNode> 
        where TRuntimeNode : RuntimeNode 
        where TGraph : RuntimeGraph
    {
        private TRuntimeNode _node;
        private Dictionary<Hash128, TRuntimeNode> _subgraphNodes = new();
        
        private Dictionary<IPort, int> _outputPortIndices = new();
        private Dictionary<IPort, int> _inputPortIndices = new();
        
        public void ClearData()
        {
            _node = null;
            _subgraphNodes.Clear();
            _outputPortIndices.Clear();
        }

        private TRuntimeNode GetRegisteredNode(DialogueImportContext context)
        {
            if (context.currentSubgraph == null && _node != null) return _node;
            if (context.currentSubgraph != null) return _subgraphNodes.GetValueOrDefault(context.currentSubgraph.ID);
            return null;
        }

        private void RegisterNode(DialogueImportContext context, TRuntimeNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (context.currentSubgraph == null) _node = node;
            else _subgraphNodes[context.currentSubgraph.ID] = node;
        }

        public TRuntimeNode GetRuntimeNode(DialogueImportContext context)
        {
            // Check if already Initialized
            var currentNode = GetRegisteredNode(context);
            if (currentNode != null) return currentNode;
            
            // Create instance
            var newNode = ScriptableObject.CreateInstance<TRuntimeNode>();
            newNode.nodeID = ID;
            if (context.currentSubgraph != null) newNode.nodeID.Append(context.currentSubgraph.ID.ToString());
            RegisterNode(context, newNode);
            
            // Create ports
            foreach (var port in GetInputPorts().Where(port => port.DataType != typeof(Untyped)))
            {
                var InputPort = port.CreateRuntimeInputPort<TGraph>(context);
                _inputPortIndices[port] = newNode.inputPorts.Count;
                newNode.inputPorts.Add(InputPort);
            }
            if (newNode is RuntimeNode dataNode)
            {
                foreach (var port in GetOutputPorts().Where(port => port.DataType != typeof(Untyped)))
                {
                    var OutputPort = port.CreateRuntimeOutputPort(dataNode);
                    _outputPortIndices[port] = dataNode.outputPorts.Count;
                    dataNode.outputPorts.Add(OutputPort);
                }
            }

            // Initialize Node Object
            InitializeRuntimeNode(context, newNode);
            return newNode;
        }

        public OutputPort GetRuntimeOutputPort(DialogueImportContext context, IPort port)
        {
            if (port == null) return null;
            var node = GetRuntimeNode(context);
            var index = _outputPortIndices.GetValueOrDefault(port);
            return node.outputPorts[index];
        }
        
        public OutputPortReference GetRuntimeOutputPortReference(DialogueImportContext context, IPort port)
        {
            if (port == null) return null;
            var node = GetRuntimeNode(context);
            return new OutputPortReference(node, _outputPortIndices[port]);
        }

        public InputPort GetRuntimeInputPort(DialogueImportContext context, IPort port)
        {
            if (port == null) return null;
            var node = GetRuntimeNode(context);
            var index = _inputPortIndices.GetValueOrDefault(port);
            return node.inputPorts[index];
        }

        public InputPortReference GetRuntimeInputPortReference(DialogueImportContext context, IPort port)
        {
            if (port == null) return null;
            var node = GetRuntimeNode(context);
            return new InputPortReference(node, _inputPortIndices[port]);
        }

        public IEnumerable<TRuntimeNode> GetNodes()
        {
            if (_node != null) yield return _node;
            foreach (var pair in _subgraphNodes)
            {
                if (pair.Value != null) yield return pair.Value;
            }
        }
        
        protected abstract void InitializeRuntimeNode(DialogueImportContext context, TRuntimeNode node);
    }
}