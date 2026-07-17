using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class TestGraph : RuntimeGraph
    {
        [SerializeField] public List<StartNode> nodes;
    }
}
