using System.Collections.Generic;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor;
using UnityEditor;
using UnityEditorInternal;

namespace Slothsoft.TestRunner.Tests.EditMode.AssetValidators {
    [TestFixture]
    [TestOf(typeof(AssemblyDefinitionValidation))]
    internal sealed class AssemblyDefinitionValidationTests {
        private const string PACKAGE = "Packages/" + AssemblyInfo.ID;
        private const string RUNTIME_ASSEMBLY = PACKAGE + "/Runtime/" + AssemblyInfo.NAMESPACE_RUNTIME + ".asmdef";
        private const string EDITOR_ASSEMBLY = PACKAGE + "/Editor/" + AssemblyInfo.NAMESPACE_EDITOR + ".asmdef";
        private const string RUNTIME_TESTS_ASSEMBLY = PACKAGE + "/Tests/PlayMode/" + AssemblyInfo.NAMESPACE_TESTS_PLAYMODE + ".asmdef";
        private const string EDITOR_TESTS_ASSEMBLY = PACKAGE + "/Tests/EditMode/" + AssemblyInfo.NAMESPACE_TESTS_EDITMODE + ".asmdef";

        [TestCase(EDITOR_ASSEMBLY, RUNTIME_ASSEMBLY)]
        [TestCase(RUNTIME_TESTS_ASSEMBLY, RUNTIME_ASSEMBLY)]
        [TestCase(EDITOR_TESTS_ASSEMBLY, RUNTIME_ASSEMBLY)]
        [TestCase(EDITOR_TESTS_ASSEMBLY, EDITOR_ASSEMBLY)]
        public void GivenAssembly_WhenGetGetReferencesAssemblies_ThenContains(string assetPath, string expected) {
            AssemblyDefinitionAsset assembly = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);

            IEnumerable<string> actual = AssemblyDefinitionValidation.GetReferencesAssemblies(assembly);

            Assert.That(actual, Does.Contain(expected));
        }
    }
}