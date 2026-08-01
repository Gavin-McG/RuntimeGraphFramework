using System.Linq;
using RuntimeGraphFramework.Editor;
using UnityEditor.AssetImporters;

namespace RuntimeGraphFramework.Tests.Editor
{
    [ScriptedImporter(2, TestEditorGraph.AssetExtension)]
    public class TestGraphImporter : GraphImporter<TestEditorGraph, TestGraph> {}
}