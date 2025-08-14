using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using UnityEngine;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.Validation {
    [TestFixture]
    [TestOf(typeof(SerializedAssetValidation))]
    [TestMustExpectAllLogs(false)]
    sealed class AssetValidationBaseTests {
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

        [TestCase("SM_Dagger_Bandit_02", true)]
        [TestCase("SM_Dagger_Bandit_02 (Missing Prefab with guid: 993696b9cad5a7041821bc496ffb7765)", false)]
        public void GivenPrefabWithMissingChildPrefab_WhenAssert_ThenFail(string name, bool shouldPass) {
            GameObject prefab = new("prefab");
            runtimeObjects.Add(prefab);
            GameObject child = new(name);
            runtimeObjects.Add(child);

            GameObjectValidation.ValidatePrefabVariants(prefab, child, sut);

            if (shouldPass) {
                sut.AssertFailNow();
            } else {
                Assert.Throws(
                    new ExceptionTypeConstraint(typeof(AssertionException))
                        .And
                        .Message
                        .Contains($"Prefab GameObject 'prefab' references a missing prefab according to its name, '{name}'!"),
                    () => sut.AssertFailNow()
                );
            }
        }
    }
}