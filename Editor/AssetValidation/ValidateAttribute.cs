using System;

namespace Slothsoft.TestRunner.Editor {
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ValidateAttribute : Attribute {
        public bool IncludeTests = false;

        public virtual bool CanValidate(AssetInfo info) {
            return IncludeTests || !info.IsTestAsset;
        }
    }
}