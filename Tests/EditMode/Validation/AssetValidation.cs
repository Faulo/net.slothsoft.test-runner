using System.Collections.Generic;
using Slothsoft.TestRunner.Editor.Validation;
using Slothsoft.TestRunner.Editor.Validation.Internal;

namespace Slothsoft.TestRunner.Validation {
    sealed class AssetValidation : AssetValidationBase<AssetValidation.AssetSource> {
        internal sealed class AssetSource : IAssetSource {
            public IEnumerable<string> GetAssetPaths() => AssetUtils.AssetsInDirectory("Packages/" + AssemblyInfo.ID);
        }
    }
}