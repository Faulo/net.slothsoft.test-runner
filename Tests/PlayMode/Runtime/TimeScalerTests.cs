using NUnit.Framework;
using UnityEngine;

namespace Slothsoft.TestRunner.Runtime {
    [TestFixture]
    [TestOf(typeof(TimeScaler))]
    sealed class TimeScalerTests {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenTimeScaler_WhenConstruct_ThenSetTimeScale(float timeScale) {
            using (new TimeScaler(timeScale)) {
                Assert.That(Time.timeScale, Is.EqualTo(timeScale));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenTimeScaler_WhenDispose_ThenResetTimeScale(float timeScale) {
            using (new TimeScaler(timeScale)) {
            }

            Assert.That(Time.timeScale, Is.EqualTo(1));
        }
    }
}
