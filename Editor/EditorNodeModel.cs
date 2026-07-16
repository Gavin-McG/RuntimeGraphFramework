using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public class EditorNodeModel<TRuntimeNode, TGraph> : IEditorNode<TRuntimeNode>
        where TRuntimeNode : RuntimeNode
        where TGraph : RuntimeGraph
    {
        private readonly IEditorNodeOwner<TRuntimeNode> _owner;

        public EditorNodeModel(IEditorNodeOwner<TRuntimeNode> owner)
        {
            _owner = owner;
        }
        
        private Dictionary<Hash128, TRuntimeNode> _nodes = new();
        
        private bool _portsRegistered = false;
        private List<IPort> _outputPorts = new();
        private List<IPort> _inputPorts = new();
        private Dictionary<IPort, int> _outputPortIndices = new();
        private Dictionary<IPort, int> _inputPortIndices = new();
        
        public void ClearData()
        {
            _nodes.Clear();

            _portsRegistered = false;
            _outputPorts.Clear();
            _inputPorts.Clear();
            _outputPortIndices.Clear();
            _inputPortIndices.Clear();
        }
        
        private void TryRegisterPorts()
        {
            if (_portsRegistered) return;
            _portsRegistered = true;
            
            // Collect all Typed ports
            _outputPorts = _owner.GetOutputPorts().Where(port => port.DataType != typeof(Untyped)).ToList();
            _inputPorts = _owner.GetInputPorts().Where(port => port.DataType != typeof(Untyped)).ToList();
            
            // Create index dictionary
            foreach (var port in _outputPorts)
            {
                _outputPortIndices[port] = _outputPortIndices.Count;
            }
            foreach (var port in _inputPorts)
            {
                _inputPortIndices[port] = _inputPortIndices.Count;
            }
        }

        private TRuntimeNode GetRegisteredNode(DialogueImportContext context)
        {
            Hash128 nodeKey = default;
            if (context.currentSubgraph != null) nodeKey = context.currentSubgraph.ID;
            return _nodes.GetValueOrDefault(nodeKey);
        }

        private void RegisterNode(DialogueImportContext context, TRuntimeNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            Hash128 nodeKey = default;
            if (context.currentSubgraph != null) nodeKey = context.currentSubgraph.ID;
            _nodes[nodeKey] = node;
        }

        public TRuntimeNode GetRuntimeNode(DialogueImportContext context)
        {
            // Check if already Initialized
            var currentNode = GetRegisteredNode(context);
            if (currentNode != null) return currentNode;
            
            // Create instance
            var newNode = ScriptableObject.CreateInstance<TRuntimeNode>();
            RegisterNode(context, newNode);
            
            // Set ID
            newNode.nodeID = _owner.ID;
            if (context.currentSubgraph != null) 
                newNode.nodeID.Append(context.currentSubgraph.ID.ToString());
            
            // Create ports
            TryRegisterPorts();
            foreach (var port in _inputPorts)
            {
                var InputPort = port.CreateRuntimeInputPort<TGraph>(context);
                newNode.inputPorts.Add(InputPort);
            }
            foreach (var port in _outputPorts)
            {
                var OutputPort = port.CreateRuntimeOutputPort(newNode);
                newNode.outputPorts.Add(OutputPort);
            }
            
            // Set untyped port bool
            newNode.hasUntypedPorts = 
                _owner.GetInputPorts().Any(port => port.DataType == typeof(Untyped)) ||
                _owner.GetOutputPorts().Any(port => port.DataType == typeof(Untyped));

            _owner.InitializeRuntimeNode(context, newNode);
            return newNode;
        }
        
        public IEnumerable<TRuntimeNode> GetRuntimeNodes() =>_nodes.Select(node => node.Value);

        public bool TryGetOutputPortIndex(IPort port, out int portIndex)
        {
            TryRegisterPorts();
            return _outputPortIndices.TryGetValue(port, out portIndex);
        }

        public bool TryGetInputPortIndex(IPort port, out int portIndex)
        {
            TryRegisterPorts();
            return _inputPortIndices.TryGetValue(port, out portIndex);
        }
    }
}