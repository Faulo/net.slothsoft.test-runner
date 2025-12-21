#nullable enable
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Slothsoft.TestRunner.Runtime {
    [TestFixture]
    [TestOf(typeof(MemoryMeasurement))]
    sealed class MemoryMeasurementTests {
        [UnityTest, Performance]
        [UnityTestCase(1, 1, true)]
        [UnityTestCase(10, 10, false)]
        [UnityTestCase(100, 100, true)]
        public IEnumerator DrawEveryFrame(int warmupCount, int measurementCount, bool recordFrameTime) {
            using var test = new TestUIHarness<VisualElement>();

            return new MemoryMeasurement()
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .RecordFrameTime(recordFrameTime)
                .Run(test.sut.MarkDirtyRepaint);
        }
    }
}
