using UnityEditor;
using UnityEngine;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    static class PrefabValidation {
        [Validate]
        public static void ValidateHasManagedReferencesWithMissingTypes(GameObject prefab, IAssetValidator validator) {
            if (PrefabUtility.IsPartOfPrefabAsset(prefab)) {
                validator.AssertFalse(
                    PrefabUtility.HasManagedReferencesWithMissingTypes(prefab),
                    $"Prefab {validator.GetName(prefab)} has managed references with missing types!"
                );
            }
        }
    }
}