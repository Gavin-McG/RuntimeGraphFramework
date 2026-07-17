using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class StartNode : RuntimeNode
    {
        [SerializeField] public RuntimeNode nextNode;
    }
}