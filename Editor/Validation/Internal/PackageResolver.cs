using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    sealed class PackageResolver : IPackageResolver {
        readonly Dictionary<string, List<string>> pathsCache = new();
        readonly bool onlyRetrieveDirectDependencies;

        internal PackageResolver(bool onlyRetrieveDirectDependencies = false) {
            this.onlyRetrieveDirectDependencies = onlyRetrieveDirectDependencies;
        }

        public PackageInfo GetPackageInfo(string packageId) {
            return PackageInfo.FindForPackageName(packageId);
        }

        public PackageInfo GetPackageInfoForAsset(string assetPath) {
            return PackageInfo.FindForAssetPath(assetPath);
        }

        public IReadOnlyList<string> GetDependentPackagePaths(string assetPath) {
            var package = GetPackageInfoForAsset(assetPath);
            bool isPackage = package is not null;

            string packageId = isPackage
                ? package.name
                : string.Empty;

            if (pathsCache.TryGetValue(packageId, out var packages)) {
                return packages;
            }

            return pathsCache[packageId] = (isPackage ? GetDependentPackagePathsForPackage(package) : GetDependentPackagePathsForProject())
                .Append("Resources")
                .Append("Library")
                .Distinct()
                .OrderBy(p => p)
                .ToList();
        }

        IEnumerable<string> GetDependentPackagePathsForPackage(PackageInfo package) {
            var dependencies = onlyRetrieveDirectDependencies
                ? package.dependencies
                : package.resolvedDependencies;

            return dependencies
                .Select(d => $"Packages/{d.name}")
                .Append($"Packages/{package.name}");
        }

        IEnumerable<string> GetDependentPackagePathsForProject() {
            var dependencies = onlyRetrieveDirectDependencies
                ? PackageInfo.GetAllRegisteredPackages().Where(p => p.isDirectDependency)
                : PackageInfo.GetAllRegisteredPackages();

            return dependencies
                .Select(d => $"Packages/{d.name}")
                .Append("Assets");
        }
    }
}
