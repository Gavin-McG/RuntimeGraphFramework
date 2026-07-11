using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
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

        private static void Validate(MethodInfo method, PortTypeCastAttribute attr)
        {
            if (!method.IsStatic)
                throw new ArgumentException($"{method.Name} is not static");

            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                throw new ArgumentException($"{method.Name} must have exactly 1 parameter");

            if (parameters[0].ParameterType != attr.outputType)
                throw new ArgumentException($"{method.Name} parameters must be of type {attr.outputType}");

            if (method.ReturnType != attr.inputType)
                throw new ArgumentException($"{method.Name} return must be of type {attr.inputType}");
        }
        
        private static Func<object, object> Compile(MethodInfo method)
        {
            var input = Expression.Parameter(typeof(object));

            var call = Expression.Call(
                method,
                Expression.Convert(input, method.GetParameters()[0].ParameterType));

            var body = Expression.Convert(call, typeof(object));

            return Expression
                .Lambda<Func<object, object>>(body, input)
                .Compile();
        }
        
        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _casts.Clear();

            foreach (MethodInfo method in TypeCache.GetMethodsWithAttribute<PortTypeCastAttribute>())
            {
                foreach (PortTypeCastAttribute attr in method.GetCustomAttributes<PortTypeCastAttribute>())
                {
                    Validate(method, attr);
                    
                    foreach (Type graphType in attr.graphTypes)
                    {
                        var newCastKey = new LookupKey
                        (
                            attr.outputType,
                            attr.inputType,
                            graphType
                        );

                        if (_casts.ContainsKey(newCastKey))
                        {
                            Debug.LogError($"Duplicate cast key {newCastKey.InputType} {newCastKey.OutputType} {newCastKey.GraphType}");
                            continue;
                        }

                        _casts.Add(newCastKey, Compile(method));
                    }
                }
            }
        }

        public static bool CanTypesCast<TGraph>(Type outputType, Type inputType)
        {
            if (outputType == inputType) return true;
            
            var key = new LookupKey(
                outputType, 
                inputType, 
                typeof(TGraph));
            
            return _casts.ContainsKey(key);
        }
       
        public static TInput GetCastedValue<TOutput, TInput, TGraph>(TOutput output)
        {
            if (typeof(TInput) == typeof(TOutput))
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