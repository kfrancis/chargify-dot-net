using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Bogus;
using ChargifyDotNet.Tests.Base; // settings class
using ChargifyNET;
using Microsoft.Extensions.Configuration; // added
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNet.Tests
{
    public abstract class ChargifyTestBase
    {
        // ReSharper disable once MemberCanBeProtected.Global
        public TestContext TestContext { get; set; } = null!;
        private Stopwatch? _testSw;

        [TestInitialize]
        public void BaseInit()
        {
            _testSw = Stopwatch.StartNew();
            TestContext.WriteLine($"[START] {TestContext.TestName} @ {DateTimeOffset.Now:O}");
        }

        [TestCleanup]
        public void BaseCleanup()
        {
            _testSw?.Stop();
            TestContext.WriteLine($"[END] {TestContext.TestName} after {_testSw?.Elapsed.TotalMilliseconds:F0} ms");
        }

        /// <summary>
        /// Writes a message to the current test output stream.
        /// </summary>
        /// <remarks>This method is typically used to provide additional diagnostic information during
        /// test execution. The output is visible in the test results or logs, depending on the test runner.</remarks>
        /// <param name="message">The message to write to the test output. Can be null or empty.</param>
        [Conditional("DEBUG")]
        protected void Log(string message) => TestContext.WriteLine(message);

        [Conditional("DEBUG")]
        protected void Log(string format, params object?[] args) => TestContext.WriteLine(format, args);

        /// <summary>
        /// Begins a timed test step with the specified name and returns a disposable scope that logs the step's
        /// completion and elapsed time when disposed.
        /// </summary>
        /// <remarks>Use this method to measure and log the duration of a specific test step. The step
        /// name is written to the test context at the start and upon completion, along with the elapsed time. This
        /// method is intended for use within a test framework that supports <see cref="TestContext"/> output.</remarks>
        /// <param name="name">The name of the test step to display in the log output. Cannot be null.</param>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, logs the completion of the test step along with the
        /// elapsed time in milliseconds.</returns>
        protected Scope Step(string name)
        {
            TestContext.WriteLine($"→ {name}...");
            return new Scope(this, name);
        }

        /// <summary>
        /// Provides a scope that executes a specified action when disposed.
        /// </summary>
        /// <remarks>Use this class to ensure that a cleanup or finalization action is performed when the
        /// scope is exited, typically in a using statement. This is useful for managing resources or performing custom
        /// teardown logic.</remarks>
        public sealed class Scope : IDisposable
        {
            private readonly ChargifyTestBase _owner;
            private readonly string _name;
            private readonly Stopwatch _sw = Stopwatch.StartNew();
            private bool _completed;

            internal Scope(ChargifyTestBase owner, string name)
            {
                _owner = owner;
                _name = name;
            }

            public void Complete(string? note = null)
            {
                if (_completed) return;
                _completed = true;
                _sw.Stop();
                var suffix = string.IsNullOrEmpty(note) ? "" : $" - {note}";
                _owner.TestContext.WriteLine($"✓ {_name} [{_sw.Elapsed.TotalMilliseconds:F0} ms]{suffix}");
            }

            public void Dispose()
            {
                if (!_completed) Complete("auto-completed on dispose");
            }
        }

        private static readonly Lazy<ChargifySettings> s_settings = new(LoadSettings);

        protected ChargifyConnect Chargify => _chargify ??= new ChargifyConnect
        {
            apiKey = s_settings.Value.ApiKey ?? string.Empty,
            Password = s_settings.Value.Password ?? string.Empty,
            URL = s_settings.Value.Url ?? string.Empty,
            SharedKey = s_settings.Value.SharedKey ?? string.Empty,
            UseJSON = s_settings.Value.UseJson,
            ProtocolType = ParseProtocol(s_settings.Value.Protocol)
        };

        private ChargifyConnect? _chargify;

        private static ChargifySettings LoadSettings()
        {
            // Base path: test project directory
            var basePath = AppContext.BaseDirectory;

            var configRootPath = FindSettingsDirectory(basePath);

            var builder = new ConfigurationBuilder()
                .SetBasePath(configRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables(prefix: "CHARGIFY_"); // allow env overrides e.g. CHARGIFY_ApiKey

            var config = builder.Build();
            var section = config.GetSection("Chargify");
            var settings = section.Get<ChargifySettings>() ?? new ChargifySettings();

            // Environment variable fallback (upper-case names) if binder didn't map
            settings.ApiKey ??= config["CHARGIFY_APIKEY"];
            settings.Password ??= config["CHARGIFY_PASSWORD"];
            settings.Url ??= config["CHARGIFY_URL"];
            settings.SharedKey ??= config["CHARGIFY_SHAREDKEY"];
            if (string.IsNullOrEmpty(settings.Protocol)) settings.Protocol = config["CHARGIFY_PROTOCOL"] ?? "Tls12";
            if (!settings.UseJson && bool.TryParse(config["CHARGIFY_USEJSON"], out var useJson)) settings.UseJson = useJson;

            return settings;

            // For net48 tests AppContext.BaseDirectory points to bin/Debug/net48; move up until project file directory if needed
            // We'll search upwards for appsettings.json if not found directly.
            static string FindSettingsDirectory(string start)
            {
                var dir = new DirectoryInfo(start);
                while (dir != null)
                {
                    var potential = Path.Combine(dir.FullName, "appsettings.json");
                    if (File.Exists(potential)) return dir.FullName;
                    dir = dir.Parent;
                }
                return start;
            }
        }

        private static SecurityProtocolType ParseProtocol(string protocol)
        {
            if (string.IsNullOrWhiteSpace(protocol)) return SecurityProtocolType.Tls12;
            try
            {
                return (SecurityProtocolType)Enum.Parse(typeof(SecurityProtocolType), protocol, ignoreCase: true);
            }
            catch
            {
                return SecurityProtocolType.Tls12;
            }
        }

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
