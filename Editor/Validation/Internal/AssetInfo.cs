using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    record AssetInfo {
        internal UnityObject asset { get; set; }
        internal string assetPath { get; set; }

        internal bool isTestAsset { get; set; }
        internal bool isWIPAsset { get; set; }
        internal bool isDeprecatedAsset { get; set; }
    }
}
