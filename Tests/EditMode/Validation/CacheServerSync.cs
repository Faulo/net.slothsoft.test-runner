using System.Collections.Generic;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Internal;

namespace Slothsoft.TestRunner.Validation {
    sealed class CacheServerSync : CacheServerSyncBase<CacheServerSync.Source> {
        internal sealed class Source : IAssetSource {
            public IEnumerable<string> GetAssetPaths() => AssetUtils.AssetsInDirectory("Packages/" + AssemblyInfo.ID);
        }
    }
}