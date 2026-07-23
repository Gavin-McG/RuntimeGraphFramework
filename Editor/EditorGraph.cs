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
        
        private void ClearNodeData()
        {
            var editorNodes = GetNodes().OfType<IEditorNode<RuntimeNode>>();
            foreach (var editorNode in editorNodes)
            {
                editorNode.ClearData();
            }
        }


        public override RuntimeGraph CreateGraph(GraphImportContext context)
        {
            ClearNodeData();

            var runtimeGraph = ScriptableObject.CreateInstance<TGraph>();
            context.EnterGraph(runtimeGraph);
            context.AddAsset(runtimeGraph);
            
            runtimeGraph.graphID = ID;
            
            // Initialize Output variables
            runtimeGraph.variables = GetRuntimeVariables(context).ToList();
            GetVariables(VariableKind.Output)
                .SelectMany(variable => variable.GetNodes())
                .ToList()
                .ForEach(variableNode => variableNode.GetRuntimeNode(context));

            // Define Graph
            DefineRuntimeGraph(context, runtimeGraph);
            
            // Add all Nodes to graph
            var editorNode = GetNodes().OfType<IEditorNode<RuntimeNode>>();
            runtimeGraph.nodes = editorNode
                .Where(node => node.IsCreated)
                .Select(node => node.GetRuntimeNode(context))
                .ToList();
            runtimeGraph.nodes.AddRange(context.ConstantNodes.Select(node => node.GetRuntimeNode(context)));
            runtimeGraph.nodes.AddRange(context.VariableNodes.Select(node => node.GetRuntimeNode(context)));
            runtimeGraph.nodes.AddRange(context.SubgraphNodes.Select(node => node.GetRuntimeNode(context)));
            runtimeGraph.nodes.AddRange(context.MissingNodes.Select(node => node.GetRuntimeNode(context)));
            
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
                .Select(group => group.First().CreateRuntimeVariable(context));
        }
    }
}