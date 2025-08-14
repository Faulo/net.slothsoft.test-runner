using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor.Validation {
    public interface IPackageSource {
        public IEnumerable<string> GetPackageIds();
    }
}