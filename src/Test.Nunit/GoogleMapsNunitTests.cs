using System.Collections;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using Test.Shared;
using Touchstone.Core;
using Touchstone.NunitAdapter;

namespace Test.Nunit
{
    /// <summary>
    /// NUnit host for the shared Touchstone suites. <see cref="TouchstoneTestCaseSource"/> projects
    /// every shared <see cref="TestCaseDescriptor"/> into an NUnit test case (honoring skip flags),
    /// so each is discovered and reported individually by <c>dotnet test</c> and the NUnit runners.
    /// </summary>
    [TestFixture]
    public sealed class GoogleMapsNunitTests
    {
        private static IEnumerable TestCases()
        {
            return new TouchstoneTestCaseSource(GoogleMapsSuites.All);
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public async Task RunTest(TestCaseDescriptor testCase)
        {
            await testCase.ExecuteAsync(CancellationToken.None);
        }
    }
}
