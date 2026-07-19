using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public abstract class GraphImporter<TEditorGraph, TRuntimeGraph> : ScriptedImporter
        where TRuntimeGraph : RuntimeGraph
        where TEditorGraph : EditorGraph<TRuntimeGraph>
    {
        [Tooltip("Prevents Node outputs from being pre-computed while importing.")]
        [SerializeField] bool DebugMode = false;
        
        [Tooltip("Allows the individual Nodes to be observable in the Project Menu.")]
        [SerializeField] bool DeveloperMode = false;
        
        protected TEditorGraph editorGraph;
        
        private void RemoveConstantNodes(HashSet<RuntimeNode> runtimeNodes, GraphImportContext context)
        {
            // Delete all Constants iteratively
            var constantContext = new BlankQueryContext();
            var constantNodes = new HashSet<RuntimeNode>();
            do
            {
                // Remove and clear constant Nodes
                foreach (var constantNode in constantNodes)
                {
                    runtimeNodes.Remove(constantNode);
                    DestroyImmediate(constantNode);
                }
                constantNodes.Clear();
                
                // Find all current Constant Nodes
                foreach (var runtimeNode in runtimeNodes)
                {
                    if (runtimeNode.IsConstantNode()) constantNodes.Add(runtimeNode);
                }
                
                // Remove Input Port references to Constant Nodes
                constantContext.RefreshQueryID();
                // foreach (var runtimeNode in runtimeNodes)
                // {
                //     for (int i = 0; i < runtimeNode.inputPorts.Count; ++i)
                //     {
                //         RuntimePort inputPort = runtimeNode.inputPorts[i];
                //         if (true) continue; //TODO
                //         
                //         var outputPort = inputPort.FirstConnectedPort;
                //         if (outputPort == null) continue;
                //         if (!constantNodes.Contains(outputPort.GetNode())) continue;
                //
                //         runtimeNode.inputPorts[i] = inputPort.CreateConstantInputPort(
                //             context,
                //             outputPort.GetValue<object>(constantContext)
                //         );
                //     }
                // }
            } while (constantNodes.Count != 0);
        }
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            editorGraph = GraphDatabase.LoadGraphForImporter<TEditorGraph>(ctx.assetPath);

            // Create main Graph asset
            var runtimeGraph = ScriptableObject.CreateInstance<TRuntimeGraph>();
            runtimeGraph.graphID = editorGraph.ID;
            ctx.AddObjectToAsset(runtimeGraph.GetType().Name, runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
            
            // Clear existing nodes' Data
            var editorNodes = editorGraph.GetNodes<IEditorNode<RuntimeNode>>().ToList();
            foreach (var editorNode in editorNodes)
            {
                editorNode.ClearData();
            }
            
            // Print out un-addable Variables
            var variableGroups = editorGraph.GetVariableGroups().ToList();
            foreach (var variableGroup in variableGroups)
            {
                if (variableGroup.Any(variable => 
                        variable.VariableKind == VariableKind.Local && 
                        variable.DataType != typeof(Untyped) && 
                        variable.DataType != variableGroup.First().DataType
                    )) {
                    Debug.LogError($"Could not include Variable \"{variableGroup.Key}\" in the Graph because there are multiple definitions. The default values will be used for all instances of this variable name (at <a href=\"{assetPath}\">{assetPath}</a>)", runtimeGraph);
                }
            }
            
            // Add all variables to Graph
            var validVariables = editorGraph.GetValidVariables(variableGroups);
            runtimeGraph.variables = validVariables.ToDictionary(
                variable => variable.Name,
                variable => variable.GetRuntimeVariable()
            );

            // Define graph
            var importContext = new GraphImportContext()
            {
                assetContext = ctx,
                runtimeGraph = runtimeGraph,
                currentSubgraph = null,
                validVariables = validVariables
            };
            DefineRuntimeGraph(runtimeGraph, importContext);

            // Get all Nodes connected to Enter Nodes
            var runtimeNodes = editorNodes
                .SelectMany(editorNode => editorNode.GetRuntimeNodes())
                .ToHashSet();

            // Remove all Nodes that can be pre-computed
            if (!DebugMode)
            {
                RemoveConstantNodes(runtimeNodes, importContext);
            }

            // Add all remaining Nodes to the asset
            foreach (var runtimeNode in runtimeNodes)
            {
                if (!DeveloperMode) runtimeNode.hideFlags = HideFlags.HideInHierarchy;
                ctx.AddObjectToAsset(runtimeNode.nodeID.ToString(), runtimeNode);
            }
        }
        
        public abstract void DefineRuntimeGraph(TRuntimeGraph runtimeGraph, GraphImportContext ctx);
    }
}