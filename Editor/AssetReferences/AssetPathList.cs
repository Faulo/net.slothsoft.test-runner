using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Slothsoft.TestRunner.Editor {
    sealed class AssetPathList : VisualElement {
        public event Action<string> onAssetSubmitted;
        public event Action<string[]> onAssetsSelected;

        readonly ListView view = new();
        readonly List<string> assetPaths = new();

        internal IEnumerable<string> items {
            set {
                assetPaths.Clear();
                assetPaths.AddRange(value.OrderBy(p => p));
                view.RefreshItems();
            }
        }

        internal void ClearSelection() {
            view.ClearSelection();
        }

        internal AssetPathList(string title, string tooltip) {
            view.makeItem = () => {
                VisualElement root = new();

                Label label = new();
                root.Add(label);

                root.RegisterCallback<ClickEvent>(evt => {
                    switch (evt.clickCount) {
                        case 1:
                            OnSelectionChanges(view.selectedItems);
                            break;
                        case 2:
                            OnDoubleClick(root.userData as string);
                            break;
                    }
                });

                return root;
            };

            view.bindItem = (root, index) => {
                root.userData = assetPaths[index];
                root.Q<Label>().text = assetPaths[index];
            };

            view.itemsSource = assetPaths;
            view.selectionType = SelectionType.Multiple;
            view.selectionChanged += OnSelectionChanges;

            Label label = new(title) {
                tooltip = tooltip,
            };
            label.AddToClassList("h2");
            Add(label);

            Add(view);
        }

        void OnSelectionChanges(IEnumerable<object> selection) {
            string[] assetPaths = selection
                .OfType<string>()
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            if (assetPaths.Length > 0) {
                onAssetsSelected.Invoke(assetPaths);
            }
        }

        void OnDoubleClick(string assetPath) {
            if (!string.IsNullOrEmpty(assetPath)) {
                onAssetSubmitted?.Invoke(assetPath);
            }
        }
    }
}