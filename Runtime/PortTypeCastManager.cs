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

        [RuntimeInitializeOnLoadMethod]
        public static void ClearCasts()
        {
            _casts.Clear();
        }
        
        private static bool Validate(MethodInfo method, Type inputType, Type outputType)
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
            if (parameters[0].ParameterType != inputType)
            {
                Debug.LogError($"Cannot register Port Type cast: {method.Name} parameters must be of type {inputType}");
                return false;
            }

            if (method.ReturnType != outputType)
            {
                Debug.LogError($"Cannot register Port Type cast: {method.Name} return must be of type {outputType}");
                return false;
            }
            
            return true;
        }

        public static void Register<TInput, TOutput, TGraph>(Func<TInput, TOutput> cast)
        {
            MethodInfo method = cast.Method;
            if (!Validate(method, typeof(TInput), typeof(TOutput)))
                return;

            var key = new LookupKey(
                typeof(TInput),
                typeof(TOutput),
                typeof(TGraph));

            Func<object, object> wrapper = obj => cast((TInput)obj);

            if (!_casts.TryAdd(key, wrapper))
            {
                Debug.LogError(
                    $"Duplicate type cast registered for {typeof(TGraph).Name}: " +
                    $"{typeof(TInput).Name} -> {typeof(TOutput).Name}");
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
       
        public static bool TryCastValue<TInput, TOutput, TGraph>(TInput input, out TOutput output)
        {
            if (input == null)
            {
                output = default;
                return false;
            }
            
            // Cast using builtin cast
            if (typeof(TOutput).IsAssignableFrom(input.GetType()))
            {
                output = (TOutput)(object)input;
                return true;
            }
            
            // Attempt to cast the value using more specific runtime type
            var runtimeKey = new LookupKey(input.GetType(), typeof(TOutput), typeof(TGraph));
            if (_casts.TryGetValue(runtimeKey, out var runtimeFunc))
            {
                output = (TOutput)runtimeFunc(input);
                return true;
            }

            // Attempt to cast the value using generic parameter
            var genericKey = new LookupKey(typeof(TInput), typeof(TOutput), typeof(TGraph));
            if (_casts.TryGetValue(genericKey, out var genericFunc))
            {
                output = (TOutput)genericFunc(input);
                return true;
            }
            
            Debug.LogError($"No Port cast Registered from {typeof(TInput).Name} to {typeof(TOutput).Name} for Graph type {typeof(TGraph).Name}.");
            output = default;
            return false;
        }
    }
}