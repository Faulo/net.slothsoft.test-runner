using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor {
    public static class ReflectionUtils {
        /// <summary>
        /// Returns all methods in all loaded assemblies that are decorated with the specified attribute <typeparamref name="T"/>.
        /// This method uses reflection across all types and is significantly slower than the TypeCache-based alternative.
        /// </summary>
        /// <typeparam name="T">The attribute type to search for.</typeparam>
        /// <param name="flags">The binding flags used to select methods (e.g., Public, NonPublic, Static).</param>
        /// <returns>An enumerable of tuples containing the method info and the corresponding attribute instance.</returns>
        [Obsolete("Use FindMethodsWithAttribute<T>() without BindingFlags instead. This method is slower and less efficient.")]
        public static IEnumerable<(MethodInfo method, T attribute)> FindMethodsWithAttribute<T>(BindingFlags flags) where T : Attribute {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(flags))
                .SelectMany(method => method.GetCustomAttributes<T>().Select(attribute => (method, attribute)));
        }

        /// <summary>
        /// Returns all methods in the current Unity Editor domain that are decorated with the specified attribute <typeparamref name="T"/>.
        /// Uses Unity's internal TypeCache for efficient lookup (Editor only).
        /// </summary>
        /// <typeparam name="T">The attribute type to search for.</typeparam>
        /// <returns>An enumerable of tuples containing the method info and the corresponding attribute instance.</returns>
        public static IEnumerable<(MethodInfo method, T attribute)> FindMethodsWithAttribute<T>() where T : Attribute {
            return TypeCache
                .GetMethodsWithAttribute<T>()
                .SelectMany(method => method.GetCustomAttributes<T>().Select(attribute => (method, attribute)));
        }
    }
}