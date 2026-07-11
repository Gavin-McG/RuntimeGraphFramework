using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IQueryContext : IVariableContext
    {
        Hash128 QueryID { get; }
    }
}