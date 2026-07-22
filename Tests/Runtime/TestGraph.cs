using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class TestGraph : RuntimeGraph
    {
        [SerializeField] public List<StartNode> nodes;

        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeCasts()
        {
            PortTypeCastManager.Register<float, string, TestGraph>(Cast_FloatToString);
        }

        private static string Cast_FloatToString(float input) => input.ToString();
    }
}
