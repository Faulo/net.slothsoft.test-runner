using System;
using System.Linq;
using Castle.DynamicProxy;
using UnityEngine;

namespace Slothsoft.TestRunner {
    sealed class ComponentProxyGenerator : ProxyGenerator {
        public GameObject gameObject;

        public ComponentProxyGenerator(IProxyGenerator generator) : base(generator.ProxyBuilder) {
        }

        public override object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options, object[] constructorArguments, params IInterceptor[] interceptors) {
            if (classToProxy == null) {
                throw new ArgumentNullException(nameof(classToProxy));
            }

            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            if (!classToProxy.IsClass) {
                throw new ArgumentException("'classToProxy' must be a class", nameof(classToProxy));
            }

            CheckNotGenericTypeDefinition(classToProxy, nameof(classToProxy));
            CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

            var proxyType = CreateClassProxyType(classToProxy, additionalInterfacesToProxy, options);
            var proxyArguments = BuildArgumentListForClassProxy(options, interceptors);
            if (constructorArguments != null && constructorArguments.Length != 0) {
                proxyArguments.AddRange(constructorArguments);
            }

            // this is where the magic happens:
            // instead of calling the proxy constructor, we let Unity add it as a component, then call the constructor anyways to set up the substitution magic.

            // return Activator.CreateInstance(proxyType, proxyArguments.ToArray());
            var component = gameObject.AddComponent(proxyType);

            object[] ctorArgs = proxyArguments
                .ToArray();
            var ctorTypes = ctorArgs
                .Select(obj => obj.GetType())
                .ToArray();

            var ctor = proxyType.GetConstructor(ctorTypes);
            ctor.Invoke(component, ctorArgs);

            return component;
        }
    }
}
