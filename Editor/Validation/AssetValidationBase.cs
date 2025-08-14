using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using UnityEngine.TestTools;

namespace Slothsoft.TestRunner.Editor.Validation {
    [TestMustExpectAllLogs(false)]
    public abstract class AssetValidationBase<T> where T : IAssetSource, new() {
        public static IEnumerable<string> allAssetPaths => AssetUtils.SortAndAddEmpty(new T().GetAssetPaths());

        readonly IPackageResolver allResolver = new PackageResolver(false);
        readonly IPackageResolver directResolver = new PackageResolver(true);

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
        public void Validate([ValueSource(nameof(allAssetPaths))] string assetPath) {
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
                validAssetPaths = resolver.GetDependentPackagePaths(assetPath),
            };

            validator.ValidateAsset(assetPath);

            validator.AssertFailNow();
        }
    }
}
