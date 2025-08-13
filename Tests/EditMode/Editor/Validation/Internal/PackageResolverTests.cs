using System.Linq;
using NUnit.Framework;

namespace Slothsoft.TestRunner.Editor.Validation.Internal {
    [TestFixture]
    [TestOf(typeof(PackageResolver))]
    sealed class PackageResolverTests {
        const string UTILITIES_CHANGELOG = "Packages/" + AssemblyInfo.ID + "/CHANGELOG.md";

        public static readonly string[] directDependencies = new[] {
            "Library",
            "Packages/com.unity.editorcoroutines",
            "Packages/com.unity.test-framework",
            "Packages/net.slothsoft.test-runner",
            "Packages/net.slothsoft.unity-extensions",
            "Packages/net.tnrd.nsubstitute",
            "Resources",
        };

        public static readonly string[] indirectDependencies = new[] {
            "Packages/com.unity.ide.visualstudio",
        };

        [TestCase(AssemblyInfo.ID)]
        public void GivenPackageId_WhenGetPackageInfo_ThenReturn(string packageId) {
            PackageResolver sut = new();

            var actual = sut.GetPackageInfo(packageId);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.name, Is.EqualTo(packageId));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetDirectDependentPackagePaths_ThenListHasCount() {
            PackageResolver sut = new(true);

            var actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Is.EqualTo(directDependencies));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetDirectDependentPackagePaths_ThenListContainsPath(
            [ValueSource(nameof(directDependencies))] string expected) {

            PackageResolver sut = new(true);

            var actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Does.Contain(expected));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetAllDependentPackagePaths_ThenListHasCount() {
            PackageResolver sut = new();

            var actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Is.SupersetOf(directDependencies.Concat(indirectDependencies)));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetAllDependentPackagePaths_ThenListContainsPath(
            [ValueSource(nameof(indirectDependencies))] string expected) {

            PackageResolver sut = new();

            var actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Does.Contain(expected));
        }
    }
}
