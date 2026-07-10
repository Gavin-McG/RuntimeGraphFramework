using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IDialogueContext : IParameterContext
    {
        GameObject gameObject { get; }
        
        Hash128 QueryID { get; }
    }
}