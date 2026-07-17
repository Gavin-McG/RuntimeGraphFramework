using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Graph(AssetExtension)]
    [Serializable]
    public class TestEditorSubgraph : EditorGraph<TestGraph>
    {
        public const string AssetExtension = "testsubgraph";
        
        [MenuItem("Assets/Create/Runtime Graph Framework/Test Subgraph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<TestEditorSubgraph>();
        }
    }
}