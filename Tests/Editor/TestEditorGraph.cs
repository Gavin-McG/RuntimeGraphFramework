using System;
using RuntimeGraphFramework.Editor;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace RuntimeGraphFramework.Tests.Editor
{
    [Graph(AssetExtension)]
    [Serializable]
    public class TestEditorGraph : EditorGraph<TestGraph>
    {
        public const string AssetExtension = "testgraph";
        
        [MenuItem("Assets/Create/Runtime Graph Framework/Test Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<TestEditorGraph>();
        }
    }
}