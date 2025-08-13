using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Slothsoft.TestRunner.Editor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Tests.PlayMode.AssetValidation {
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