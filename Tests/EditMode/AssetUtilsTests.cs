using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Tests.EditMode {
    [TestFixture]
    [TestOf(typeof(AssetUtils))]
    [TestMustExpectAllLogs(false)]
    sealed class AssetUtilsTests {
        [SetUp]
        public void ClearCache() {
            AssetUtils.ClearCache(Enumerable.Empty<string>());
        }

        [TestCase("Assets")]
        [TestCase("Assets")]
        [TestCase("Packages")]
        [TestCase("Packages")]
        [TestCase("Library/PackageCache")]
        [TestCase("Library/PackageCache")]
        public void GivenDirectory_WhenAssetsInDirectory_ThenReturn(string directory) {
            var actual = directory.AssetsInDirectory();

            Assert.That(actual, Is.InstanceOf<List<string>>());
        }

        [TestCase("Assets", false)]
        [TestCase("Assets", false)]
        [TestCase("Assets", true)]
        [TestCase("Assets", true)]
        [TestCase("Packages", false)]
        [TestCase("Packages", false)]
        [TestCase("Packages", true)]
        [TestCase("Packages", true)]
        public void GivenDirectory_WhenDependentAssets_ThenReturn(string directory, bool isRecursive) {
            IEnumerable<string> actual = directory
                .AssetsInDirectory()
                .DependentAssets(isRecursive)
                .ToList();

            Assert.That(actual, Is.InstanceOf<List<string>>());
        }

        interface INotImplementedAnywhere {
        }

        interface INotImplementedAnywhereGeneric<T> {
        }

        [Test]
        public void GivenInterface_WhenFindAssetsOfType_ThenReturnEmpty() {
            string[] actual = AssetUtils.FindAssetsOfType<INotImplementedAnywhere>();

            Assert.That(actual, Is.EqualTo(Array.Empty<string>()));
        }

        [Test]
        public void GivenInterface_WhenFindAndLoadAssetsOfType_ThenReturnEmpty() {
            IEnumerable<INotImplementedAnywhere> actual = AssetUtils.FindAndLoadAssetsOfType<INotImplementedAnywhere>();

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GivenGenericInterface_WhenFindAndLoadAssetsOfType_ThenReturnEmpty() {
            IEnumerable<INotImplementedAnywhereGeneric<UnityObject>> actual = AssetUtils.FindAndLoadAssetsOfType<INotImplementedAnywhereGeneric<UnityObject>>();

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GivenAssemblyDefinitionAsset_WhenFindAndLoadAssetsOfType_ThenReturnSomething() {
            IEnumerable<AssemblyDefinitionAsset> actual = AssetUtils.FindAndLoadAssetsOfType<AssemblyDefinitionAsset>();

            Assert.That(actual, Is.Not.Empty);
        }

        [TestCase("Packages/" + AssemblyInfo.ID + "/Tests/EditMode/AssetUtilsTests.cs")]
        public void GiveMonoScript_WhenFindAssetsOfType_ThenReturnMainAsset(string assetPath) {
            string expected = AssetDatabase.AssetPathToGUID(assetPath);

            Assert.That(expected, Is.Not.Null);

            string[] actual = AssetUtils.FindAssetsOfType<MonoScript>();

            Assert.That(actual, Does.Contain(expected));
        }

        [TestCase("Packages/" + AssemblyInfo.ID + "/Tests/EditMode/AssetUtilsTests.cs")]
        public void GiveMonoScript_WhenFindAndLoadAssetsOfType_ThenReturnMainAsset(string assetPath) {
            var expected = AssetDatabase.LoadMainAssetAtPath(assetPath);

            Assert.That(expected, Is.Not.Null);

            IEnumerable<MonoScript> actual = AssetUtils.FindAndLoadAssetsOfType<MonoScript>();

            Assert.That(actual, Does.Contain(expected));
        }

        [TestCase(true)]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(false)]
        public void GivenAssetSearch_WhenAssetDatabaseChanged_ThenClearCache(bool triggerChange) {
            if (triggerChange) {
                AssetUtilsCacheClearer.OnPostprocessAllAssets(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            }

            Assert.That(AssetUtils.FindAndLoadAssetsOfType<ScriptableObject>(), Is.Not.Empty);
        }

        [TestCase(true)]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(false)]
        public void GivenPrefabSearch_WhenAssetDatabaseChanged_ThenClearCache(bool triggerChange) {
            if (triggerChange) {
                AssetUtilsCacheClearer.OnPostprocessAllAssets(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            }

            Assert.That(AssetUtils.FindAndLoadPrefabsWithComponent<Transform>(), Is.Not.Empty);
        }
    }
}
