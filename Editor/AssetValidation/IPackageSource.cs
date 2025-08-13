using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor {
    public interface IPackageSource {
        public IEnumerable<string> GetPackageIds();
    }
}