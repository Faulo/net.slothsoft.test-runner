using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor.Validation.Internal;
using UnityEngine.TestTools;

namespace Slothsoft.TestRunner.Editor.Validation {
    [TestMustExpectAllLogs(false)]
    public abstract class DependencyValidationBase<T> where T : IPackageSource, new() {
        public static IEnumerable<string> AllPackageIds {
            get {
                SortedSet<string> allPackageIds = new(new T().GetPackageIds(), StringComparer.InvariantCultureIgnoreCase);

                if (allPackageIds.Count == 0) {
                    allPackageIds.Add(string.Empty);
                }

                return allPackageIds;
            }
        }

        readonly IPackageResolver resolver = new PackageResolver(false);

        [Test]
        public void CheckForCircularDependencies([ValueSource(nameof(AllPackageIds))] string packageId) {
            if (string.IsNullOrEmpty(packageId)) {
                Assert.Ignore("No dependencies to check.");
                return;
            }

            var package = resolver.GetPackageInfo(packageId);

            Assert.That(package, Is.Not.Null, $"Failed to load package '{packageId}'.");

            foreach (string dependencyId in package.resolvedDependencies.Select(d => d.name)) {
                var dependency = resolver.GetPackageInfo(dependencyId);

                if (dependency is null) {
                    continue;
                }

                Assert.That(
                    dependency.resolvedDependencies.Select(d => d.name),
                    Does.Not.Contain(packageId),
                    $"Package '{packageId}' contains a circular dependency to itself via package '{dependencyId}'!"
                );
            }
        }
    }
}
