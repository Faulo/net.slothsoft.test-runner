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
    class AssetValidationBaseTests {
        internal sealed class StubAsset : ScriptableObject {
            [SerializeField]
            internal StubAsset assetField;
            [SerializeField]
            internal Material[] materialField;
        }

        AssetValidator sut;

        [SetUp]
        public void SetUpSuT() {
            sut = new();
        }

        [TearDown]
        public void TearDownSuT() {
            sut.Dispose();
        }

        readonly List<UnityObject> runtimeObjects = new();

        [TearDown]
        public void TearDownRuntimeObjects() {
            foreach (var obj in runtimeObjects) {
                if (obj) {
                    UnityObject.Destroy(obj);

                }
            }

            runtimeObjects.Clear();
        }

        static StubAsset CreateAsset(string name) {
            var asset = ScriptableObject.CreateInstance<StubAsset>();
            asset.name = name;
            return asset;
        }

        [Test]
        public void GivenAssetWithoutReference_WhenAssert_ThenPass() {
            var asset = CreateAsset("test");

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);
            sut.AssertFailNow();
        }

        [Test]
        public void GivenAssetWithReference_WhenAssert_ThenPass() {
            var asset = CreateAsset("test");
            asset.assetField = CreateAsset("child");

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);

            sut.AssertFailNow();
        }

        [Test]
        public void GivenAssetWithMissingReference_WhenAssert_ThenFail() {
            var asset = CreateAsset("test");
            asset.assetField = CreateAsset("child");
            UnityObject.DestroyImmediate(asset.assetField);

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(StubAsset).Name} in property '{nameof(StubAsset.assetField)}'!"),
                () => sut.AssertFailNow()
            );
        }

        [Test]
        public void GivenAssetWithMissingMaterial_WhenAssert_ThenFail() {
            var asset = CreateAsset("test");
            asset.materialField = new[] { new Material(Shader.Find("Diffuse")) };
            UnityObject.DestroyImmediate(asset.materialField[0]);

            SerializedAssetValidation.ValidateSerializedProperties(asset, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(Material).Name} in property '{nameof(StubAsset.materialField)}.Array.data[0]'!"),
                () => sut.AssertFailNow()
            );
        }

        [Test]
        public void GivenSerializedPropertyWithoutReference_WhenAssert_ThenPass() {
            var asset = CreateAsset("test");

            var property = new SerializedObject(asset).FindProperty(nameof(StubAsset.assetField));

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);
            sut.AssertFailNow();
        }

        [Test]
        public void GivenSerializedPropertyWithReference_WhenAssert_ThenPass() {
            var asset = CreateAsset("test");
            asset.assetField = CreateAsset("child");

            var property = new SerializedObject(asset).FindProperty(nameof(StubAsset.assetField));

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);

            sut.AssertFailNow();
        }

        [Test]
        public void GivenSerializedPropertyWithMissingReference_WhenAssert_ThenFail() {
            var asset = CreateAsset("test");
            asset.assetField = CreateAsset("child");
            UnityObject.DestroyImmediate(asset.assetField);

            var property = new SerializedObject(asset).FindProperty(nameof(StubAsset.assetField));

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(StubAsset).Name} in property '{nameof(StubAsset.assetField)}'!"),
                () => sut.AssertFailNow()
            );
        }

        [Test]
        public void GivenSerializedPropertyWithMissingMaterial_WhenAssert_ThenFail() {
            var asset = CreateAsset("test");
            asset.materialField = new[] { new Material(Shader.Find("Diffuse")) };
            UnityObject.DestroyImmediate(asset.materialField[0]);

            var property = new SerializedObject(asset).FindProperty(nameof(StubAsset.materialField)).GetArrayElementAtIndex(0);

            SerializedAssetValidation.ValidateSerializedProperty(property, sut);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"{typeof(StubAsset).Name} '{asset.name}' references a missing {typeof(Material).Name} in property '{nameof(StubAsset.materialField)}.Array.data[0]'!"),
                () => sut.AssertFailNow()
            );
        }
    }
}