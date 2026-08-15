using System.Collections.Generic;

using Touchstone.Core;

namespace Test.Shared
{
    /// <summary>
    /// Central source of truth for the GoogleMapsClient test suite.
    ///
    /// Every test case for the library is defined here once as a framework-agnostic Touchstone
    /// descriptor. The same descriptors are executed by:
    ///   * Test.Automated  (Touchstone.Cli console runner)
    ///   * Test.Xunit      (Touchstone xUnit adapter)
    ///   * Test.Nunit      (Touchstone NUnit adapter)
    ///
    /// This guarantees all three runners exercise an identical set of assertions.
    /// </summary>
    public static class GoogleMapsSuites
    {
        /// <summary>
        /// All test suites for the library, in a stable execution order.
        /// </summary>
        public static IReadOnlyList<TestSuiteDescriptor> All
        {
            get
            {
                return new List<TestSuiteDescriptor>
                {
                    ClientSuites.ConstructionSuite(),
                    ClientSuites.PropertySuite(),
                    ClientSuites.ArgumentValidationSuite(),
                    ClientSuites.DisposalSuite(),
                    AddressSuite.AddressParsingSuite(),
                    ModelSuites.ResponseModelSuite(),
                    ModelSuites.CoordinatesSuite(),
                    ModelSuites.TimestampSuite(),
                    ModelSuites.SerializationSuite(),
                    IntegrationSuite.GeocodeIntegrationSuite()
                };
            }
        }
    }
}
