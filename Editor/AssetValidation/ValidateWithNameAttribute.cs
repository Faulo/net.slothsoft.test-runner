using System;

namespace Slothsoft.TestRunner.Editor {
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ValidateWithNameAttribute : ValidateAttribute {
        readonly string assetName;
        readonly StringComparison comparison;

        public ValidateWithNameAttribute(string assetName, StringComparison comparison = StringComparison.OrdinalIgnoreCase) {
            this.assetName = assetName;
            this.comparison = comparison;
        }

        public override bool CanValidate(AssetInfo info) {
            return base.CanValidate(info) && info.Asset.name.Equals(assetName, comparison);
        }
    }
}