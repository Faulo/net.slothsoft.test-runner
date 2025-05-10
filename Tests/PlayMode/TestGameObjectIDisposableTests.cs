using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.TestTools;

namespace Slothsoft.TestRunner.Tests.PlayMode {
    [TestFixture]
    [TestOf(typeof(TestGameObject<TestComponent>))]
    sealed class TestGameObjectIDisposableTests {
        sealed class TestComponent : MonoBehaviour {
        }

        [UnityTest]
        public IEnumerator TestConcreteDeconstructorDoesNotErrorWhenDisposed() {
            {
                var sut = new TestGameObject();
                yield return null;
                sut.Dispose();
                sut = default;
            }

            yield return GC_Co();
        }

        [UnityTest]
        public IEnumerator TestConcreteDeconstructorDoesErrorWhenNotDisposed() {
            {
                var sut = new TestGameObject();
                yield return null;
                LogAssert.Expect(LogType.Error, "A previous TestGameObject was not disposed of properly!");
                sut = default;
            }

            yield return GC_Co();
        }

        [UnityTest]
        public IEnumerator TestGenericDeconstructorDoesNotErrorWhenDisposed() {
            {
                var sut = new TestGameObject<TestComponent>();
                yield return null;
                sut.Dispose();
                sut = default;
            }

            yield return GC_Co();
        }

        [UnityTest]
        public IEnumerator TestGenericDeconstructorDoesErrorWhenNotDisposed() {
            {
                var sut = new TestGameObject<TestComponent>();
                yield return null;
                LogAssert.Expect(LogType.Error, "A previous TestGameObject<TestComponent> was not disposed of properly!");
                sut = default;
            }

            yield return GC_Co();
        }

        IEnumerator GC_Co() {
            yield return null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            yield return null;
            GC.WaitForPendingFinalizers();
            yield return null;
        }
    }
}
