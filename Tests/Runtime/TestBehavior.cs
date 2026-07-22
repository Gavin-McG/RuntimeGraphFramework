using System;
using System.Linq;
using RuntimeGraphFramework.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class TestBehavior : MonoBehaviour
    {
        [SerializeField] TestGraph _graph;

        private void Start()
        {
            _graph.nodes.First()._inputPort.TryGetValue(new BlankQueryContext(), out string value);
            Debug.Log(value);
        }
    }
}