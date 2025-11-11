using System;
using System.Net;
using Bogus;
using ChargifyNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNet.Tests
{
    public class ChargifyTestBase
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        protected TestContext TestContext { get; set; }

        protected ChargifyConnect Chargify => _chargify ??= new ChargifyConnect
        {
            apiKey = "",
            Password = "X",
            URL = "https://subdomain.chargify.com/",
            SharedKey = "123456789",
            UseJSON = false,
            ProtocolType = SecurityProtocolType.Tls12
        };

        private ChargifyConnect _chargify;

        /// <summary>
        /// Method that allows me to use Faker methods in place rather than writing a bunch of specific "GetRandom.." methods.
        /// </summary>
        /// <param name="oldValue">The value that the result cannot be</param>
        /// <param name="generateValue">The method (that returns string) that will be used to generate the random value</param>
        /// <param name="generateArg">
        /// An argument to be passed to the generateValue function
        /// </param>
        /// <returns>A new random string value that isn't the same as the existing/old value</returns>
        protected static string GetNewRandomValue(string oldValue, Func<bool, string> generateValue, bool generateArg = false)
        {
            string retVal;
            do
            {
                retVal = generateValue(generateArg);
            }
            while (retVal == oldValue);
            return retVal;
        }

        /// <summary>
        /// Method that allows me to use Faker methods in place rather than writing a bunch of specific "GetRandom.." methods.
        /// </summary>
        /// <param name="oldValue">The value that the result cannot be</param>
        /// <param name="generateValue">The method (that returns string) that will be used to generate the random value</param>
        /// <returns>A new random string value that isn't the same as the existing/old value</returns>
        protected static string GetNewRandomValue(string oldValue, Func<string> generateValue)
        {
            string retVal;
            do
            {
                retVal = generateValue();
            }
            while (retVal == oldValue);
            return retVal;
        }

        /// <summary>
        /// Method that allows me to use Faker methods in place rather than writing a bunch of specific "GetRandom.." methods.
        /// </summary>
        /// <param name="oldValue">The value that the result cannot be</param>
        /// <param name="generateValue">The method (that returns string) that will be used to generate the random value</param>
        /// <param name="generateArg">An argument to be passed to the generateValue function</param>
        /// <returns>A new random string value that isn't the same as the existing/old value</returns>
        protected static string GetNewRandomValue(string oldValue, Func<string, string> generateValue, string generateArg = null)
        {
            string retVal;
            do
            {
                retVal = generateValue(generateArg);
            }
            while (retVal == oldValue);
            return retVal;
        }

        protected static Faker Faker => new();

        internal void SetJson(bool useJson)
        {
            if (Chargify != null)
            {
                _chargify.UseJSON = useJson;
            }
        }

        internal static int GetRandomNegativeInt()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode()) * -1;
        }

        /// <summary>
        /// Validates test data by executing the specified validation function and fails the test if the validation does
        /// not pass.
        /// </summary>
        /// <param name="validation">A function that returns <see langword="true"/> if the test data is valid; otherwise, <see
        /// langword="false"/>.</param>
        /// <param name="customFailureMessage">An optional custom message to include if the validation fails. If <see langword="null"/>, a default failure
        /// message is used.</param>
        internal static void ValidateRun(Func<bool> validation, string customFailureMessage = null)
        {//To prevent "multiple asserts" in a single test class this masks the
            //idea of having multiple asserts and allows us to verify all data is valid before running
            if (!validation())
                Assert.Fail(customFailureMessage ?? "The test setup resulted in invalid test data. Please resolve any issues before continuing");
        }

        /// <summary>
        /// Executes the specified action and asserts that it throws an exception, then invokes the provided assertion
        /// action on the thrown exception.
        /// </summary>
        /// <remarks>Use this method to verify that a particular action throws an exception and to perform
        /// further checks on the exception instance. If the action does not throw an exception, the test will
        /// fail.</remarks>
        /// <param name="runAttempt">The action to execute, which is expected to throw an exception.</param>
        /// <param name="runAssertions">An action to perform additional assertions on the exception that was thrown by the runAttempt action.</param>
        internal void AssertTheFollowingThrowsException(Action runAttempt, Action<Exception> runAssertions)
        {
            try
            {
                runAttempt();
                Assert.Fail("Attempt should have thrown an error but did not");
            }
            catch (Exception e)
            {
                runAssertions(e);
            }
        }

        /// <summary>
        /// Executes the specified action and verifies that it throws an exception of the specified type.
        /// </summary>
        /// <remarks>If the action does not throw an exception of type T, the test will fail. If the
        /// action throws a different type of exception, the test will also fail.</remarks>
        /// <typeparam name="T">The type of exception that is expected to be thrown by the action. Must derive from Exception.</typeparam>
        /// <param name="action">The action to execute. This action is expected to throw an exception of type T.</param>
        /// <returns>The exception of type T that was thrown by the action.</returns>
        internal T ExpectException<T>(Action action) where T : Exception
        {
            try
            {
                action();
                Assert.Fail($"Expected exception of type {typeof(T).Name} was not thrown");
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected {typeof(T).Name}, but {ex.GetType().Name} was thrown");
            }
            return null;
        }
    }
}
