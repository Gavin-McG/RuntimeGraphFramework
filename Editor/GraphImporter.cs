using Unity.GraphToolkit.Editor;
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
        
        // private void RemoveConstantNodes(HashSet<RuntimeNode> runtimeNodes, GraphImportContext context)
        // {
        //     // Delete all Constants iteratively
        //     var constantContext = new BlankQueryContext();
        //     var constantNodes = new HashSet<RuntimeNode>();
        //     do
        //     {
        //         // Remove and clear constant Nodes
        //         foreach (var constantNode in constantNodes)
        //         {
        //             runtimeNodes.Remove(constantNode);
        //             DestroyImmediate(constantNode);
        //         }
        //         constantNodes.Clear();
        //         
        //         // Find all current Constant Nodes
        //         foreach (var runtimeNode in runtimeNodes)
        //         {
        //             if (runtimeNode.IsConstantNode()) constantNodes.Add(runtimeNode);
        //         }
        //         
        //         // Remove Input Port references to Constant Nodes
        //         constantContext.RefreshQueryID();
        //         foreach (var runtimeNode in runtimeNodes)
        //         {
        //             for (int i = 0; i < runtimeNode.inputPorts.Count; ++i)
        //             {
        //                 RuntimePort inputPort = runtimeNode.inputPorts[i];
        //                 if (true) continue; //TODO
        //                 
        //                 var outputPort = inputPort.FirstConnectedPort;
        //                 if (outputPort == null) continue;
        //                 if (!constantNodes.Contains(outputPort.GetNode())) continue;
        //         
        //                 runtimeNode.inputPorts[i] = inputPort.CreateConstantInputPort(
        //                     context,
        //                     outputPort.GetValue<object>(constantContext)
        //                 );
        //             }
        //         }
        //     } while (constantNodes.Count != 0);
        // }
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            editorGraph = GraphDatabase.LoadGraphForImporter<TEditorGraph>(ctx.assetPath);

            // Create main Graph asset
            var importContext = new GraphImportContext()
            {
                assetContext = ctx
            };

            var runtimeGraph = editorGraph.CreateGraph(importContext);
            
            // Remove all Nodes that can be pre-computed
            if (!DebugMode)
            {
                //RemoveConstantNodes(runtimeNodes, importContext);
            }
            
            // Add assets to Graph
            foreach (var asset in importContext.Assets)
            {
                if (asset is RuntimeGraph graph) ctx.AddObjectToAsset(graph.graphID.ToString(), asset);
                else if (asset is RuntimeNode node) ctx.AddObjectToAsset(node.ID.ToString(), node); 
                else ctx.AddObjectToAsset(asset.GetHashCode().ToString(), asset);
            }
            
            ctx.SetMainObject(runtimeGraph);
        }
    }
}