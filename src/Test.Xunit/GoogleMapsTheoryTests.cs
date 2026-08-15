using System.Threading;
using System.Threading.Tasks;

using Test.Shared;
using Touchstone.Core;
using Xunit;

namespace Test.Xunit
{
    /// <summary>
    /// xUnit host for the shared Touchstone suites. Each non-skipped <see cref="TestCaseDescriptor"/>
    /// becomes a separate theory row so it is discovered and reported individually by
    /// <c>dotnet test</c>, Visual Studio, and other xUnit-aware runners.
    /// </summary>
    public sealed class GoogleMapsTheoryTests
    {
        /// <summary>
        /// Flattens every non-skipped shared test case into xUnit theory data.
        /// </summary>
        public static TheoryData<TestCaseDescriptor> TestCases()
        {
            TheoryData<TestCaseDescriptor> data = new TheoryData<TestCaseDescriptor>();

            foreach (TestSuiteDescriptor suite in GoogleMapsSuites.All)
            {
                foreach (TestCaseDescriptor testCase in suite.Cases)
                {
                    if (!testCase.Skip)
                        data.Add(testCase);
                }
            }

            return data;
        }

        /// <summary>
        /// Executes a single shared test case.
        /// </summary>
        /// <param name="testCase">The shared descriptor to run.</param>
        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunTest(TestCaseDescriptor testCase)
        {
            await testCase.ExecuteAsync(CancellationToken.None);
        }
    }
}
