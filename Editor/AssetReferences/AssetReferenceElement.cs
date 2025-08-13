using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor {
    sealed class AssetReferenceElement : ScrollView {

        readonly ObjectField assetField = new("Target Asset") {
            allowSceneObjects = true,
        };
        readonly TextField pathField = new("Asset Location") {
            isReadOnly = true,
        };

        readonly GroupBox dependencies = new();
        readonly AssetPathList dependingAssetView = new("Upstream Assets", "List of all assets that reference 'Target Asset'.");
        readonly AssetPathList dependentAssetView = new("Downstream Assets", "List of all assets that 'Target Asset' references.");

        UnityObject TargetAsset {
            get => assetField.value;
            set => assetField.value = value;
        }

        string AssetLocation {
            get => pathField.value;
            set {
                pathField.value = value;

                if (string.IsNullOrEmpty(value)) {
                    dependingAssetView.items = Enumerable.Empty<string>();
                    dependentAssetView.items = Enumerable.Empty<string>();
                } else {
                    dependingAssetView.items = AssetUtils.DependingAssets(value);
                    dependentAssetView.items = AssetUtils.DependentAssets(value, false);
                }
            }
        }

        internal void SetTargetAsset(UnityObject asset) {
            if (asset) {
                AssetLocation = AssetDatabase.GetAssetPath(asset);
                TargetAsset = asset;
            } else {
                AssetLocation = string.Empty;
                TargetAsset = default;
            }
        }

        void SetAssetPath(string assetPath) {
            if (string.IsNullOrEmpty(assetPath)) {
                AssetLocation = string.Empty;
                TargetAsset = default;
            } else {
                AssetLocation = assetPath;
                TargetAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            }
        }

        void SelectAssetPaths(string[] assetPaths) {
            var assets = assetPaths
                .Select(AssetDatabase.LoadMainAssetAtPath)
                .Where(o => o)
                .ToArray();

            if (assets.Length > 0) {
                Selection.objects = assets;
                EditorGUIUtility.PingObject(assets[0]);
            }
        }

        public AssetReferenceElement() {
            assetField.RegisterValueChangedCallback(OnChange);

            dependingAssetView.style.width = Length.Percent(50);
            dependingAssetView.onAssetSubmitted += SetAssetPath;
            dependingAssetView.onAssetSubmitted += _ => ClearSelections();
            dependingAssetView.onAssetsSelected += SelectAssetPaths;
            dependingAssetView.onAssetsSelected += assetPaths => {
                if (assetPaths.Length > 0) {
                    dependentAssetView.ClearSelection();
                }
            };

            dependentAssetView.onAssetSubmitted += SetAssetPath;
            dependentAssetView.onAssetSubmitted += _ => ClearSelections();
            dependentAssetView.onAssetsSelected += SelectAssetPaths;
            dependentAssetView.onAssetsSelected += assetPaths => {
                if (assetPaths.Length > 0) {
                    dependingAssetView.ClearSelection();
                }
            };
            dependentAssetView.style.width = Length.Percent(50);

            dependencies.contentContainer.style.flexDirection = FlexDirection.Row;
            dependencies.Add(dependingAssetView);
            dependencies.Add(dependentAssetView);

            Add(assetField);
            Add(pathField);

            var hr = new VisualElement();
            hr.AddToClassList("hr");
            Add(hr);
            Add(dependencies);
        }

        void ClearSelections() {
            dependingAssetView.ClearSelection();
            dependentAssetView.ClearSelection();
        }

        void OnChange(ChangeEvent<UnityObject> evt) {
            SetTargetAsset(evt.newValue);
        }
    }
}