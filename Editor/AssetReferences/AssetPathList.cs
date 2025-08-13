using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
                view.Rebuild();
            }
        }

        internal void ClearSelection() {
            view.ClearSelection();
        }

        internal AssetPathList(string title, string tooltip) {
            view.makeItem = () => {
                var root = new VisualElement {
                    pickingMode = PickingMode.Position,
                };
                root.style.flexDirection = FlexDirection.Row;
                root.style.alignItems = Align.Center;
                root.style.alignContent = Align.FlexStart;
                root.style.paddingLeft = 4;
                root.style.paddingRight = 4;
                root.style.height = EditorGUIUtility.singleLineHeight;

                var icon = new Image {
                    scaleMode = ScaleMode.ScaleAndCrop,
                };
                icon.style.width = EditorGUIUtility.singleLineHeight;
                icon.style.height = EditorGUIUtility.singleLineHeight;
                icon.style.marginRight = 4;
                icon.style.flexShrink = 0;
                root.Add(icon);

                var label = new Label();
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
                string path = assetPaths[index];
                root.userData = path;

                root.Q<Label>().text = path;

                var icon = AssetDatabase.GetCachedIcon(path);
                if (!icon) {
                    icon = EditorGUIUtility.IconContent("TextAsset Icon").image;
                }

                root.Q<Image>().image = icon;
                root.tooltip = path;
            };

            view.itemsSource = assetPaths;
            view.selectionType = SelectionType.Multiple;
            view.selectionChanged += OnSelectionChanges;

            var header = new Label(title) {
                tooltip = tooltip,
            };
            header.AddToClassList("h2");
            Add(header);

            Add(view);
        }

        void OnSelectionChanges(IEnumerable<object> selection) {
            string[] selected = selection
                .OfType<string>()
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            if (selected.Length > 0) {
                onAssetsSelected?.Invoke(selected);
            }
        }

        void OnDoubleClick(string assetPath) {
            if (!string.IsNullOrEmpty(assetPath)) {
                onAssetSubmitted?.Invoke(assetPath);
            }
        }
    }
}
