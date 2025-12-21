#nullable enable
using System;
using System.Collections;
using Unity.PerformanceTesting;
using Unity.Profiling;
using UnityEngine;

namespace Slothsoft.TestRunner {
    public sealed class MemoryMeasurement {
        readonly string m_counterName;
        readonly ProfilerCategory m_counterCategory;

        int m_warmupCount = 1;
        int m_measurementCount = 1;

        SampleUnit? m_displayUnit;

        bool m_recordFrameTime = false;
        readonly SampleGroup m_recordFrameTimeGroup = new("FrameTime");

        /// <summary>
        /// Creates a new memory recorder using <see href="https://docs.unity3d.com/6000.3/Documentation/Manual/profiler-counters-reference.html">Unity's memory counters</see>.
        /// </summary>
        /// <param name="counterName">One Unity's memory counters.</param>
        public MemoryMeasurement(string counterName = "GC Allocated In Frame") : this(counterName, ProfilerCategory.Memory) {
        }

        /// <summary>
        /// Creates a new memory recorder using <see href="https://docs.unity3d.com/6000.3/Documentation/Manual/profiler-counters-reference.html">Unity's counters</see>.
        /// </summary>
        /// <param name="counterName">One Unity's counters in <paramref name="counterName"/>.</param>
        /// <param name="category">One of Unity's <see href="https://docs.unity3d.com/6000.3/Documentation/Manual/profiler-counters-reference.html">Unity's counters</see>.</param>
        public MemoryMeasurement(string counterName, ProfilerCategory category) {
            m_counterName = counterName;
            m_counterCategory = category;
        }

        /// <summary>
        /// Specifies whether frame times should be recorded.
        /// </summary>
        /// <returns>An updated instance of the <see cref="MemoryMeasurement"/> to be used in fluent syntax.</returns>
        public MemoryMeasurement RecordFrameTime(bool doRecord = true) {
            m_recordFrameTime = doRecord;
            return this;
        }

        /// <summary>
        /// Count of warmup executions.
        /// </summary>
        /// <param name="count">Count of warmup executions.</param>
        /// <returns>An updated instance of the <see cref="MemoryMeasurement"/> to be used in fluent syntax.</returns>
        public MemoryMeasurement WarmupCount(int count) {
            m_warmupCount = Mathf.Max(1, count);
            return this;
        }

        /// <summary>
        /// Unit to use when displaying memory usage.
        /// </summary>
        /// <param name="displayUnit">One of <see langword="null"/>, <see cref="SampleUnit.Byte"/>, <see cref="SampleUnit.Kilobyte"/>, <see cref="SampleUnit.Megabyte"/>, or <see cref="SampleUnit.Gigabyte"/>. Pass <see langword="null"/> to auto-detect which unit to use during warmup.</param>
        /// <returns>An updated instance of the <see cref="MemoryMeasurement"/> to be used in fluent syntax.</returns>
        public MemoryMeasurement DisplayUnit(SampleUnit? displayUnit) {
            m_displayUnit = displayUnit;
            return this;
        }

        /// <summary>
        /// Count of measurements to take.
        /// </summary>
        /// <param name="count">Count of measurements.</param>
        /// <returns>An updated instance of the <see cref="MemoryMeasurement"/> to be used in fluent syntax.</returns>
        public MemoryMeasurement MeasurementCount(int count) {
            m_measurementCount = Mathf.Max(1, count);
            return this;
        }

        /// <summary>
        /// Executes the memory measurement with given parameters.
        /// </summary>
        /// <returns><see cref="IEnumerator"/> to yield until finish.</returns>
        public IEnumerator Run(Action? action = null) {
            using var recorder = ProfilerRecorder.StartNew(m_counterCategory, m_counterName);
            double average = 0;

            recorder.Reset();

            for (int i = 0; i < m_warmupCount; i++) {
                action?.Invoke();
                recorder.Start();
                yield return null;
                average += recorder.LastValueAsDouble / m_warmupCount;
                recorder.Reset();
            }

            var target = m_displayUnit ?? average switch {
                > 1073741824 => SampleUnit.Gigabyte,
                > 1048576 => SampleUnit.Megabyte,
                > 1024 => SampleUnit.Kilobyte,
                _ => SampleUnit.Byte,
            };
            var group = new SampleGroup(m_counterName, target);

            recorder.Reset();

            for (int i = 0; i < m_measurementCount; i++) {
                action?.Invoke();

                recorder.Start();

                if (m_recordFrameTime) {
                    using (Measure.Scope(m_recordFrameTimeGroup)) {
                        yield return null;
                    }
                } else {
                    yield return null;
                }

                double value = target switch {
                    SampleUnit.Gigabyte => recorder.LastValueAsDouble / 1073741824,
                    SampleUnit.Megabyte => recorder.LastValueAsDouble / 1048576,
                    SampleUnit.Kilobyte => recorder.LastValueAsDouble / 1024,
                    _ => recorder.LastValueAsDouble,
                };

                recorder.Reset();

                Measure.Custom(group, value);
            }
        }
    }
}
