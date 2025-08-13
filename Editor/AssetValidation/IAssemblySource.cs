using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor {
    public interface IAssemblySource {
        public IEnumerable<string> GetAssemblyNames();
    }
}