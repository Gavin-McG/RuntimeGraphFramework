using System;
using System.Linq;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Graph(AssetExtension, GraphOptions.SupportsSubgraphs)]
    [Serializable]
    public class TestEditorGraph : EditorGraph<TestGraph>
    {
        public const string AssetExtension = "testgraph";
        
        [MenuItem("Assets/Create/Runtime Graph Framework/Test Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<TestEditorGraph>();
        }
        
        protected override void DefineRuntimeGraph(GraphImportContext ctx, TestGraph runtimeGraph)
        {
            runtimeGraph.startNodes = GetNodes()
                .OfType<StartEditorNode>()
                .Select(node => node.GetRuntimeNode(ctx))
                .ToList();
        }

    }
}