using System.Linq;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor;

namespace Slothsoft.TestRunner.Tests.EditMode.AssetValidation {
    [TestFixture]
    [TestOf(typeof(PackageResolver))]
    sealed class PackageResolverTests {
        const string UTILITIES_CHANGELOG = "Packages/" + AssemblyInfo.ID + "/CHANGELOG.md";

        public static readonly string[] UtilitiesDirectDependencies = new[] {
            "Library",
            "Packages/net.slothsoft.unity-extensions",
            "Packages/com.unity.test-framework",
            "Packages/net.tnrd.nsubstitute",
            "Packages/com.unity.editorcoroutines",
            "Resources",
        };

        public static readonly string[] UtilitiesIndirectDependencies = new[] {
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

            Assert.That(actual, Is.EqualTo(UtilitiesDirectDependencies));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetDirectDependentPackagePaths_ThenListContainsPath(
            [ValueSource(nameof(UtilitiesDirectDependencies))] string expected) {

            PackageResolver sut = new(true);

            var actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Does.Contain(expected));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetAllDependentPackagePaths_ThenListHasCount() {
            PackageResolver sut = new();

            var actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Is.SupersetOf(UtilitiesDirectDependencies.Concat(UtilitiesIndirectDependencies)));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetAllDependentPackagePaths_ThenListContainsPath(
            [ValueSource(nameof(UtilitiesIndirectDependencies))] string expected) {

            PackageResolver sut = new();

            var actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Does.Contain(expected));
        }
    }
}
