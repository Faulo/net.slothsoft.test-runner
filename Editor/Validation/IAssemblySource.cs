using System.Collections.Generic;
using Slothsoft.TestRunner.Editor.Validation.Internal;

namespace Slothsoft.TestRunner.Editor.Validation {
    public interface IAssemblySource {
        public IEnumerable<string> GetAssemblyNames();

        public bool IsWIPAsset(string assetPath) => AssetUtils.IsWIPAsset(assetPath);
        public bool IsTestAsset(string assetPath) => AssetUtils.IsTestAsset(assetPath);
        public bool IsDeprecatedAsset(string assetPath) => AssetUtils.IsDeprecatedAsset(assetPath);
    }
}