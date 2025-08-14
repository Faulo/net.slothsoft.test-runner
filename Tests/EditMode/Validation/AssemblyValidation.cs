using System.Collections.Generic;
using Slothsoft.TestRunner.Editor.Validation;

namespace Slothsoft.TestRunner.Validation {
    sealed class AssemblyValidation : AssemblyValidationBase<AssemblyValidation.Source> {
        internal sealed class Source : IAssemblySource {
            public IEnumerable<string> GetAssemblyNames() => new string[] {
                // AssemblyInfo.NAMESPACE_RUNTIME,
                // AssemblyInfo.NAMESPACE_EDITOR,
                // AssemblyInfo.NAMESPACE_TESTS_PLAYMODE,
                // AssemblyInfo.NAMESPACE_TESTS_EDITMODE,
            };
        }
    }
}