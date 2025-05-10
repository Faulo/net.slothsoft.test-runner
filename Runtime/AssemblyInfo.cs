using System.Runtime.CompilerServices;
using Slothsoft.TestRunner;

[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_EDITOR)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS_EDITMODE)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS_PLAYMODE)]

namespace Slothsoft.TestRunner {
    static class AssemblyInfo {
        public const string ID = "net.slothsoft.test-runner";

        public const string NAMESPACE_RUNTIME = "Slothsoft.TestRunner";
        public const string NAMESPACE_EDITOR = "Slothsoft.TestRunner.Editor";

        public const string NAMESPACE_TESTS_PLAYMODE = "Slothsoft.TestRunner.Tests.PlayMode";
        public const string NAMESPACE_TESTS_EDITMODE = "Slothsoft.TestRunner.Tests.EditMode";

        public const string NAMESPACE_PROXYGEN = "DynamicProxyGenAssembly2";
    }
}