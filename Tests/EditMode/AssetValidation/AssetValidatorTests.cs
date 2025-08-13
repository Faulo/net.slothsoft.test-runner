using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using Slothsoft.TestRunner.Editor;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Tests.EditMode.AssetValidation {
    [TestFixture]
    [TestOf(typeof(SerializedAssetValidation))]
    internal class AssetValidatorTests {
        [Test]
        public void GivenNewMaterial_WhenValidate_ThenPass() {
            using AssetValidator sut = new();
            sut.ValidateAsset(new Material(Shader.Find("Diffuse")));
        }

        [Test]
        public void GivenBrokenPrefabAsset_WhenValidateThenAssert_ThenFail() {
            BrokenPrefabAsset asset = Activator.CreateInstance(typeof(BrokenPrefabAsset), true) as BrokenPrefabAsset;

            using AssetValidator sut = new();
            sut.ValidateAsset(asset);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"Prefab MISSING BrokenPrefabAsset REFERENCE is broken!"),
                () => sut.AssertFailNow()
            );
        }

        [Test]
        public void GivenBrokenPrefabAsset_WhenFailImmediately_ThenFail() {
            BrokenPrefabAsset asset = Activator.CreateInstance(typeof(BrokenPrefabAsset), true) as BrokenPrefabAsset;

            using AssetValidator sut = new() {
                FailImmediately = true
            };

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .Contains($"Prefab MISSING BrokenPrefabAsset REFERENCE is broken!"),
                () => sut.ValidateAsset(asset)
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GivenAssertThat_WhenFail_ThenDisplayExpectedAndActual(bool addMessage) {
            string message = "Test.";

            List<string> expectedLines = new();
            if (addMessage) {
                expectedLines.Add(message);
            }

            expectedLines.Add(TextMessageWriter.Pfx_Expected + "not <empty>");
            expectedLines.Add(TextMessageWriter.Pfx_Actual + "<empty>");
            expectedLines.Add(string.Empty);

            string expected = string.Join(Environment.NewLine, expectedLines);

            using AssetValidator sut = new();

            if (addMessage) {
                sut.AssertThat(Array.Empty<string>(), Is.Not.Empty, message);
            } else {
                sut.AssertThat(Array.Empty<string>(), Is.Not.Empty);
            }

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                    .And
                    .Message
                    .EqualTo(expected),
                () => sut.AssertFailNow()
            );
        }

        [TestCase("Packages/package/Asset.asset", false)]
        [TestCase("Packages/package/Tests/Test.asset", false)]
        [TestCase("Packages/package/DEPRECATED.asset", true)]
        [TestCase("Packages/package/WIP.asset", false)]
        public void GivenDeprecatedAsset_WhenAssertAssetPath_ThenFail(string assetPath, bool isError) {
            using AssetValidator sut = new();

            sut.AssertAssetPath(assetPath, "Failed to validate assetPath.");

            if (isError) {
                Assert.Throws(
                    new ExceptionTypeConstraint(typeof(AssertionException)),
                    () => sut.AssertFailNow()
                );
            } else {
                sut.AssertFailNow();
            }
        }

        [TestCase(
            "Packages/" + AssemblyInfo.ID + "/Tests/Assets/Test_PrefabWithBrokenRenderer.prefab",
            "SkinnedMeshRenderer 'Test_PrefabWithBrokenRenderer' references a missing Material in property 'm_Materials.Array.data[0]'!"
        )]
        [TestCase(
            "Packages/" + AssemblyInfo.ID + "/Tests/Assets/Test_PrefabWithBrokenRenderer.prefab",
            "SkinnedMeshRenderer 'Test_PrefabWithBrokenRenderer' references a missing Mesh in property 'm_Mesh'!"
        )]
        public void GivenBrokenAssetWhenValidate_ThenExpectError(string assetPath, string message) {
            using AssetValidator sut = new();

            sut.ValidateAsset(assetPath);

            Assert.Throws(
                new ExceptionTypeConstraint(typeof(AssertionException))
                .And
                .Message
                .Contains(message),
                sut.AssertFailNow
            );
        }

        private sealed class StubObject : ScriptableObject { }
        private sealed class SomeObject : ScriptableObject { }

        [TestCase(typeof(StubObject), "A", "StubObject 'A'")]
        [TestCase(typeof(SomeObject), "B", "SomeObject 'B'")]
        public void GivenScriptableObject_WhenGetName_ThenReturnName(Type type, string name, string expected) {
            ScriptableObject obj = ScriptableObject.CreateInstance(type);
            obj.name = name;

            using AssetValidator sut = new();

            string actual = sut.GetName(obj);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(false, "NULL REFERENCE")]
        [TestCase(true, "MISSING StubObject REFERENCE")]
        public void GivenMissingObject_WhenGetName_ThenReturnUnknown(bool destroy, string expected) {
            ScriptableObject obj = default;

            if (destroy) {
                obj = ScriptableObject.CreateInstance<StubObject>();
                UnityObject.DestroyImmediate(obj);
            }

            using AssetValidator sut = new();

            string actual = sut.GetName(obj);

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