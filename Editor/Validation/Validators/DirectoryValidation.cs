using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using System.IO;
using NUnit.Framework;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class DirectoryValidation {
        [Validate(includeTestAssets = true)]
        public static void PreventEmptyDirectories(DefaultAsset asset, IAssetValidator validator) {
            if (validator.CurrentAssetPath is not string assetPath) {
                return;
            }

            if (Directory.Exists(assetPath)) {
                validator.AssertThat(new DirectoryInfo(assetPath).EnumerateFileSystemInfos(), Is.Not.Empty, $"Directory '{assetPath}' must not be empty!");
            }
        }
    }
}
