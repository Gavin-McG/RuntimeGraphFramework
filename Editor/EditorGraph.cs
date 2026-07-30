using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    [Serializable]
    public abstract class EditorGraph : Graph
    {
        public abstract RuntimeGraph CreateGraph(GraphImportContext context);
    }
    
    [Serializable]
    public abstract class EditorGraph<TGraph> : EditorGraph where TGraph : RuntimeGraph
    {
        public bool CanTypesConnect(Type outputType, Type inputType)
        {
            return PortTypeCastManager.CanTypesCast<TGraph>(outputType, inputType);
        }

        private List<INode> GetAllNodes()
        {
            var nodes = GetNodes().ToList();
            var contextNodes = nodes.OfType<ContextNode>();
            foreach (var contextNode in contextNodes)
            {
                var blockNodes = contextNode.BlockNodes;
                nodes.AddRange(blockNodes);
            }
            return nodes;
        }

        private List<IEditorNode<RuntimeNode>> GetAllEditorNodes(GraphImportContext context)
        {
            return GetAllNodes()
                .Select(node => node.AsEditorNode(context))
                .ToList();
        }
        
        private void ClearAllNodeData(GraphImportContext context)
        {
            var editorNodes = GetAllEditorNodes(context);
            foreach (var editorNode in editorNodes) editorNode.ClearData();
        }

        public override RuntimeGraph CreateGraph(GraphImportContext context)
        {
            // Create Graph asset
            var runtimeGraph = ScriptableObject.CreateInstance<TGraph>();
            context.EnterGraph(runtimeGraph);
            context.AddAsset(runtimeGraph);
            runtimeGraph.graphID = ID;
            
            // Initialize Nodes
            ClearAllNodeData(context);
            var editorNodes = GetAllEditorNodes(context);
            foreach (var editorNode in editorNodes) editorNode.CreateRuntimeNode(context);
            foreach (var editorNode in editorNodes) editorNode.ConnectRuntimeNode(context);
            foreach (var editorNode in editorNodes) editorNode.InitializeRuntimeNode(context);
            
            // Initialize Variables
            runtimeGraph.variables = GetRuntimeVariables(context).ToList();
            
            // Define Graph
            DefineRuntimeGraph(context, runtimeGraph);
            
            context.ExitGraph();
            return runtimeGraph;
        }
        
        protected virtual void DefineRuntimeGraph(GraphImportContext ctx, TGraph runtimeGraph) {}
        
        public override bool IsConnectionAllowed(IPort output, IPort input)
        {
            // Check for Types
            Type outputType = output.DataType;
            Type inputType = input.DataType;
            if (!CanTypesConnect(outputType, inputType)) return false;
            
            // Check for Recursion of data ports
            INode outputNode = output.GetNode();
            INode inputNode = input.GetNode();
            if (outputNode == null || inputNode == null || !outputNode.WouldConnectionCreateCycle(inputNode)) return false;
            
            // Prevent Multiple Untyped input Variable connections
            if (outputNode is IVariableNode variableNode)
            {
                IVariable outputVariable = variableNode.Variable;
                if (outputVariable.VariableKind == VariableKind.Input && outputVariable.DataType == typeof(Untyped))
                {
                    var variableNodes = new List<IVariableNode>();
                    outputVariable.GetNodes(variableNodes);
                    return !variableNodes.Any(node => node.IsConnected);
                }
            }

            // Allow by Default
            return true;
        }

        public IEnumerable<IVariable> GetVariables(VariableKind variableKind)
        {
            return GetVariables(SortMethod.Display)
                .GroupBy(variable => variable.Name)
                .Select(group => group.First())
                .Where(variable => variable.VariableKind == variableKind);
        }

        public IEnumerable<RuntimeVariable> GetRuntimeVariables(GraphImportContext context)
        {
            return GetVariables(SortMethod.Display)
                .GroupBy(variable => variable.Name)
                .Select(group =>
                {
                    var variable = group.First();
                    var runtimeVariable = variable.CreateRuntimeVariable(context);
                    // Add nodes of Variable
                    variable.GetNodes()
                        .ToList()
                        .ForEach(node => 
                            runtimeVariable.AddNode(node.AsEditorNode(context).RuntimeNode)
                        );
                    return runtimeVariable;
                });
        }
    }
}