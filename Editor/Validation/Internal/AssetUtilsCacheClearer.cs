using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using System.Linq;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    sealed class AssetUtilsCacheClearer : AssetPostprocessor {
        internal static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            AssetUtils.ClearCache(importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedAssets).Concat(movedFromAssetPaths));
        }
    }
}