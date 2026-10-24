using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Examples.BasicExamples
{
    /// <summary>
    /// Example: Proper error handling with CurlDotNet.
    /// HTTP error statuses come back as normal results; transport failures
    /// (bad URL, timeout, unreachable host) throw typed exceptions.
    /// </summary>
    public class ErrorHandlingExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Error Handling Example ===\n");

            // Example 1: Check the IsSuccess property (true for 2xx)
            Console.WriteLine("Example 1: Using IsSuccess property");
            var result1 = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");
            if (!result1.IsSuccess)
            {
                Console.WriteLine($"Request failed with status: {result1.StatusCode}\n");
            }

            // Example 2: Handle specific HTTP status codes
            Console.WriteLine("Example 2: Handle specific status codes");
            var result2 = await Curl.ExecuteAsync("curl https://httpbin.org/status/500");
            switch (result2.StatusCode)
            {
                case 404: Console.WriteLine("Resource not found"); break;
                case 500: Console.WriteLine("Server error occurred"); break;
                case 401: Console.WriteLine("Authentication required"); break;
                default: Console.WriteLine(result2.IsSuccess ? "Request succeeded" : $"Failed: {result2.StatusCode}"); break;
            }

            // Example 3: Turn a bad status into an exception with EnsureSuccess()
            Console.WriteLine("\nExample 3: EnsureSuccess()");
            try
            {
                (await Curl.GetAsync("https://httpbin.org/status/403")).EnsureSuccess();
            }
            catch (CurlDotNet.Core.CurlHttpException ex) // qualified: Exceptions has a CurlHttpException too
            {
                Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
            }

            // Example 4: Malformed URL (the port is out of range)
            Console.WriteLine("\nExample 4: Malformed URL");
            try
            {
                await Curl.ExecuteAsync("curl https://example.com:99999/");
            }
            catch (CurlMalformedUrlException ex)
            {
                Console.WriteLine($"Invalid URL: {ex.Message}");
            }

            // Example 5: Timeout handling (the server waits 5 seconds, we allow 2)
            Console.WriteLine("\nExample 5: Timeout handling");
            try
            {
                await CurlRequestBuilder.Get("https://httpbin.org/delay/5")
                    .WithTimeout(TimeSpan.FromSeconds(2))
                    .ExecuteAsync();
            }
            catch (CurlOperationTimeoutException ex)
            {
                Console.WriteLine($"Timed out after {ex.Timeout?.TotalSeconds} seconds (curl exit code {ex.CurlErrorCode})");
            }

            // Example 6: Connection errors - a host that cannot be resolved or reached
            Console.WriteLine("\nExample 6: Connection error handling");
            try
            {
                await Curl.ExecuteAsync("curl https://this-domain-definitely-does-not-exist-12345.com");
            }
            catch (CurlCouldntConnectException ex)
            {
                Console.WriteLine($"Could not connect to {ex.Host}:{ex.Port}");
            }
            catch (CurlException ex) // base class for every curl failure
            {
                Console.WriteLine($"Curl error {ex.CurlErrorCode}: {ex.Message}");
            }

            // Example 7: Caller cancellation surfaces as OperationCanceledException
            Console.WriteLine("\nExample 7: Cancellation");
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                try
                {
                    await Curl.ExecuteAsync("curl https://httpbin.org/delay/5", cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Request cancelled by the caller");
                }
            }

            // Example 8: Built-in retries (retries any failure with a fixed delay)
            Console.WriteLine("\nExample 8: Retry logic");
            var settings = new CurlSettings().WithRetries(3, delayMs: 1000);
            var retried = await Curl.ExecuteAsync("curl https://httpbin.org/status/503", settings);
            Console.WriteLine(retried.IsSuccess
                ? "Request succeeded"
                : $"Still failing after retries: {retried.StatusCode}");
        }
    }
}
