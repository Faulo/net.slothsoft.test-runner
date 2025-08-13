using Slothsoft.TestRunner.Editor;
using Slothsoft.TestRunner.Editor.DependencyExplorer;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using Slothsoft.TestRunner.Editor.Validation.Validators;
using System;

namespace Slothsoft.TestRunner.Editor.Validation {
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ValidateAttribute : Attribute {
        public bool includeTestAssets = false;
        public bool includeWIPAssets = false;
        public bool includeDeprecatedAssets = false;

        internal bool CanValidate(AssetInfo info) {
            return (includeTestAssets || !info.isTestAsset)
                && (includeWIPAssets || !info.isWIPAsset)
                && (includeDeprecatedAssets || !info.isDeprecatedAsset);
        }
    }
}