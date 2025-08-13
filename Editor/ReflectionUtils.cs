using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor {
    public static class ReflectionUtils {
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