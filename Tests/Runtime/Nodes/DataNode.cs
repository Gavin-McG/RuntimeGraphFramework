using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class DataNode : RuntimeNode
    {
        [SerializeField] public RuntimeNode nextNode;
        [SerializeField] public InputPortReference dataInput;
    }
}