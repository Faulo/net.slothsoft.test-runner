using System;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute.Core;
using NSubstitute.Proxies.CastleDynamicProxy;
using UnityEngine;

namespace Slothsoft.TestRunner {
    public static class GameObjectExtensions {
        /// <summary>
        /// Create a mock <see cref="MonoBehaviour"/> that implements <typeparamref name="T"/> and add it to <paramref name="gameObject"/>.
        /// </summary>
        /// <typeparam name="T">The class or interface to mock.</typeparam>
        /// <param name="gameObject">The object to attach the mock to.</param>
        /// <returns>The added <see cref="MonoBehaviour"/>.</returns>
        public static T AddSubstitute<T>(this GameObject gameObject) {
            var substituteFactory = substitutionContext.GetSubstituteFactory();
            var proxyFactory = substituteFactory.GetProxyFactory();
            var defaultProxyGenerator = proxyFactory.GetProxyGenerator();

            var componentProxyGenerator = new ComponentProxyGenerator(defaultProxyGenerator) {
                gameObject = gameObject
            };

            proxyFactory.SetProxyGenerator(componentProxyGenerator);
            try {
                var type = typeof(T);
                var types = type.IsClass
                    ? new[] { type }
                    : new[] { typeof(MonoBehaviour), type };
                object substitute = substituteFactory.Create(types, Array.Empty<object>());
                return (T)substitute;
            } finally {
                proxyFactory.SetProxyGenerator(defaultProxyGenerator);
            }
        }

        static ISubstitutionContext substitutionContext
            => SubstitutionContext.Current
            ?? throw new MissingReferenceException("Missing SubstitutionContext!");

        static SubstituteFactory GetSubstituteFactory(this ISubstitutionContext context)
            => context.SubstituteFactory as SubstituteFactory
            ?? throw new MissingReferenceException($"Missing {typeof(ISubstituteFactory)} of type {typeof(SubstituteFactory)}!");

        static CastleDynamicProxyFactory GetProxyFactory(this SubstituteFactory context)
            => context.Get<CastleDynamicProxyFactory>("_proxyFactory")
            ?? throw new MissingReferenceException($"Missing {typeof(IProxyFactory)} of type {typeof(CastleDynamicProxyFactory)}!");

        static ProxyGenerator GetProxyGenerator(this CastleDynamicProxyFactory context)
            => context.Get<ProxyGenerator>("_proxyGenerator")
            ?? throw new MissingReferenceException($"Missing {typeof(ProxyGenerator)}!");
        static void SetProxyGenerator(this CastleDynamicProxyFactory context, ProxyGenerator proxyGenerator)
            => context.Set("_proxyGenerator", proxyGenerator);

        static T Get<T>(this object obj, string name) where T : class => obj
            .GetFieldInfo(name)
            .GetValue(obj) as T;
        static void Set<T>(this object obj, string name, T value) where T : class => obj
            .GetFieldInfo(name)
            .SetValue(obj, value);
        static FieldInfo GetFieldInfo(this object obj, string name) => obj
            .GetType()
            .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
    }
}