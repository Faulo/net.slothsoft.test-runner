#nullable enable
using NUnit.Framework;

namespace Slothsoft.TestRunner {
    /// <summary>
    /// Shorthand attribute for a <see cref="TestCaseAttribute"/> with <see cref="TestCaseAttribute.ExpectedResult"/>=<see langword="false"/>.
    /// </summary>
    public sealed class UnityTestCaseAttribute : TestCaseAttribute {
        public UnityTestCaseAttribute(params object[] arguments) : base(arguments) {
            ExpectedResult = null;
        }
    }
}
