using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor {
    public record AssetInfo {
        public UnityObject Asset { get; internal set; }
        public string AssetPath { get; internal set; }
        public bool IsTestAsset { get; internal set; }
    }
}
