using System;
using NSubstitute;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor;
using UnityEngine;

namespace Slothsoft.TestRunner.Tests.PlayMode.AssetValidators {
    [TestFixture]
    [TestOf(typeof(GameObjectValidation))]
    sealed class GameObjectValidationTests {
        [TestCase(typeof(Transform))]
        [TestCase(typeof(SkinnedMeshRenderer))]
        public void GivenSingleGameObject_WhenValidateGameObjectHierarchy_ThenValidateComponents(Type type) {
            var validator = Substitute.For<IAssetValidator>();

            GameObject obj = new();
            var component = GetOrAdd(obj, type);

            GameObjectValidation.ValidateGameObjectHierarchy(obj, validator);

            validator.Received(1).ValidateAsset(component);
        }

        [TestCase(typeof(Transform))]
        [TestCase(typeof(SkinnedMeshRenderer))]
        public void GivenGameObjectWithChild_WhenValidateGameObjectHierarchy_ThenValidateChildren(Type type) {
            var validator = Substitute.For<IAssetValidator>();

            GameObject parent = new();
            GameObject child = new();
            child.transform.parent = parent.transform;
            var component = GetOrAdd(child, type);

            GameObjectValidation.ValidateGameObjectHierarchy(parent, validator);

            validator.Received(1).ValidateAsset(component);
        }

        Component GetOrAdd(GameObject obj, Type type) {
            return obj.TryGetComponent(type, out var component)
                ? component
                : obj.AddComponent(type);
        }
    }
}
