using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner {
    public class TestGameObject : IDisposable {
        protected readonly string name;
        public readonly GameObject gameObject;
        public readonly Transform transform;

        protected TestGameObject(string name) {
            this.name = name;
            gameObject = new(name);
            transform = gameObject.transform;
        }

        public TestGameObject(Action<GameObject> setup = null)
            : this(nameof(TestGameObject)) {
            setup?.Invoke(gameObject);
        }
        ~TestGameObject() {
            if (!wasDisposed) {
                Debug.LogError($"A previous {name} was not disposed of properly!");
            }
        }

        bool wasDisposed;
        public void Dispose() {
            wasDisposed = true;
            if (Application.isPlaying) {
                UnityObject.Destroy(gameObject);
            } else {
                UnityObject.DestroyImmediate(gameObject);
            }
        }
    }

    public sealed class TestGameObject<TSuT> : TestGameObject where TSuT : Component {
        public readonly TSuT sut;

        public TestGameObject(Action<GameObject> setup = null)
            : base($"{nameof(TestGameObject<TSuT>)}<{typeof(TSuT).Name}>") {
            setup?.Invoke(gameObject);
            sut = gameObject.AddComponent<TSuT>();
        }
    }
}
