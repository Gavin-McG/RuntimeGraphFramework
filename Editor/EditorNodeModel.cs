using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public class EditorNodeModel<TRuntimeNode> : IEditorNode<TRuntimeNode>
        where TRuntimeNode : RuntimeNode
    {
        private readonly IEditorNodeOwner<TRuntimeNode> _owner;

        public EditorNodeModel(IEditorNodeOwner<TRuntimeNode> owner)
        {
            _owner = owner;
        }
        
        private TRuntimeNode _node;
        
        private bool _portsRegistered = false;
        private List<IPort> _outputPorts = new();
        private List<IPort> _inputPorts = new();
        private readonly Dictionary<IPort, int> _outputPortIndices = new();
        private readonly Dictionary<IPort, int> _inputPortIndices = new();
        
        public bool IsCreated => _node != null;
        
        public void ClearData()
        {
            _node = null;

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
            _outputPorts = _owner.GetOutputPorts().ToList();
            _inputPorts = _owner.GetInputPorts().ToList();
            
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
        
        public TRuntimeNode GetRuntimeNode(GraphImportContext context)
        {
            // Check if already Initialized
            if (_node != null) return _node;
            
            // Create instance
            _node = ScriptableObject.CreateInstance<TRuntimeNode>();
            _node.name = typeof(TRuntimeNode).Name;
            _node.graph = context.runtimeGraph;
            
            // Set ID
            _node.nodeID = _owner.ID;
            
            // Create ports
            TryRegisterPorts();
            foreach (var port in _inputPorts)
            {
                var InputPort = port.CreateRuntimePort(context);
                _node.inputPorts.Add(InputPort);
            }
            foreach (var port in _outputPorts)
            {
                var OutputPort = port.CreateRuntimePort(context);
                _node.outputPorts.Add(OutputPort);
            }
            
            // Set port Connections
            foreach (var port in _inputPorts)
            {
                var index = _inputPortIndices[port];
                var connectedPorts = new List<IPort>();
                port.GetConnectedPorts(connectedPorts);
                connectedPorts.ForEach(port =>
                {
                    var portReference = port.GetRuntimePortReference(context);
                    _node.inputPorts[index].Connect(portReference);
                });
            }

            foreach (var port in _outputPorts)
            {
                var index = _outputPortIndices[port];
                var connectedPorts = new List<IPort>();
                port.GetConnectedPorts(connectedPorts);
                connectedPorts.ForEach(port =>
                {
                    var portReference = port.GetRuntimePortReference(context);
                    _node.outputPorts[index].Connect(portReference);
                });
            }
            
            // Initialize Node data
            _owner.InitializeRuntimeNode(context, _node);
            return _node;
        }
        
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