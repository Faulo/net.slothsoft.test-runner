using System.Collections.Generic;
using UnityEditor.PackageManager;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    interface IPackageResolver {
        PackageInfo GetPackageInfo(string packageId);
        PackageInfo GetPackageInfoForAsset(string assetPath);
        IReadOnlyList<string> GetDependentPackagePaths(string assetPath);
    }
}
