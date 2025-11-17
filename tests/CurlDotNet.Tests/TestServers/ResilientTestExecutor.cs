using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CurlDotNet.Tests.TestServers
{
    /// <summary>
    /// Provides resilient test execution with automatic server fallback.
    /// When a test fails with one server, automatically retries with alternatives.
    /// </summary>
    public class ResilientTestExecutor
    {
        private readonly ITestOutputHelper _output;
        private readonly List<TestServerEndpoint> _servers;
        private int _maxRetries;

        public ResilientTestExecutor(ITestOutputHelper output = null, int maxRetries = 3)
        {
            _output = output;
            _maxRetries = maxRetries;
            _servers = TestServerConfiguration.AvailableServers
                .Where(s => s.Features.HasFlag(TestServerFeatures.All))
                .OrderBy(s => s.Priority)
                .ToList();
        }

        /// <summary>
        /// Execute a test with automatic retry on different servers if it fails.
        /// </summary>
        public async Task<T> ExecuteWithFallbackAsync<T>(
            Func<string, Task<T>> testAction,
            string testName = "Test",
            TestServerFeatures requiredFeatures = TestServerFeatures.Basic)
        {
            var candidateServers = _servers
                .Where(s => s.Features.HasFlag(requiredFeatures))
                .Take(_maxRetries)
                .ToList();

            List<Exception> failures = new List<Exception>();

            foreach (var server in candidateServers)
            {
                try
                {
                    LogInfo($"Attempting {testName} with {server.Name} ({server.BaseUrl})");

                    // Quick health check first
                    if (!await IsServerHealthyQuickAsync(server))
                    {
                        LogInfo($"Server {server.Name} not responding, trying next...");
                        continue;
                    }

                    var result = await testAction(server.BaseUrl);
                    LogInfo($"✅ Success with {server.Name}!");
                    return result;
                }
                catch (Exception ex)
                {
                    failures.Add(ex);
                    LogInfo($"❌ Failed with {server.Name}: {ex.Message}");

                    if (server == candidateServers.Last())
                    {
                        // Last server, throw aggregate exception
                        throw new AggregateException(
                            $"Test '{testName}' failed with all {candidateServers.Count} servers",
                            failures);
                    }

                    LogInfo($"Retrying with next server...");
                }
            }

            throw new InvalidOperationException("No servers available for testing");
        }

        /// <summary>
        /// Execute a synchronous test with automatic retry.
        /// </summary>
        public T ExecuteWithFallback<T>(
            Func<string, T> testAction,
            string testName = "Test",
            TestServerFeatures requiredFeatures = TestServerFeatures.Basic)
        {
            return ExecuteWithFallbackAsync(
                async (url) => await Task.FromResult(testAction(url)),
                testName,
                requiredFeatures).Result;
        }

        /// <summary>
        /// Execute a void test with automatic retry.
        /// </summary>
        public async Task ExecuteWithFallbackAsync(
            Func<string, Task> testAction,
            string testName = "Test",
            TestServerFeatures requiredFeatures = TestServerFeatures.Basic)
        {
            await ExecuteWithFallbackAsync(async (url) =>
            {
                await testAction(url);
                return true;
            }, testName, requiredFeatures);
        }

        /// <summary>
        /// Execute a synchronous void test with automatic retry.
        /// </summary>
        public void ExecuteWithFallback(
            Action<string> testAction,
            string testName = "Test",
            TestServerFeatures requiredFeatures = TestServerFeatures.Basic)
        {
            ExecuteWithFallbackAsync((url) =>
            {
                testAction(url);
                return Task.CompletedTask;
            }, testName, requiredFeatures).Wait();
        }

        private async Task<bool> IsServerHealthyQuickAsync(TestServerEndpoint server)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(2) })
                {
                    var response = await client.GetAsync($"{server.BaseUrl}{server.HealthCheckPath}");
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private void LogInfo(string message)
        {
            _output?.WriteLine($"[ResilientTest] {message}");
            Console.WriteLine($"[ResilientTest] {message}");
        }

        /// <summary>
        /// Get the best available server URL for a specific feature set.
        /// </summary>
        public static async Task<string> GetBestServerUrlAsync(TestServerFeatures features = TestServerFeatures.Basic)
        {
            var server = await TestServerConfiguration.GetBestAvailableServerAsync(features);
            return server.BaseUrl;
        }
    }
}