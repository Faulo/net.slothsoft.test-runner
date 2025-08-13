using System.Collections.Generic;

namespace Slothsoft.TestRunner.Editor {
    public interface IAssemblySource {
        public IEnumerable<string> GetAssemblyNames();

        public bool IsWIPAsset(string assetPath) => AssetUtils.IsWIPAsset(assetPath);
        public bool IsTestAsset(string assetPath) => AssetUtils.IsTestAsset(assetPath);
        public bool IsDeprecatedAsset(string assetPath) => AssetUtils.IsDeprecatedAsset(assetPath);
    }
}