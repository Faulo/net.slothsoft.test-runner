using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Slothsoft.TestRunner.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Tests.EditMode.AssetValidation {
    [TestFixture]
    [TestOf(typeof(SerializedAssetValidation))]
    [TestMustExpectAllLogs(false)]
    internal class AssetValidationBaseTests {
        internal sealed class StubAsset : ScriptableObject {
            [SerializeField]
            internal StubAsset AssetField;
            [SerializeField]
            internal Material[] MaterialField;
        }

        private AssetValidator sut;

        [SetUp]
        public void SetUpSuT() {
            sut = new();
        }

        [TearDown]
        public void TearDownSuT() {
            sut.Dispose();
        }

        private readonly List<UnityObject> runtimeObjects = new();

        [TearDown]
        public void TearDownRuntimeObjects() {
            foreach (UnityObject obj in runtimeObjects) {
                if (obj) {
                    UnityObject.Destroy(obj);

                }
            }

            runtimeObjects.Clear();
        }

        private static StubAsset CreateAsset(string name) {
            StubAsset asset = ScriptableObject.CreateInstance<StubAsset>();
            asset.name = name;
            return asset;
        }

        [Test]
        public void GivenAssetWithoutReference_WhenAssert_ThenPass() {
            StubAsset asset = CreateAsset("test");

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);
            sut.AssertFailNow();
        }

        [Test]
        public void GivenAssetWithReference_WhenAssert_ThenPass() {
            StubAsset asset = CreateAsset("test");
            asset.AssetField = CreateAsset("child");

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);

            sut.AssertFailNow();
        }

        [Test]
        public void GivenAssetWithMissingReference_WhenAssert_ThenFail() {
            StubAsset asset = CreateAsset("test");
            asset.AssetField = CreateAsset("child");
            UnityObject.DestroyImmediate(asset.AssetField);

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(StubAsset).Name} in property '{nameof(StubAsset.AssetField)}'!"),
                () => sut.AssertFailNow()
            );
        }

        [Test]
        public void GivenAssetWithMissingMaterial_WhenAssert_ThenFail() {
            StubAsset asset = CreateAsset("test");
            asset.MaterialField = new[] { new Material(Shader.Find("Diffuse")) };
            UnityObject.DestroyImmediate(asset.MaterialField[0]);

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(Material).Name} in property '{nameof(StubAsset.MaterialField)}.Array.data[0]'!"),
                () => sut.AssertFailNow()
            );
        }

        [Test]
        public void GivenSerializedPropertyWithoutReference_WhenAssert_ThenPass() {
            StubAsset asset = CreateAsset("test");

            SerializedProperty property = new SerializedObject(asset).FindProperty(nameof(StubAsset.AssetField));

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);
            sut.AssertFailNow();
        }

        [Test]
        public void GivenSerializedPropertyWithReference_WhenAssert_ThenPass() {
            StubAsset asset = CreateAsset("test");
            asset.AssetField = CreateAsset("child");

            SerializedProperty property = new SerializedObject(asset).FindProperty(nameof(StubAsset.AssetField));

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);

            sut.AssertFailNow();
        }

        [Test]
        public void GivenSerializedPropertyWithMissingReference_WhenAssert_ThenFail() {
            StubAsset asset = CreateAsset("test");
            asset.AssetField = CreateAsset("child");
            UnityObject.DestroyImmediate(asset.AssetField);

            SerializedProperty property = new SerializedObject(asset).FindProperty(nameof(StubAsset.AssetField));

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(StubAsset).Name} in property '{nameof(StubAsset.AssetField)}'!"),
                () => sut.AssertFailNow()
            );
        }

        [Test]
        public void GivenSerializedPropertyWithMissingMaterial_WhenAssert_ThenFail() {
            StubAsset asset = CreateAsset("test");
            asset.MaterialField = new[] { new Material(Shader.Find("Diffuse")) };
            UnityObject.DestroyImmediate(asset.MaterialField[0]);

            SerializedProperty property = new SerializedObject(asset).FindProperty(nameof(StubAsset.MaterialField)).GetArrayElementAtIndex(0);

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(Material).Name} in property '{nameof(StubAsset.MaterialField)}.Array.data[0]'!"),
                () => sut.AssertFailNow()
            );
        }
    }
}