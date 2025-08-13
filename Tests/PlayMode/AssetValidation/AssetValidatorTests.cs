using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Slothsoft.TestRunner.Editor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Tests.PlayMode.AssetValidation {
    [TestFixture]
    [TestOf(typeof(SerializedAssetValidation))]
    internal class AssetValidatorTests {
        [TestCase(0, "A", "GameObject 'A'")]
        [TestCase(1, "B", "GameObject 'BB' > 'B'")]
        [TestCase(2, "C", "GameObject 'CCCC' > 'CC' > 'C'")]
        public void GivenGameObject_WhenGetName_ThenReturnName(int parentCount, string name, string expected) {
            GameObject obj = CreateGameObject(name);

            GameObject current = obj;
            for (int i = 0; i < parentCount; i++) {
                name += name;
                GameObject parent = CreateGameObject(name);
                current.transform.parent = parent.transform;
                current = parent;
            }

            using AssetValidator sut = new();

            string actual = sut.GetName(obj);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, "A", "Transform 'A'")]
        [TestCase(1, "B", "Transform 'BB' > 'B'")]
        [TestCase(2, "C", "Transform 'CCCC' > 'CC' > 'C'")]
        public void GivenComponent_WhenGetName_ThenReturnName(int parentCount, string name, string expected) {
            GameObject obj = CreateGameObject(name);

            GameObject current = obj;
            for (int i = 0; i < parentCount; i++) {
                name += name;
                GameObject parent = CreateGameObject(name);
                current.transform.parent = parent.transform;
                current = parent;
            }

            using AssetValidator sut = new();

            string actual = sut.GetName(obj.transform);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private readonly List<GameObject> _objects = new();

        private GameObject CreateGameObject(string name) {
            GameObject obj = new(name);
            _objects.Add(obj);
            return obj;
        }

        [TearDown]
        public void TearDownObjects() {
            foreach (GameObject obj in _objects) {
                if (obj) {
                    UnityObject.Destroy(obj);
                }
            }

            _objects.Clear();
        }
    }
}