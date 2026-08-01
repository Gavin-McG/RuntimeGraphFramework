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

        
        /// <summary>
        /// Simple test of Constant Node connected to Output
        /// </summary>
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
        

        /// <summary>
        /// Constant Node connected to Output, but retrieved as wrong type
        /// </summary>
        [Test]
        public static void ConstantNodeFail_Test()
        {
            var graph = LoadGraph("ConstantNodeFail_Test");
            
            var context = new QueryContext(graph);
            Assert.IsFalse(graph.TryGetGraphOutput(context, "output", out Vector2 output));
            Assert.AreEqual(default(Vector2), output);
        }
        

        /// <summary>
        /// Simple test of Variable Node connected to Output
        /// </summary>
        [Test]
        public static void VariableNode_Test()
        {
            var graph = LoadGraph("VariableNode_Test");
            
            // Default value
            var context = new QueryContext(graph);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output", out float output));
            Assert.AreEqual(5f, output);

            // Overridden value
            context.SetVariable("variable", 10f);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output", out output));
            Assert.AreEqual(10f, output);
        }
        

        /// <summary>
        /// Variable Node connected to Output, but retrieved as wrong type
        /// </summary>
        [Test]
        public static void VariableNodeFail_Test()
        {
            var graph = LoadGraph("VariableNodeFail_Test");
            
            var context = new QueryContext(graph);
            Assert.IsFalse(graph.TryGetGraphOutput(context, "output", out Vector2 output));
            Assert.AreEqual(default(Vector2), output);
        }
        

        /// <summary>
        /// Test Simple type cast with Constant Node
        /// </summary>
        [Test]
        public static void TypeCast_Test()
        {
            var graph = LoadGraph("TypeCast_Test");
            
            var context = new QueryContext(graph);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output", out string output));
            Assert.AreEqual("5", output);
        }
        

        /// <summary>
        /// Test Custom Math Node
        /// </summary>
        [Test]
        public static void MathNode_Test()
        {
            var graph = LoadGraph("MathNode_Test");
            
            var context = new QueryContext(graph);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output1", out float output1));
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output2", out float output2));
            
            Assert.AreEqual(10f, output1);
            Assert.AreEqual(25f, output2);
        }

        
        /// <summary>
        /// Test Math Node within local Subgraph
        /// </summary>
        [Test]
        public static void LocalSubgraph_Test()
        {
            var graph = LoadGraph("LocalSubgraph_Test");
            
            var context = new QueryContext(graph);
            Assert.IsTrue(graph.TryGetGraphOutput(context, "output", out float output));
            Assert.AreEqual(15, output);
        }
    }
}