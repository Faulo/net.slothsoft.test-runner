using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Slothsoft.TestRunner {
    public static class WaitUtils {
        static readonly WaitForFixedUpdate forFixedUpdate = new();

        public static IEnumerator WaitFor(Func<bool> predicate, string message = default, float timeout = 1) {
            float timer = 0;
            while (!predicate() && timer < timeout) {
                timer += Time.fixedDeltaTime;
                yield return forFixedUpdate;
            }

            Assert.That(predicate(), Is.True, message ?? $"Waited {timeout} second(s) in vain!");
        }
    }
}
