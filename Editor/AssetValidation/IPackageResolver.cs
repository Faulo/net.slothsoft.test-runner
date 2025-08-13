using System.Collections.Generic;
using UnityEditor.PackageManager;

namespace Slothsoft.TestRunner.Editor {
    interface IPackageResolver {
        public PackageInfo GetPackageInfo(string packageId);
        public PackageInfo GetPackageInfoForAsset(string assetPath);
        public IReadOnlyList<string> GetDependentPackagePaths(string assetPath);
    }
}
