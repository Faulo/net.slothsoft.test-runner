using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor.Validation {
    public interface IAssetSource {
        public IEnumerable<string> GetAssetPaths();
    }
}