using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Slothsoft.TestRunner.Runtime {
    [TestFixture]
    [TestOf(typeof(TestGameObject<TestComponent>))]
    sealed class TestGameObjectGenericTestsEditMode : TestGameObjectGenericTests {
    }

    [TestFixture]
    [TestOf(typeof(TestGameObject<TestComponent>))]
    sealed class TestGameObjectGenericTestsPlayMode : TestGameObjectGenericTests {
    }

    abstract class TestGameObjectGenericTests {
        protected class TestComponent : MonoBehaviour {
        }

        TestGameObject<TestComponent> sut;

        [UnitySetUp]
        public IEnumerator SetUp() {
            if (sut is not null) {
                if (sut.gameObject) {
                    yield return null;
                }

                Assert.IsFalse(sut.gameObject);
                sut = null;
            }

            yield return null;
        }

        [TearDown]
        public void TearDown() {
            Assert.IsNotNull(sut);
        }

        [Test]
        public void TestGameObjectGetsCreated() {
            using (sut = new()) {
                Assert.IsTrue(sut.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator TestGameObjectGetsCreated_Co() {
            using (sut = new()) {
                Assert.IsTrue(sut.gameObject);
                yield return null;
                Assert.IsTrue(sut.gameObject);
            }
        }

        [Test]
        public void TestGameObjectGetsNamed() {
            using (sut = new()) {
                Assert.AreEqual("TestGameObject<TestComponent>", sut.gameObject.name);
            }
        }

        [UnityTest]
        public IEnumerator TestGameObjectGetsDestroyed_Co() {
            using (sut = new()) {
                yield return null;
            }

            yield return null;
            Assert.IsFalse(sut.gameObject);
        }

        [UnityTest]
        public IEnumerator TestComponentGetsAdded_Co() {
            using (sut = new()) {
                Assert.IsTrue(sut.sut);
                Assert.AreEqual(sut.gameObject, sut.sut.gameObject);
                yield return null;
                Assert.IsTrue(sut.sut);
                Assert.AreEqual(sut.gameObject, sut.sut.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator TestComponentGetsDestroyed_Co() {
            using (sut = new()) {
                yield return null;
            }

            yield return null;
            Assert.IsFalse(sut.sut);
        }

        [Test]
        public void TestSetupGetsCalled() {
            bool wasCalled = false;
            using (sut = new(_ => wasCalled = true)) {
                Assert.IsTrue(wasCalled);
            }
        }

        [Test]
        public void TestSetupGetsCalledWithGameObject() {
            GameObject obj = default;
            using (sut = new(value => obj = value)) {
                Assert.AreEqual(sut.gameObject, obj);
            }
        }

        [Test]
        public void TestSetupGetsCalledBeforeAddComponent() {
            using (sut = new(obj => Assert.IsFalse(obj.GetComponent<TestComponent>()))) {
                Assert.Pass();
            }
        }
    }
}
