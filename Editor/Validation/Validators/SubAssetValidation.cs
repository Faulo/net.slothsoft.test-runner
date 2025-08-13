using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class SubAssetValidation {
        public static bool VALIDATE_SUB_ASSETS = false;

        [Validate]
        public static void ValidateSubAssets(UnityObject asset, IAssetValidator validator) {
            if (!VALIDATE_SUB_ASSETS) {
                return;
            }

            if (asset is GameObject or SceneAsset) {
                return;
            }

            if (validator.CurrentAssetPath is not string assetPath) {
                return;
            }

            var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var subAsset in subAssets) {
                if (subAsset == asset || subAsset is GameObject or Component) {
                    continue;
                }

                validator.ValidateAsset(subAsset);
            }
        }
    }
}