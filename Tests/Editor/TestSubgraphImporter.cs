using RuntimeGraphFramework.Editor;
using UnityEditor.AssetImporters;

namespace RuntimeGraphFramework.Tests.Editor
{
    [ScriptedImporter(1, TestEditorSubgraph.AssetExtension)]
    public class TestSubgraphImporter : GraphImporter<TestEditorSubgraph, TestGraph>
    {
        public override void DefineRuntimeGraph(TestGraph runtimeGraph, GraphImportContext ctx)
        {
            
        }
    }
}