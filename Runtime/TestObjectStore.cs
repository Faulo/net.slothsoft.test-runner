using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner {
    public class TestObjectStore : IDisposable {
        readonly List<UnityObject> instances = new();

        readonly bool destroyImmediately;

        public TestObjectStore() : this(!Application.isPlaying) {
        }

        public TestObjectStore(bool destroyImmediately) {
            this.destroyImmediately = destroyImmediately;
        }

        ~TestObjectStore() {
            if (instances.Count > 0) {
                Debug.LogError($"A previous {this} was not disposed of properly!");
            }
        }

        public T CreateUnityObject<T>(string name = default) where T : UnityObject, new() {
            T instance = new() {
                name = string.IsNullOrEmpty(name) ? "New " + ObjectNames.NicifyVariableName(typeof(T).Name) : name,
                hideFlags = HideFlags.DontSave
            };
            instances.Add(instance);
            return instance;
        }

        public Material CreateMaterial(string shader = "Diffuse") {
            var instance = new Material(Shader.Find(shader));
            instances.Add(instance);
            return instance;
        }

        public T CreateScriptableObject<T>(string name = default) where T : ScriptableObject {
            var instance = ScriptableObject.CreateInstance<T>();
            instance.name = string.IsNullOrEmpty(name) ? "New " + ObjectNames.NicifyVariableName(typeof(T).Name) : name;
            instance.hideFlags = HideFlags.DontSave;
            instances.Add(instance);
            return instance;
        }

        public GameObject CreateGameObject(string name = default, Vector3? position = default, Quaternion? rotation = default, Transform parent = null) {
            GameObject instance = string.IsNullOrEmpty(name)
                ? new()
                : new(name);

            if (parent) {
                instance.transform.parent = parent;
            }

            if (position.HasValue) {
                instance.transform.position = position.Value;
            }

            if (rotation.HasValue) {
                instance.transform.rotation = rotation.Value;
            }

            instances.Add(instance);

            return instance;
        }

        public GameObject CreateGameObjectFromPrefab(GameObject prefab, string name = default, Vector3? position = default, Quaternion? rotation = default, Transform parent = null) {
            var instance = parent
                ? UnityObject.Instantiate(prefab, parent)
                : UnityObject.Instantiate(prefab);

            if (!string.IsNullOrEmpty(name)) {
                instance.name = name;
            }

            if (position.HasValue) {
                instance.transform.position = position.Value;
            }

            if (rotation.HasValue) {
                instance.transform.rotation = rotation.Value;
            }

            instances.Add(instance);

            return instance;
        }

        public GameObject CreatePrimitive(PrimitiveType type, Vector3? position = default, Quaternion? rotation = default) {
            var instance = GameObject.CreatePrimitive(type);
            if (position.HasValue) {
                instance.transform.position = position.Value;
            }

            if (rotation.HasValue) {
                instance.transform.rotation = rotation.Value;
            }

            instances.Add(instance);

            return instance;
        }

        public RenderTexture CreateRenderTexture(in RenderTextureDescriptor descriptor) {
            var instance = new RenderTexture(descriptor);
            instances.Add(instance);
            return instance;
        }

        /// <summary>
        /// Destroys all objects created by this instance.
        /// </summary>
        public void Dispose() {
            foreach (var instance in instances) {
                if (instance) {
                    if (instance is RenderTexture rt) {
                        rt.Release();
                    }

                    if (destroyImmediately) {
                        UnityObject.DestroyImmediate(instance);
                    } else {
                        UnityObject.Destroy(instance);
                    }
                }
            }

            instances.Clear();
        }

        public void Register(UnityObject toBeDisposed) {
            switch (toBeDisposed) {
                case null:
                    break;
                case Component { gameObject: GameObject obj }:
                    instances.Add(obj);
                    break;
                default:
                    instances.Add(toBeDisposed);
                    break;
            }
        }
    }
}