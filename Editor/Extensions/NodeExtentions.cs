using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public static class NodeExtensions
    {
        public static IEnumerable<IPort> GetBlockInputPorts(this ContextNode node, bool includeContext = true)
        {
            var blockPorts = node.BlockNodes.SelectMany(blockNode => blockNode.GetInputPorts()).ToList();
            if (includeContext)
            {
                blockPorts.AddRange(node.GetInputPorts());
            }
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

    }
}