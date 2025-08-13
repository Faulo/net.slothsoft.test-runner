using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Slothsoft.TestRunner.Editor {
    [TestMustExpectAllLogs(false)]
    public abstract class AssetValidationBase<T> where T : IAssetSource, new() {
        public static bool UnloadUnusedAssets = false;
        public static bool FailImmediately = false;

        public static IEnumerable<string> AllAssetPaths {
            get {
                SortedSet<string> allAssetPaths = new(new T().GetAssetPaths(), StringComparer.InvariantCultureIgnoreCase);

                if (allAssetPaths.Count == 0) {
                    allAssetPaths.Add(string.Empty);
                }

                return allAssetPaths;
            }
        }
        readonly IPackageResolver allResolver = new PackageResolver(false);
        readonly IPackageResolver directResolver = new PackageResolver(true);

#if ULISSES_CORE_LOGGING
        [OneTimeSetUp]
        public void SetUpLog() {
            Ulisses.Core.Logging.Log.SetGlobalLogLevel(Ulisses.Core.Logging.ELogTypes.ErrorOrWorse);
        }

        [OneTimeTearDown]
        public void TearDownLog() {
            Ulisses.Core.Logging.Log.ResetAllLogLevels();
        }
#endif

        [UnityTest]
        public IEnumerator UploadAssetsToCacheServer() {
            if (!AssetDatabase.IsConnectedToCacheServer()) {
                Assert.Pass("Not connected to a cache server, skipping upload.");
                yield break;
            }

            yield return RequestAndWait_Co(UploadArtifactsToCacheServer);
        }

        [UnityTest]
        public IEnumerator UploadShadersToCacheServer() {
            if (!AssetDatabase.IsConnectedToCacheServer()) {
                Assert.Pass("Not connected to a cache server, skipping upload.");
                yield break;
            }

            if (AssetValidationHelper.HasUploadedShadersRecently) {
                Assert.Ignore("Uploaded shaders recently, skipping upload.");
                yield break;
            }

            AssetValidationHelper.HasUploadedShadersRecently = true;

            yield return RequestAndWait_Co(CacheServer.UploadShaderCache);
        }

        static IEnumerator RequestAndWait_Co(Action request) {
            while (Progress.GetCount() > 0) {
                yield return null;
            }

            request();

            while (Progress.GetCount() > 0) {
                yield return null;
            }
        }

        static void UploadArtifactsToCacheServer() {
            var assets = AllAssetPaths
                .Select(AssetDatabase.AssetPathToGUID)
                .Select(guid => new GUID(guid))
                .ToArray();

            if (assets.Length > 0) {
                CacheServer.UploadArtifacts(assets, uploadAllRevisions: true);
            }
        }

        [UnityTearDown]
        public IEnumerator WaitForUnloadUnusedAssets() {
            if (UnloadUnusedAssets) {
                var operation = Resources.UnloadUnusedAssets();
                while (!operation.isDone) {
                    yield return null;
                }
            }
        }

        static bool MayHaveIndirectDependencies(string assetPath) {
            return assetPath.StartsWith("Assets")
                || extensionsWithIndirectDependencies.Any(ext => assetPath.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase));
        }

        static readonly string[] extensionsWithIndirectDependencies = new[] {
            ".prefab",
            ".fbx",
            ".unity",
            ".asmdef"
        };

        [Test]
        [Timeout(1800000)] // 30min
        public void Validate([ValueSource(nameof(AllAssetPaths))] string assetPath) {
            if (string.IsNullOrEmpty(assetPath)) {
                Assert.Ignore("No assets to validate.");
                return;
            }

            if (!File.Exists(assetPath) && !Directory.Exists(assetPath)) {
                Assert.Inconclusive($"Skipping assset '{assetPath}' because it stopped existing. Reload the Test Runner window or restart Unity to refresh the asset list.");
                return;
            }

            var resolver = MayHaveIndirectDependencies(assetPath)
                ? allResolver
                : directResolver;

            using AssetValidator validator = new() {
                FailImmediately = FailImmediately,
                ValidAssetPaths = resolver.GetDependentPackagePaths(assetPath),
                IsInTests = AssetUtils.IsTestAsset(assetPath),
            };

            validator.ValidateAsset(assetPath);

            validator.AssertFailNow();
        }
    }
}
