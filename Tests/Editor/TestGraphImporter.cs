using System.Linq;
using RuntimeGraphFramework.Editor;
using UnityEditor.AssetImporters;

namespace RuntimeGraphFramework.Tests.Editor
{
    [ScriptedImporter(1, TestEditorGraph.AssetExtension)]
    public class TestGraphImporter : GraphImporter<TestEditorGraph, TestGraph>
    {
        public override void DefineRuntimeGraph(TestGraph runtimeGraph, GraphImportContext ctx)
        {
            runtimeGraph.nodes = editorGraph.GetNodes<StartEditorNode>(false).Select(node => node.GetRuntimeNode(ctx)).ToList();
        }
    }
}