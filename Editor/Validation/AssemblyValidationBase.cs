using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Slothsoft.TestRunner.Editor.Validation.Internal;

namespace Slothsoft.TestRunner.Editor.Validation {
    public abstract class AssemblyValidationBase<T> where T : IAssemblySource, new() {
        public static IEnumerable<string> allAssemblyNames => AssetUtils.SortAndAddEmpty(new T().GetAssemblyNames());

        [Test]
        public void VerifyFormatting([ValueSource(nameof(allAssemblyNames))] string assemblyName) {
            if (string.IsNullOrEmpty(assemblyName)) {
                Assert.Ignore("No assemblies to check.");
                return;
            }

            string projectFile = $"{assemblyName}.csproj";

            if (!File.Exists(projectFile)) {
                Assert.Inconclusive($"Project file '{projectFile}' not found.");
                return;
            }

            ProcessStartInfo startInfo = new() {
                FileName = "dotnet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("format");
            startInfo.ArgumentList.Add(projectFile);
            startInfo.ArgumentList.Add("--verify-no-changes");
            startInfo.ArgumentList.Add("--no-restore");

            using Process process = new() { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();

            string error = process.StandardError.ReadToEnd();

            Assert.That(process.ExitCode, Is.EqualTo(0), error);
        }
    }
}
