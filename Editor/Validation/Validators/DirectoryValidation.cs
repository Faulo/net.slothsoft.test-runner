using System.IO;
using NUnit.Framework;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class DirectoryValidation {
        [Validate(includeTestAssets = true)]
        public static void PreventEmptyDirectories(DefaultAsset _, IAssetValidator validator) {
            if (validator.currentAssetPath is not string assetPath) {
                return;
            }

            if (Directory.Exists(assetPath)) {
                validator.AssertThat(new DirectoryInfo(assetPath).EnumerateFileSystemInfos(), Is.Not.Empty, $"Directory '{assetPath}' must not be empty!");
            }
        }
    }
}
