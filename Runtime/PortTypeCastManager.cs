using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RuntimeGraphFramework
{
    public static class PortTypeCastManager
    {
        internal readonly struct LookupKey : IEquatable<LookupKey>
        {
            public readonly Type OutputType;
            public readonly Type InputType;
            public readonly Type GraphType;

            public LookupKey(Type outputType, Type inputType, Type graphType)
            {
                OutputType = outputType;
                InputType = inputType;
                GraphType = graphType;
            }

            public bool Equals(LookupKey other)
            {
                return InputType == other.InputType && 
                       OutputType == other.OutputType && 
                       GraphType == other.GraphType;
            }

            public override int GetHashCode()
            {
                return InputType.GetHashCode() ^ OutputType.GetHashCode() ^ GraphType.GetHashCode();
            }
        }
        
        private static readonly Dictionary<LookupKey, Func<object, object>> _casts = new();
        
        private static bool Validate(MethodInfo method, Type outputType, Type inputType)
        {
            // Ensure that method is static
            if (!method.IsStatic)
            {
                Debug.LogError($"Cannot register Port Type cast: {method.Name} is not static");
                return false;
            }

            // Check for parameter count
            var parameters = method.GetParameters();
            if (parameters.Length != 1)
            {
                Debug.LogError($"Cannot register Port Type cast: {method.Name} must have exactly 1 parameter");
                return false;
            }

            // Check parameter/return types
            if (parameters[0].ParameterType != outputType)
            {
                Debug.LogError($"Cannot register Port Type cast: {method.Name} parameters must be of type {outputType}");
                return false;
            }

            if (method.ReturnType != inputType)
            {
                Debug.LogError($"Cannot register Port Type cast: {method.Name} return must be of type {inputType}");
                return false;
            }
            
            return true;
        }

        public static void Register<TGraph>(Type outputType, Type inputType, Func<object, object> cast)
        {
            // Validate method
            if (!Validate(cast.GetMethodInfo(), outputType, inputType)) return;
            
            var key = new LookupKey(
                outputType, 
                inputType, 
                typeof(TGraph));

            if (!_casts.TryAdd(key, cast))
            {
                Debug.LogError($"Duplicate Type cast registered for {typeof(TGraph).Name}: {outputType.Name} to {inputType.Name}");
            }
        }

        public static bool CanTypesCast<TGraph>(Type outputType, Type inputType)
        {
            if (inputType.IsAssignableFrom(outputType)) return true;
            
            var key = new LookupKey(
                outputType, 
                inputType, 
                typeof(TGraph));
            
            return _casts.ContainsKey(key);
        }
       
        public static TInput GetCastedValue<TOutput, TInput, TGraph>(TOutput output)
        {
            if (typeof(TInput).IsAssignableFrom(typeof(TOutput)))
                return (TInput)(object)output;

            var key = new LookupKey(
                typeof(TOutput),
                typeof(TInput),
                typeof(TGraph));

            if (!_casts.TryGetValue(key, out var del))
            {
                throw new InvalidOperationException($"No port cast exists from {key.OutputType} to {key.InputType} for graph {key.GraphType}.");
            }

            return (TInput)del(output);
        }
    }
}