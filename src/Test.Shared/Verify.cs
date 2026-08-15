using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.Shared
{
    /// <summary>
    /// Minimal, framework-agnostic assertion helpers. Every failed assertion throws
    /// <see cref="VerificationException"/>, which Touchstone renders as a test failure
    /// regardless of the host runner (CLI, xUnit, or NUnit).
    /// </summary>
    internal static class Verify
    {
        internal static void IsTrue(bool condition, string message = null)
        {
            if (!condition) throw new VerificationException(message ?? "Expected condition to be true.");
        }

        internal static void IsFalse(bool condition, string message = null)
        {
            if (condition) throw new VerificationException(message ?? "Expected condition to be false.");
        }

        internal static void AreEqual<T>(T expected, T actual, string message = null)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new VerificationException(
                    (message ?? "Values are not equal.")
                    + " Expected: [" + Describe(expected) + "], Actual: [" + Describe(actual) + "].");
            }
        }

        internal static void AreEqual(double expected, double actual, double tolerance, string message = null)
        {
            if (Math.Abs(expected - actual) > tolerance)
            {
                throw new VerificationException(
                    (message ?? "Values are not within tolerance.")
                    + " Expected: [" + expected + "] +/- " + tolerance + ", Actual: [" + actual + "].");
            }
        }

        internal static void IsNull(object value, string message = null)
        {
            if (value != null) throw new VerificationException(message ?? "Expected value to be null, but it was not.");
        }

        internal static void IsNotNull(object value, string message = null)
        {
            if (value == null) throw new VerificationException(message ?? "Expected value to be non-null, but it was null.");
        }

        internal static void InRange(double value, double min, double max, string message = null)
        {
            if (value < min || value > max)
            {
                throw new VerificationException(
                    (message ?? "Value is out of range.")
                    + " Value: [" + value + "], expected range: [" + min + ", " + max + "].");
            }
        }

        /// <summary>
        /// Asserts that the supplied synchronous action throws an exception assignable to
        /// <typeparamref name="TException"/>. Returns the thrown exception for further inspection.
        /// </summary>
        internal static TException Throws<TException>(Action action, string message = null)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                throw new VerificationException(
                    (message ?? "Wrong exception type thrown.")
                    + " Expected: " + typeof(TException).Name + ", Actual: " + ex.GetType().Name + " (" + ex.Message + ").");
            }

            throw new VerificationException(
                (message ?? "No exception was thrown.")
                + " Expected: " + typeof(TException).Name + ".");
        }

        /// <summary>
        /// Asserts that the supplied asynchronous action throws an exception assignable to
        /// <typeparamref name="TException"/>. Returns the thrown exception for further inspection.
        /// </summary>
        internal static async Task<TException> ThrowsAsync<TException>(Func<Task> action, string message = null)
            where TException : Exception
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (TException ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                throw new VerificationException(
                    (message ?? "Wrong exception type thrown.")
                    + " Expected: " + typeof(TException).Name + ", Actual: " + ex.GetType().Name + " (" + ex.Message + ").");
            }

            throw new VerificationException(
                (message ?? "No exception was thrown.")
                + " Expected: " + typeof(TException).Name + ".");
        }

        private static string Describe(object value)
        {
            if (value == null) return "null";
            return value.ToString();
        }
    }

    /// <summary>
    /// Raised when a <see cref="Verify"/> assertion fails.
    /// </summary>
    internal sealed class VerificationException : Exception
    {
        internal VerificationException(string message) : base(message)
        {
        }
    }
}
