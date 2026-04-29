using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor.Validation {
    public interface IPackageSource {
        IEnumerable<string> GetPackageIds();
    }
}