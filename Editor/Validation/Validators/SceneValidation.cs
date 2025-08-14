using System;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class SceneValidation {
        [Validate]
        public static void ValidateScene(SceneAsset scene, IAssetValidator validator) {
            if (validator.CurrentAssetPath is not string assetPath) {
                return;
            }

            if (!validator.CanOpenScene(assetPath)) {
                return;
            }

            try {
                validator.OpenScene(assetPath);
                try {
                    foreach (var obj in validator.CurrentScene.GetRootGameObjects()) {
                        validator.ValidateAsset(obj);
                    }
                } finally {
                    validator.CloseScene();
                }
            } catch (Exception e) {
                validator.AssertFail($"Failed to validate Scene '{scene.name}':" + Environment.NewLine + e);
            }
        }
    }
}