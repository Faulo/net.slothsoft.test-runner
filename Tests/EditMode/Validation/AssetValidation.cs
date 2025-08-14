using System.Collections.Generic;
using Slothsoft.TestRunner.Editor.Validation;

namespace Slothsoft.TestRunner.Validation {
    sealed class AssetValidation : AssetValidationBase<AssetValidation.Source> {
        internal sealed class Source : IAssetSource {
            public IEnumerable<string> GetAssetPaths() => AssetUtils.AssetsInDirectory("Packages/" + AssemblyInfo.ID);
        }
    }
}