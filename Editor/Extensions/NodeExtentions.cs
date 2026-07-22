using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public static class NodeExtensions
    {
        public static IEnumerable<IPort> GetBlockInputPorts(this ContextNode node)
        {
            var blockPorts = node.BlockNodes.SelectMany(blockNode => blockNode.GetInputPorts()).ToList();
            return blockPorts;
        }
        
        public static IEnumerable<IPort> GetBlockOutputPorts(this ContextNode node)
        {
            var blockPorts = node.BlockNodes.SelectMany(blockNode => blockNode.GetInputPorts()).ToList();
            return blockPorts;
        }
        
        public static bool WouldConnectionCreateCycle(this INode outputNode, INode inputNode)
        {
            var outputConnectedIDs = GetConnectedNodeIDs(outputNode);
            return !outputConnectedIDs.Contains(inputNode.ID);
        }

        public static HashSet<Hash128> GetConnectedNodeIDs(this INode node)
        {
            void GetConnectedHelper(INode node, HashSet<Hash128> ids)
            {
                if (node==null || !ids.Add(node.ID)) return;

                var inputPorts = node.GetInputPorts();
                foreach (var input in inputPorts)
                {
                    if (input.DataType == typeof(Untyped)) continue;
                    var connectedPorts = new List<IPort>();
                    input.GetConnectedPorts(connectedPorts);
                    foreach (var connectedPort in connectedPorts)
                    {
                        GetConnectedHelper(connectedPort.GetNode(), ids);
                    }
                }

                if (node is ContextNode contextNode)
                {
                    foreach (var blockNode in contextNode.BlockNodes)
                    {
                        GetConnectedHelper(blockNode, ids);
                    }
                }
            }
            
            HashSet<Hash128> set = new HashSet<Hash128>();
            GetConnectedHelper(node, set);
            return set;
        }

        public static List<string> GetRequiredVariables(this INode node)
        {
            void GetRequiredVariablesHelper(INode node, HashSet<string> variableNames, HashSet<Hash128> visited)
            {
                if (node == null || !visited.Add(node.ID)) return;
                if (node is IVariableNode variableNode)
                {
                    variableNames.Add(variableNode.Variable.Name);
                    return;
                }

                var inputPorts = node.GetInputPorts();
                foreach (var input in inputPorts)
                {
                    if (input.DataType == typeof(Untyped)) continue;
                    var connectedPorts = new List<IPort>();
                    input.GetConnectedPorts(connectedPorts);
                    foreach (var connectedPort in connectedPorts)
                    {
                        GetRequiredVariablesHelper(connectedPort.GetNode(), variableNames, visited);
                    }
                }

                if (node is ContextNode contextNode)
                {
                    foreach (var blockNode in contextNode.BlockNodes)
                    {
                        GetRequiredVariablesHelper(blockNode, variableNames, visited);
                    }
                }
            }
            
            HashSet<string> set = new HashSet<string>();
            HashSet<Hash128> visited = new HashSet<Hash128>();
            GetRequiredVariablesHelper(node, set, visited);
            return set.ToList();
        }

        public static RuntimeNode GetRuntimeNode(this INode node, GraphImportContext context)
        {
            if (node == null) return null;

            // IEditorNode
            if (node is IEditorNode<RuntimeNode> editorNode)
            {
                return editorNode.GetRuntimeNode(context);
            }
            
            // IConstantNode
            if (node is IConstantNode constantNode)
            {
                var editorConstantNode = context.GetConstantNode(constantNode);
                return editorConstantNode.GetRuntimeNode(context);
            }

            // IVariableNode
            if (node is IVariableNode variableNode)
            {
                var editorVariableNode = context.GetVariableNode(variableNode);
                return editorVariableNode.GetRuntimeNode(context);
            }

            // ISubgraphNode
            if (node is ISubgraphNode subgraphNode)
            {
                var editorSubgraphNode = context.GetSubgraphNode(subgraphNode);
                return editorSubgraphNode.GetRuntimeNode(context);
            }

            throw new ArgumentException($"Node type is not supported: {node.GetType()}");
        }
    }
}