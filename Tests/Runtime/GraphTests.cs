using NUnit.Framework;
using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public static class GraphTests
    {
        private static TestGraph LoadGraph(string graphPath)
        {
            var graph = Resources.Load<TestGraph>($"Test Graphs/{graphPath}");
            if (graph == null) throw new System.Exception("Graph not found: " + graphPath);
            return graph;
        }

        [Test]
        public static void ConstantNode_Test()
        {
            var graph = LoadGraph("ConstantNode_Test");
            
            var context = new QueryContext(graph);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output1", out float output1));
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output2", out float output2));
            Assert.AreEqual(5f, output1);
            Assert.AreEqual(10f, output2);
        }

        [Test]
        public static void ConstantNodeFail_Test()
        {
            var graph = LoadGraph("ConstantNodeFail_Test");
            
            var context = new QueryContext(graph);
            Assert.IsFalse(graph.TryGetGraphOutput(context, "output", out Vector2 output));
            Assert.AreEqual(default(Vector2), output);
        }

        [Test]
        public static void VariableNode_Test()
        {
            var graph = LoadGraph("VariableNode_Test");
            
            // Default value
            var context = new QueryContext(graph);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output", out float output));
            Assert.AreEqual(5f, output);

            // Overridden value
            Assert.IsTrue(context.TrySetVariable("variable", 10f));
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output", out output));
            Assert.AreEqual(10f, output);
        }

        [Test]
        public static void VariableNodeFail_Test()
        {
            var graph = LoadGraph("VariableNodeFail_Test");
            
            var context = new QueryContext(graph);
            Assert.IsFalse(graph.TryGetGraphOutput(context, "output", out Vector2 output));
            Assert.AreEqual(default(Vector2), output);
        }

        [Test]
        public static void TypeCast_Test()
        {
            var graph = LoadGraph("TypeCast_Test");
            
            var context = new QueryContext(graph);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output", out string output));
            Assert.AreEqual("5", output);
        }
        
    }
}