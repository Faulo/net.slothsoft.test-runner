using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal;

namespace Slothsoft.TestRunner.Editor.Validation.Validators {
    [TestFixture]
    [TestOf(typeof(AssemblyDefinitionValidation))]
    sealed class AssemblyDefinitionValidationTests {
        const string PACKAGE = "Packages/" + AssemblyInfo.ID;
        const string RUNTIME_ASSEMBLY = PACKAGE + "/Runtime/" + AssemblyInfo.NAMESPACE_RUNTIME + ".asmdef";
        const string EDITOR_ASSEMBLY = PACKAGE + "/Editor/" + AssemblyInfo.NAMESPACE_EDITOR + ".asmdef";
        const string RUNTIME_TESTS_ASSEMBLY = PACKAGE + "/Tests/PlayMode/" + AssemblyInfo.NAMESPACE_TESTS_PLAYMODE + ".asmdef";
        const string EDITOR_TESTS_ASSEMBLY = PACKAGE + "/Tests/EditMode/" + AssemblyInfo.NAMESPACE_TESTS_EDITMODE + ".asmdef";

        [TestCase(EDITOR_ASSEMBLY, RUNTIME_ASSEMBLY)]
        [TestCase(RUNTIME_TESTS_ASSEMBLY, RUNTIME_ASSEMBLY)]
        [TestCase(EDITOR_TESTS_ASSEMBLY, RUNTIME_ASSEMBLY)]
        [TestCase(EDITOR_TESTS_ASSEMBLY, EDITOR_ASSEMBLY)]
        public void GivenAssembly_WhenGetGetReferencesAssemblies_ThenContains(string assetPath, string expected) {
            var assembly = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);

            var actual = AssemblyDefinitionValidation.GetReferencesAssemblies(assembly);

            Assert.That(actual, Does.Contain(expected));
        }
    }
}