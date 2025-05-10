using System.Runtime.CompilerServices;
using NUnit.Framework;
using Slothsoft.TestRunner;
using UnityEngine.TestTools;

[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_PROXYGEN)]
[assembly: TestMustExpectAllLogs]
[assembly: Category(AssemblyInfo.NAMESPACE_RUNTIME)]