using System;
using UnityEngine;

namespace Slothsoft.TestRunner {
    public sealed record TimeScaler : IDisposable {
        public TimeScaler(float timeScale) {
            Time.timeScale = timeScale;
        }

        public void Dispose() {
            Time.timeScale = 1;
        }
    }
}
