using System;
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

        [InitializeOnLoadMethod]
        public static void RegisterTypeCasts()
        {
            TestGraph.RegisterTypeCasts();
        }
    }
    
    [Serializable, Node(null, null, "Math Node"), UseWithGraph(typeof(TestEditorGraph))]
    public class MathEditorNode : ExampleNodes.Editor.MathEditorNode {}
    
    [Serializable, Node(null, null, "Logic Node"), UseWithGraph(typeof(TestEditorGraph))]
    public class LogicEditorNode : ExampleNodes.Editor.LogicEditorNode {}
    
    [Serializable, Node(null, null, "Comparison Node"), UseWithGraph(typeof(TestEditorGraph))]
    public class ComparisonEditorNode : ExampleNodes.Editor.ComparisonEditorNode {}
    
    [Serializable, Node(null, null, "Split Node"), UseWithGraph(typeof(TestEditorGraph))]
    public class SplitEditorNode : ExampleNodes.Editor.SplitEditorNode {}
    
    [Serializable, Node(null, null, "Concatenate Node"), UseWithGraph(typeof(TestEditorGraph))]
    public class ConcatenateEditorNode : ExampleNodes.Editor.ConcatenateEditorNode {}
}