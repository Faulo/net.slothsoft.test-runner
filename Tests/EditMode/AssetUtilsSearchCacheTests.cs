using NUnit.Framework;
using Slothsoft.TestRunner.Editor;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Tests.EditMode {
    [TestFixture]
    [TestOf(typeof(AssetUtils))]
    [TestMustExpectAllLogs(true)]
    internal sealed class AssetUtilsSearchCacheTests {
        private interface IA {
        }
        private interface IB {
        }
        private interface IC {
        }

        private class UA : UnityObject, IA {
        }

        private class UB : UA, IB {
        }

        private class UC : UB, IC {
        }

        private class A : IA {
        }

        private class B : A, IB {
        }

        private class C : B, IC {
        }

        [Test]
        public void GivenGetSearchCache_WhenGetInterfaceA_ThenFindDeepestCommonUnityClass() {
            Assert.That(AssetUtils.GetSearchCache<IA>(), Is.EqualTo("t:UA"));
        }

        [Test]
        public void GivenGetSearchCache_WhenGetInterfaceB_ThenFindDeepestCommonUnityClass() {
            Assert.That(AssetUtils.GetSearchCache<IB>(), Is.EqualTo("t:UB"));
        }

        [Test]
        public void GivenGetSearchCache_WhenGetInterfaceC_ThenFindDeepestCommonUnityClass() {
            Assert.That(AssetUtils.GetSearchCache<IC>(), Is.EqualTo("t:UC"));
        }
    }
}
