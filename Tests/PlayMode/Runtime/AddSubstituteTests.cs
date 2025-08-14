using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Runtime {
    abstract class AbstractTestBehaviour : MonoBehaviour, ITestInterface {
        public abstract void Print(string message);
    }

    sealed class ConcreteTestBehaviour : MonoBehaviour, ITestInterface {
        public void Print(string message) {
        }
    }

    interface ITestInterface {
        void Print(string message);
    }

    [TestFixture(typeof(AbstractTestBehaviour))]
    [TestFixture(typeof(ConcreteTestBehaviour))]
    [TestFixture(typeof(ITestInterface))]
    [TestOf(typeof(GameObjectExtensions))]
    sealed class AddSubstituteTests<T> where T : class, ITestInterface {
        GameObject gameObject;

        [SetUp]
        public void SetUp() {
            gameObject = new();
        }
        [TearDown]
        public void TearDown() {
            UnityObject.Destroy(gameObject);
        }

        [Test]
        public void T01_SubstituteIsNotNull() {
            var substitute = gameObject.AddSubstitute<T>();

            Assert.IsNotNull(substitute);
        }

        [Test]
        public void T02_SubstituteIsMonoBehaviour() {
            var substitute = gameObject.AddSubstitute<T>();

            Assert.IsInstanceOf<MonoBehaviour>(substitute);
            Assert.IsTrue(substitute as MonoBehaviour);
        }

        [Test]
        public void T11_SubstituteIsAttachedToGameObject() {
            var substitute = gameObject.AddSubstitute<T>();

            var substituteComponent = substitute as MonoBehaviour;

            Assert.AreEqual(gameObject, substituteComponent.gameObject);
        }

        [Test]
        public void T12_SubstituteHasTransform() {
            var substitute = gameObject.AddSubstitute<T>();

            var substituteComponent = substitute as MonoBehaviour;

            Assert.AreEqual(gameObject.transform, substituteComponent.transform);
        }

        [TestCase("Hello World")]
        public void T21_SubstituteCanReceiveMethods(string message) {
            var substitute = gameObject.AddSubstitute<T>();

            substitute.Print(message);

            substitute.Received().Print(message);
        }

        [TestCase("Hello World")]
        public void T22_SubstituteCanReceiveMessages(string message) {
            var substitute = gameObject.AddSubstitute<T>();

            gameObject.SendMessage(nameof(ITestInterface.Print), message);

            substitute.Received().Print(message);
        }
    }
}
