#nullable enable
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Slothsoft.TestRunner {
    public sealed class TestUIHarness<T> : IDisposable where T : VisualElement, new() {
        readonly string name = typeof(T).FullName;
        readonly TestObjectStore store = new();
        readonly TestGameObject<Camera> testCamera = new();
        readonly TestGameObject<UIDocument> testDocument = new();
        readonly PanelSettings settings;
        readonly ThemeStyleSheet stylesheet;

        public readonly T sut = new();
        public Camera camera => testCamera.sut;
        public UIDocument document => testDocument.sut;

        public event Action<float>? onUpdate;

        public TestUIHarness() {
            stylesheet = store.CreateScriptableObject<ThemeStyleSheet>();

            settings = store.CreateScriptableObject<PanelSettings>();
            settings.name = name;
            settings.themeStyleSheet = stylesheet;

            document.panelSettings = settings;

            _ = document.StartCoroutine(Update());
        }

        IEnumerator Update() {
            document.rootVisualElement.style.flexGrow = 1;
            document.rootVisualElement.Add(sut);

            yield return null;

            while (onUpdate is not null) {
                onUpdate(Time.deltaTime);
                yield return null;
            }
        }

        public void Dispose() {
            testCamera.Dispose();
            testDocument.Dispose();
            store.Dispose();
        }
    }
}