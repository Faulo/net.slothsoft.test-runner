using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Slothsoft.TestRunner.Editor {
    [Obsolete]
    public abstract class AssetSourceBase : IAssetSource, IPackageSource {
        protected abstract string ValidateAssetsInDirectory { get; }
        protected abstract IEnumerable<string> CheckCircularDependenciesForPackages { get; }
        protected virtual bool ValidateTestAssets => false;
        protected virtual bool IsWIP => false;

        static readonly Regex isLibraryPath = new("Library/PackageCache/([^/]+)@[^/]+/");

        protected virtual bool ShouldValidateAsset(string assetPath) {
            if (assetPath.Contains("/.")) {
                // Unity ignores files and folders starting with '.', so we should, too.
                return false;
            }

            if (assetPath.Contains("~/")) {
                // Unity ignores folders ending with '~', so we should, too.
                return false;
            }

            if (assetPath.Contains(".bundle/")) {
                // Unity treats folders ending with '.bundle' as MacOS assets, so we should ignore them.
                return false;
            }

            if (assetPath.Contains("InitTestScene")) {
                // temp scene created by Unity's Test Runner
                return false;
            }

            if (!ValidateTestAssets && assetPath.Contains("/Tests/")) {
                // test assets don't need to get validated
                return false;
            }

            return IsWIP == AssetUtils.IsWIPAsset(assetPath);
        }

        public IEnumerable<string> GetAssetPaths() {
            if (string.IsNullOrEmpty(ValidateAssetsInDirectory)) {
                return Enumerable.Empty<string>();
            }

            DirectoryInfo directory = new(ValidateAssetsInDirectory);
            string projectRoot = directory.FullName;

            SortedSet<string> allAssetPaths = new(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in directory.EnumerateFiles("*.meta", SearchOption.AllDirectories)) {
                string path = file.FullName;

                path = ValidateAssetsInDirectory + path[projectRoot.Length..^".meta".Length];

                path = path.Replace('\\', '/');

                if (isLibraryPath.Match(path) is { Success: true, Groups: GroupCollection groups, Value: string match }) {
                    path = path.Replace(match, $"Packages/{groups[1].Value}/");
                }

                if (ShouldValidateAsset(path)) {
                    allAssetPaths.Add(path);
                }
            }

            return allAssetPaths;
        }

        public IEnumerable<string> GetPackageIds() {
            return CheckCircularDependenciesForPackages;
        }
    }
}