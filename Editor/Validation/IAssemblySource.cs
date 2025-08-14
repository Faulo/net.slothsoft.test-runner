using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor.Validation {
    public interface IAssemblySource {
        public IEnumerable<string> GetAssemblyNames();
    }
}