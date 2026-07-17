using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class BasicNode : RuntimeNode
    {
        [SerializeField] public RuntimeNode nextNode;
    }
}