using System;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Examples.AdvancedScenarios
{
    /// <summary>
    /// Example: Using proxies with CurlDotNet.
    /// The proxy hosts below are placeholders - replace them with your own. Each request is
    /// wrapped in TryAsync so an unreachable proxy prints an error instead of ending the sample.
    /// </summary>
    public class ProxyUsageExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Proxy Usage Example ===\n");

            // Example 1: Basic HTTP proxy with -x
            Console.WriteLine("Example 1: HTTP Proxy");
            await TryAsync("HTTP proxy", () =>
                Curl.ExecuteAsync("curl -x http://proxy.company.com:8080 https://httpbin.org/ip"));

            // Example 2: Proxy with authentication (fluent builder)
            Console.WriteLine("\nExample 2: Proxy with authentication");
            await TryAsync("Authenticated proxy", () =>
                CurlRequestBuilder.Get("https://httpbin.org/ip")
                    .WithProxy("http://proxy.company.com:8080", "proxyuser", "proxypass")
                    .ExecuteAsync());

            // Example 3: SOCKS5 proxy (Tor) - available through the --socks5 flag
            Console.WriteLine("\nExample 3: SOCKS5 Proxy (Tor)");
            await TryAsync("Tor", async () =>
            {
                var result = await Curl.ExecuteAsync("curl --socks5 127.0.0.1:9050 https://check.torproject.org");
                Console.WriteLine($"  Using Tor: {result.Body.Contains("Congratulations")}");
                return result;
            });

            // Example 4: Rotating proxies for web scraping
            Console.WriteLine("\nExample 4: Rotating proxy for web scraping");
            string[] proxies =
            {
                "http://proxy1.provider.com:8000",
                "http://proxy2.provider.com:8000",
                "http://proxy3.provider.com:8000"
            };

            for (int i = 0; i < 3; i++)
            {
                var proxy = proxies[i % proxies.Length];
                await TryAsync($"Request {i + 1} via {proxy}", () =>
                    CurlRequestBuilder.Get("https://httpbin.org/ip")
                        .WithProxy(proxy)
                        .WithTimeout(TimeSpan.FromSeconds(10))
                        .ExecuteAsync());
            }

            // Example 5: Sticky sessions - many residential providers encode the session in the username
            Console.WriteLine("\nExample 5: Residential proxy with session");
            var sessionId = Guid.NewGuid().ToString("N");
            await TryAsync("Residential proxy", () =>
                CurlRequestBuilder.Get("https://httpbin.org/ip")
                    .WithProxy("http://gate.provider.com:10000", $"user-country-us-session-{sessionId}", "password")
                    .ExecuteAsync());

            // Example 6: Proxy credentials in a pasted command with --proxy-user
            Console.WriteLine("\nExample 6: Proxy credentials in a command");
            await TryAsync("--proxy-user", () => Curl.ExecuteAsync(@"
                curl -x http://proxy.company.com:8080 \
                     --proxy-user proxyuser:proxypass \
                     https://httpbin.org/ip
            "));

            // Example 7: See the equivalent curl command - handy for debugging proxy setups
            Console.WriteLine("\nExample 7: Builder to curl command");
            var command = CurlRequestBuilder.Get("https://httpbin.org/ip")
                .WithProxy("http://proxy.company.com:8080")
                .WithTimeout(TimeSpan.FromSeconds(10))
                .ToCurlCommand();
            Console.WriteLine($"  {command}");
        }

        // Runs one request and reports the outcome; proxy failures surface as CurlException subclasses
        private static async Task TryAsync(string label, Func<Task<CurlResult>> request)
        {
            try
            {
                var result = await request();
                Console.WriteLine($"  {label}: HTTP {result.StatusCode}");
            }
            catch (CurlException ex)
            {
                Console.WriteLine($"  {label}: failed ({ex.GetType().Name}, curl exit code {ex.CurlErrorCode})");
            }
        }
    }
}
