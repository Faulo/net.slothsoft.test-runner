using System.Linq;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    sealed class AssetUtilsCacheClearer : AssetPostprocessor {
        internal static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            AssetUtils.ClearCache(importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedAssets).Concat(movedFromAssetPaths));
        }
    }
}