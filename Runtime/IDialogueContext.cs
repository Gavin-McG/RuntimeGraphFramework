using UnityEngine;

namespace BehavioralDialogue
{
    public interface IDialogueContext : IParameterContext
    {
        GameObject gameObject { get; }
        
        Hash128 QueryID { get; }
    }
}