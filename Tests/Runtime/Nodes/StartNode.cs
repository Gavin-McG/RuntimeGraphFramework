using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class StartNode : RuntimeNode
    {
        [SerializeField] public RuntimePortReference _inputPort;
        [SerializeField] public RuntimeNode nextNode;
    }
}