using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor {
    public interface IAssetSource {
        public IEnumerable<string> GetAssetPaths();
    }
}