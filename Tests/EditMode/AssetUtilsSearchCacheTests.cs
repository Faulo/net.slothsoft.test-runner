using NUnit.Framework;
using Slothsoft.TestRunner.Editor;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.TestRunner.Tests.EditMode {
    [TestFixture]
    [TestOf(typeof(AssetUtils))]
    [TestMustExpectAllLogs(true)]
    sealed class AssetUtilsSearchCacheTests {
        interface IA {
        }
        interface IB {
        }
        interface IC {
        }

        class UA : UnityObject, IA {
        }

        class UB : UA, IB {
        }

        class UC : UB, IC {
        }

        class A : IA {
        }

        class B : A, IB {
        }

        class C : B, IC {
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
