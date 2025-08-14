using System.Collections.Generic;
using Slothsoft.TestRunner.Editor.Validation;

namespace Slothsoft.TestRunner.Validation {
    sealed class PackageValidation : PackageValidationBase<PackageValidation.Source> {
        internal sealed class Source : IPackageSource {
            public IEnumerable<string> GetPackageIds() => new[] { AssemblyInfo.ID };
        }
    }
}