using System.Collections;
using System.Diagnostics;
using System.Linq;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Slothsoft.TestRunner.Editor.DependencyExplorer {
    sealed class DependencyExplorerWindow : EditorWindow {
        const string WINDOW_TITLE = "Asset Dependency Explorer";
        const string MENU_ITEM = "Assets/" + WINDOW_TITLE;

        AssetReferenceElement element;

        void UpdateTarget() {
            element.SetTargetAsset(Selection.objects.FirstOrDefault());
        }

        [MenuItem(MENU_ITEM, false, 20)]
        public static void FindReferences() {
            var window = GetWindow<DependencyExplorerWindow>(WINDOW_TITLE);

            if (window.element is not null) {
                window.UpdateTarget();
            }
        }

        void OnEnable() {
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"Packages/{AssemblyInfo.ID}/USS/Default.uss");
            if (stylesheet) {
                rootVisualElement.styleSheets.Add(stylesheet);
            }

            EditorCoroutineUtility.StartCoroutine(Start_Co(), this);
        }

        IEnumerator Start_Co() {
            yield return PopulateDependencyCache_Co();

            element = new AssetReferenceElement();
            UpdateTarget();

            rootVisualElement.Add(element);
        }

        IEnumerator PopulateDependencyCache_Co() {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            Label h1 = new();
            h1.AddToClassList("h1");
            rootVisualElement.Add(h1);

            Label h3 = new();
            h3.AddToClassList("h3");
            rootVisualElement.Add(h3);

            string[] assetPaths = AssetDatabase.GetAllAssetPaths();

            for (int i = 0; i < assetPaths.Length; i++) {
                if (stopwatch.ElapsedMilliseconds > 20) {
                    int progress = Mathf.RoundToInt(100f * i / assetPaths.Length);

                    h1.text = $"Building Dependency Map: {progress}%";
                    h3.text = $"Current Asset: {assetPaths[i]}";

                    yield return null;

                    stopwatch.Restart();
                }

                _ = AssetUtils.DependentAssets(assetPaths[i], false);
            }

            h1.text = $"Building Dependency Map: 100%";
            h3.text = "Done!";

            yield return null;

            rootVisualElement.Clear();
        }

        void OnDisable() {
            element = default;
            rootVisualElement.Clear();
        }
    }
}