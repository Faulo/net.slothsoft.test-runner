using NUnit.Framework;
using UnityEditor;

namespace Slothsoft.TestRunner.Editor {
    static class ScriptValidation {
        [Validate]
        public static void ScriptsMustBeInAssemblies(MonoScript _, IAssetValidator validator) {
            Assert.That(
                string.Join('/', validator.CurrentAssetPath.Split('/')[2..]),
                Does.StartWith("Editor/").Or.StartWith("Runtime/").Or.StartWith("Tests/Editor/").Or.StartWith("Tests/Runtime/").Or.StartWith("Tests/Utilities/")
            );
        }
    }
}