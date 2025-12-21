#nullable enable
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Slothsoft.TestRunner {
    public sealed class TestUIHarness<T> : IDisposable where T : VisualElement, new() {
        readonly TestGameObject<Camera> testCamera = new();
        readonly TestGameObject<UIDocument> testDocument = new();

        public readonly T sut = new();
        public Camera camera => testCamera.sut;
        public UIDocument document => testDocument.sut;

        public event Action<float>? onUpdate;

#if UNITY_EDITOR
        const string PANEL_SETTINGS = "Packages/net.slothsoft.test-runner/USS/TestUIHarness_PanelSettings.asset";
        static PanelSettings settings => UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>(PANEL_SETTINGS);
#else
        static PanelSettings settings {
            get {
                var settings = ScriptableObject.CreateInstance<PanelSettings>();
                settings.themeStyleSheet = ScriptableObject.CreateInstance<ThemeStyleSheet>();
                return settings;
            }
        }
#endif

        public TestUIHarness() {
            document.panelSettings = settings;

            _ = document.StartCoroutine(Update());
        }

        IEnumerator Update() {
            document.rootVisualElement.style.flexGrow = 1;
            document.rootVisualElement.Add(sut);

            yield return null;

            if (onUpdate is null) {
                yield break;
            }

            while (true) {
                onUpdate(Time.deltaTime);
                yield return null;
            }
        }

        public void Dispose() {
            testCamera.Dispose();
            testDocument.Dispose();
        }
    }
}