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
    }
}