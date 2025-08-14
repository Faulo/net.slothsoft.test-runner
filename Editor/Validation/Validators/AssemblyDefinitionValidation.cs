using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class AssemblyDefinitionValidation {
        [Serializable]
        sealed class AssemblyDefinitionData {
            [SerializeField]
            internal string[] references;
            [SerializeField]
            internal VersionDefineData[] versionDefines;
        }

        [Serializable]
        sealed class VersionDefineData {
            [SerializeField]
            internal string name;
        }

        [Validate]
        public static void CheckReferencedAssemblies(AssemblyDefinitionAsset asset, IAssetValidator validator) {
            var pathExceptions = asset
                .GetReferencesPackagePaths()
                .ToList();

            foreach (string path in asset.GetReferencesAssemblies()) {
                if (pathExceptions.Any(path.StartsWith)) {
                    continue;
                }

                validator.AssertAssetPath(path, $"{validator.GetName(asset)} references assembly '{path}', but that path is not part of its dependencies.");
            }
        }

        internal static IEnumerable<string> GetReferencesAssemblies(this AssemblyDefinitionAsset asset) {
            var data = JsonUtility.FromJson<AssemblyDefinitionData>(asset.text);
            if (data is { references: string[] references }) {
                foreach (string reference in references) {
                    if (!string.IsNullOrEmpty(reference)) {
                        if (IsGuid(reference)) {
                            string path = AssetDatabase.GUIDToAssetPath(reference);
                            if (!string.IsNullOrEmpty(path)) {
                                yield return path;
                            }
                        } else {
                            foreach (string assetPath in AssetDatabase.FindAssets($"t:{nameof(AssemblyDefinitionAsset)} {reference}").Select(AssetDatabase.GUIDToAssetPath)) {
                                if (!string.IsNullOrEmpty(assetPath)) {
                                    if (AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath) is { name: string assetName }) {
                                        if (assetName.Equals(reference)) {
                                            yield return assetPath;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static IEnumerable<string> GetReferencesPackagePaths(this AssemblyDefinitionAsset asset) {
            var data = JsonUtility.FromJson<AssemblyDefinitionData>(asset.text);
            if (data is { versionDefines: VersionDefineData[] versions }) {
                return versions
                    .Select(v => v.name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Select(n => $"Packages/{n}");
            }

            return Enumerable.Empty<string>();
        }

        static bool IsGuid(string input) {
            return Guid.TryParse(input, out _);
        }
    }
}