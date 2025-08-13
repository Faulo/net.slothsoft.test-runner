using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor;

namespace Slothsoft.TestRunner.Tests.EditMode {
    [TestFixture]
    [TestOf(typeof(ReflectionUtils))]
    sealed class ReflectionUtilsTests {
        [AttributeUsage(AttributeTargets.Method)]
        sealed class TestMarkerAttribute : Attribute { }

        static class DummyClass {
            [TestMarker]
            public static void MarkedMethod() { }

            public static void UnmarkedMethod() { }
        }

        [Test]
        public void GivenDummyClass_WhenFindMethodsWithAttributes_ThenReturnMarkedMethod() {
            var result = ReflectionUtils.FindMethodsWithAttribute<TestMarkerAttribute>();

            var expected = typeof(DummyClass).GetMethod(nameof(DummyClass.MarkedMethod));

            Assert.That(result.ToList(), Has.Count.EqualTo(1).And.All.Matches<(MethodInfo method, TestMarkerAttribute attribute)>(pair => pair.method == expected && pair.attribute is not null));
        }
    }
}
