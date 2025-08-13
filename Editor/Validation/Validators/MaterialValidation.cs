using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Slothsoft.TestRunner.Editor {
    static class MaterialValidation {
        [Validate]
        public static void ValidateMaterialVariants(Material material, IAssetValidator validator) {
            if (material.isVariant) {
                if (material.parent) {
                    string path = AssetDatabase.GetAssetPath(material.parent);

                    validator.AssertThat(
                        path,
                        Is.Not.Null,
                        $"The parent of {validator.GetName(material)}, {validator.GetName(material.parent)} does not have appear to have an asset path!?"
                    );

                    validator.AssertAssetPath(
                        path,
                        $"The parent of {validator.GetName(material)}, {validator.GetName(material.parent)}, does NOT reside inside its package or any of that package's dependent packages!{Environment.NewLine}  Either move the material to the package, remove the reference to it, or update the package's dependencies to include the material.{Environment.NewLine}  The offending material is: {path}"
                    );
                } else {
                    validator.AssertFail($"The parent of {validator.GetName(material)} has gone MISSING!");
                }
            }
        }
    }
}