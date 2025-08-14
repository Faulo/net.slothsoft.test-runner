using System.Collections.Generic;
using System.Linq;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Editor.DependencyExplorer {
    sealed class AssetReferenceElement : VisualElement {

        readonly ObjectField assetField = new("Target Asset") {
            allowSceneObjects = true,
        };
        readonly TextField pathField = new("Asset Location") {
            isReadOnly = true,
        };

        readonly GroupBox input = new();
        readonly VisualElement dependencies = new();
        readonly AssetPathList dependingAssetView = new("Upstream Assets", "List of all assets that reference 'Target Asset'.");
        readonly AssetPathList dependentAssetView = new("Downstream Assets", "List of all assets that 'Target Asset' references.");

        internal UnityObject targetAsset {
            get => assetField.value;
            private set => assetField.value = value;
        }

        internal string targetAssetLocation {
            get => pathField.value;
            private set {
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
            (targetAsset, targetAssetLocation) = asset
                ? (asset, AssetDatabase.GetAssetPath(asset))
                : (default, string.Empty);
        }

        internal void SetTargetAssetLocation(string assetPath) {
            (targetAsset, targetAssetLocation) = string.IsNullOrEmpty(assetPath)
                ? (default, string.Empty)
                : (AssetDatabase.LoadMainAssetAtPath(assetPath), assetPath);
        }

        void SelectAssetPaths(IReadOnlyList<string> assetPaths) {
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
            style.flexGrow = 1;

            dependingAssetView.onAssetSubmitted += SetTargetAssetLocation;
            dependingAssetView.onAssetSubmitted += _ => ClearSelections();
            dependingAssetView.onAssetsSelected += SelectAssetPaths;
            dependingAssetView.onAssetsSelected += _ => dependentAssetView.ClearSelection();

            dependentAssetView.onAssetSubmitted += SetTargetAssetLocation;
            dependentAssetView.onAssetSubmitted += _ => ClearSelections();
            dependentAssetView.onAssetsSelected += SelectAssetPaths;
            dependentAssetView.onAssetsSelected += _ => dependingAssetView.ClearSelection();

            dependencies.contentContainer.style.flexDirection = FlexDirection.Row;
            dependencies.contentContainer.style.flexGrow = 1;

            dependingAssetView.style.width = Length.Percent(50);
            dependencies.Add(dependingAssetView);

            dependencies.Add(new VerticalRule());

            dependentAssetView.style.width = Length.Percent(50);
            dependencies.Add(dependentAssetView);

            input.Add(assetField);
            input.Add(pathField);

            Add(input);
            Add(new HorizontalRule());
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