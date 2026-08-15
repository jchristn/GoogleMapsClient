using System.Threading;
using System.Threading.Tasks;

using Test.Shared;
using Touchstone.Cli;

namespace Test.Automated
{
    /// <summary>
    /// Touchstone CLI runner for the GoogleMapsClient test suite. Executes every descriptor defined
    /// in <see cref="GoogleMapsSuites"/> and renders a colored pass/fail/skip table, returning a
    /// non-zero process exit code if any test fails (suitable for CI gating).
    ///
    /// Usage:
    ///   Test.Automated                     Run all suites.
    ///   Test.Automated --results out.json  Run all suites and also write JSON results to out.json.
    /// </summary>
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            string resultsPath = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--results" && i + 1 < args.Length)
                {
                    resultsPath = args[i + 1];
                    break;
                }
            }

            return await ConsoleRunner.RunAsync(
                GoogleMapsSuites.All,
                resultsPath: resultsPath,
                cancellationToken: CancellationToken.None);
        }
    }
}
