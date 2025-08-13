using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor;
using UnityEditor.PackageManager;

namespace Slothsoft.TestRunner.Tests.EditMode.AssetValidation {
    [TestFixture]
    [TestOf(typeof(PackageResolver))]
    internal sealed class PackageResolverTests {
        private const string UTILITIES_CHANGELOG = "Packages/" + AssemblyInfo.ID + "/CHANGELOG.md";

        public static readonly string[] UtilitiesDirectDependencies = new[] {
            "Library",
            "Packages/com.unity.editorcoroutines",
            "Packages/com.unity.mathematics",
            "Packages/com.unity.modules.animation",
            "Packages/com.unity.modules.audio",
            "Packages/com.unity.modules.uielements",
            "Packages/com.unity.nuget.newtonsoft-json",
            "Packages/com.unity.shadergraph",
            "Packages/com.unity.test-framework",
            "Packages/com.unity.ugui",
            "Packages/com.unity.ui",
            "Packages/de.ulisses-spiele.core.utilities",
            "Packages/net.tnrd.nsubstitute",
            "Resources",
        };

        public static readonly string[] UtilitiesIndirectDependencies = new[] {
            "Packages/com.unity.burst",
            "Packages/com.unity.modules.physics",
            "Packages/com.unity.collections",
            "Packages/com.unity.ext.nunit",
        };

        [TestCase(AssemblyInfo.ID)]
        public void GivenPackageId_WhenGetPackageInfo_ThenReturn(string packageId) {
            PackageResolver sut = new();

            PackageInfo actual = sut.GetPackageInfo(packageId);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.name, Is.EqualTo(packageId));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetDirectDependentPackagePaths_ThenListHasCount() {
            PackageResolver sut = new(true);

            IReadOnlyList<string> actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Is.EqualTo(UtilitiesDirectDependencies));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetDirectDependentPackagePaths_ThenListContainsPath(
            [ValueSource(nameof(UtilitiesDirectDependencies))] string expected) {

            PackageResolver sut = new(true);

            IReadOnlyList<string> actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Does.Contain(expected));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetAllDependentPackagePaths_ThenListHasCount() {
            PackageResolver sut = new();

            IReadOnlyList<string> actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Is.SupersetOf(UtilitiesDirectDependencies.Concat(UtilitiesIndirectDependencies)));
        }

        [Test]
        public void GivenUtilitiesPath_WhenGetAllDependentPackagePaths_ThenListContainsPath(
            [ValueSource(nameof(UtilitiesIndirectDependencies))] string expected) {

            PackageResolver sut = new();

            IReadOnlyList<string> actual = sut.GetDependentPackagePaths(UTILITIES_CHANGELOG);

            Assert.That(actual, Does.Contain(expected));
        }
    }
}
