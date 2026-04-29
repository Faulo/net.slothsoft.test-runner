using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor.Validation {
    public interface IAssetSource {
        IEnumerable<string> GetAssetPaths();
    }
}