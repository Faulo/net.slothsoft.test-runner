using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class GameObjectValidation {
        [Validate]
        public static void ValidateGameObjectHierarchy(GameObject prefab, IAssetValidator validator) {
            var children = prefab.GetComponentsInChildren<Transform>(true);
            foreach (var child in children) {
                ValidatePrefabComponents(prefab, child.gameObject, validator);
            }
        }

        internal static void ValidatePrefabComponents(GameObject prefab, GameObject child, IAssetValidator validator) {
            ValidatePrefabVariants(prefab, child, validator);

            if (PrefabUtility.IsPartOfPrefabInstance(child)) {
                validator.AssertFalse(
                    PrefabUtility.IsPrefabAssetMissing(child),
                    $"Prefab {validator.GetName(prefab)} references a missing prefab in {validator.GetName(child)} according to PrefabUtility.IsPrefabAssetMissing!"
                );
            }

            var components = child.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++) {
                var component = components[i];
                validator.AssertTrue(
                    component,
                    $"Prefab {validator.GetName(prefab)} references a missing script in {validator.GetName(child)}!"
                );

                validator.ValidateAsset(component);
            }
        }

        internal static void ValidatePrefabVariants(GameObject prefab, GameObject child, IAssetValidator validator) {
            validator.AssertFalse(
                Regex.IsMatch(child.name, "\\(Missing Prefab with guid: [a-z0-9]{32}\\)"),
                $"Prefab {validator.GetName(prefab)} references a missing prefab according to its name, '{child.name}'!"
            );

            if (PrefabUtility.IsOutermostPrefabInstanceRoot(child)) {
                var childPrefab = PrefabUtility.GetCorrespondingObjectFromSource(child);

                if (!childPrefab) {
                    validator.AssertFail($"Prefab {validator.GetName(prefab)} references a missing prefab {validator.GetName(child)}!");
                } else {
                    string path = AssetDatabase.GetAssetPath(childPrefab);
                    if (string.IsNullOrEmpty(path)) {
                        validator.AssertFail($"Prefab {validator.GetName(prefab)} references a prefab {validator.GetName(childPrefab)} without an asset path!");
                    } else {
                        validator.AssertAssetPath(
                            path,
                            $"Prefab {validator.GetName(prefab)} references a prefab {validator.GetName(childPrefab)} NOT residing in any of its dependent packages!{Environment.NewLine}  Either move the prefab to the package, remove the reference to it, or update the package's dependencies to include the prefab.{Environment.NewLine}  The offending prefab is: {path}"
                        );
                    }
                }
            }
        }
    }
}