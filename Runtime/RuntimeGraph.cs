using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public class RuntimeGraph : ScriptableObject
    {
        [SerializeField] public Hash128 graphID;
        [SerializeField] public Dictionary<string, RuntimeVariable> variables;
    }
}