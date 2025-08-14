using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    [TestFixture]
    [TestOf(typeof(SerializedAssetValidation))]
    sealed class AssetValidatorTests {
        readonly TestObjectStore _store = new();

        [TestCase(0, "A", "GameObject 'A'")]
        [TestCase(1, "B", "GameObject 'BB' > 'B'")]
        [TestCase(2, "C", "GameObject 'CCCC' > 'CC' > 'C'")]
        public void GivenGameObject_WhenGetName_ThenReturnName(int parentCount, string name, string expected) {
            var obj = _store.CreateGameObject(name);

            var current = obj;
            for (int i = 0; i < parentCount; i++) {
                name += name;
                var parent = _store.CreateGameObject(name);
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
            var obj = _store.CreateGameObject(name);

            var current = obj;
            for (int i = 0; i < parentCount; i++) {
                name += name;
                var parent = _store.CreateGameObject(name);
                current.transform.parent = parent.transform;
                current = parent;
            }

            using AssetValidator sut = new();

            string actual = sut.GetName(obj.transform);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TearDown]
        public void TearDownObjects() {
            _store.Dispose();
        }

        [Test]
        public void GivenMissingMesh_WhenValidate_ThenExpectError() {
            using var test = new TestGameObject<SkinnedMeshRenderer>();

            var mesh = _store.CreateUnityObject<Mesh>();
            test.sut.sharedMesh = mesh;
            UnityObject.DestroyImmediate(mesh);

            using AssetValidator sut = new();

            sut.ValidateAsset(test.gameObject);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                .And
                .Message
                .Contains("SkinnedMeshRenderer 'TestGameObject<SkinnedMeshRenderer>' references a missing Mesh in property 'm_Mesh'!"),
                sut.AssertFailNow
            );
        }

        [Test]
        public void GivenMissingMaterial_WhenValidate_ThenExpectError() {
            using var test = new TestGameObject<SkinnedMeshRenderer>();

            var material = _store.CreateMaterial();
            test.sut.sharedMaterials = new[] { material };
            UnityObject.DestroyImmediate(material);

            using AssetValidator sut = new();

            sut.ValidateAsset(test.gameObject);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                .And
                .Message
                .Contains("SkinnedMeshRenderer 'TestGameObject<SkinnedMeshRenderer>' references a missing Material in property 'm_Materials.Array.data[0]'!"),
                sut.AssertFailNow
            );
        }
    }
}