using System;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Examples.BasicExamples
{
    /// <summary>
    /// Example: Proper error handling with CurlDotNet
    /// </summary>
    public class ErrorHandlingExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Error Handling Example ===\n");

            // Example 1: Check IsSuccess property
            Console.WriteLine("Example 1: Using IsSuccess property");
            var result1 = await Curl.ExecuteAsync("curl https://httpstat.us/404");
            if (!result1.IsSuccess)
            {
                Console.WriteLine($"❌ Request failed with status: {result1.StatusCode}");
                Console.WriteLine($"   Response: {result1.Body}\n");
            }

            // Example 2: Handle specific HTTP errors
            Console.WriteLine("Example 2: Handle specific status codes");
            var result2 = await Curl.ExecuteAsync("curl https://httpstat.us/500");
            switch (result2.StatusCode)
            {
                case 404:
                    Console.WriteLine("Resource not found");
                    break;
                case 500:
                    Console.WriteLine("Server error occurred");
                    break;
                case 401:
                    Console.WriteLine("Authentication required");
                    break;
                default:
                    if (result2.IsSuccess)
                        Console.WriteLine("Request succeeded");
                    else
                        Console.WriteLine($"Request failed with status: {result2.StatusCode}");
                    break;
            }

            // Example 3: Try-catch with specific exceptions
            Console.WriteLine("\nExample 3: Exception handling");
            try
            {
                // This will throw because of invalid URL
                await Curl.ExecuteAsync("curl not-a-valid-url");
            }
            catch (CurlMalformedUrlException ex)
            {
                Console.WriteLine($"❌ Invalid URL: {ex.Message}");
            }
            catch (CurlException ex)
            {
                Console.WriteLine($"❌ Curl error: {ex.Message}");
            }

            // Example 4: Timeout handling
            Console.WriteLine("\nExample 4: Timeout handling");
            try
            {
                var result = await Curl.GetAsync("https://httpstat.us/200?sleep=5000")
                    .WithTimeout(TimeSpan.FromSeconds(2))
                    .ExecuteAsync();
            }
            catch (CurlOperationTimeoutException ex)
            {
                Console.WriteLine($"❌ Request timed out after {ex.TimeoutSeconds} seconds");
            }

            // Example 5: Connection errors
            Console.WriteLine("\nExample 5: Connection error handling");
            try
            {
                await Curl.ExecuteAsync("curl https://this-domain-definitely-does-not-exist-12345.com");
            }
            catch (CurlCouldntResolveHostException ex)
            {
                Console.WriteLine($"❌ Could not resolve host: {ex.Host}");
            }
            catch (CurlCouldntConnectException ex)
            {
                Console.WriteLine($"❌ Could not connect to {ex.Host}:{ex.Port}");
            }

            // Example 6: Retry on failure
            Console.WriteLine("\nExample 6: Retry logic");
            int retries = 3;
            for (int i = 0; i < retries; i++)
            {
                var result = await Curl.ExecuteAsync("curl https://httpstat.us/503");
                if (result.IsSuccess)
                {
                    Console.WriteLine("✅ Request succeeded");
                    break;
                }
                else if (i < retries - 1)
                {
                    Console.WriteLine($"Attempt {i + 1} failed, retrying...");
                    await Task.Delay(1000 * (i + 1)); // Exponential backoff
                }
                else
                {
                    Console.WriteLine($"❌ All {retries} attempts failed");
                }
            }
        }
    }
}