using UnityEngine;

namespace RuntimeGraphFramework
{
    public interface IQueryContext : IParameterContext
    {
        Hash128 QueryID { get; }
    }
}