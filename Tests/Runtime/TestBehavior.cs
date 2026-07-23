using System.Linq;
using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class TestBehavior : MonoBehaviour
    {
        [SerializeField] TestGraph _graph;

        private void Start()
        {
            _graph.startNodes.First()._inputPort.TryGetValue(new BlankQueryContext(), out string value);
            Debug.Log(value);
        }
    }
}