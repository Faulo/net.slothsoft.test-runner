using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Slothsoft.TestRunner.Runtime {
    [TestFixture]
    [TestOf(typeof(TestGameObject))]
    sealed class TestGameObjectConcreteTestsEditMode : TestGameObjectConcreteTests {
    }

    [TestFixture]
    [TestOf(typeof(TestGameObject))]
    sealed class TestGameObjectConcreteTestsPlayMode : TestGameObjectConcreteTests {
    }

    abstract class TestGameObjectConcreteTests {
        TestGameObject sut;

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
                Assert.AreEqual("TestGameObject", sut.gameObject.name);
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
    }
}
