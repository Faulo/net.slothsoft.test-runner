#nullable enable
using System;
using System.Collections;
using UnityEditor;
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

        const string PANEL_SETTINGS = "Packages/net.slothsoft.test-runner/USS/TestUIHarness_PanelSettings.asset";

        public TestUIHarness() {
            document.panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PANEL_SETTINGS);

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