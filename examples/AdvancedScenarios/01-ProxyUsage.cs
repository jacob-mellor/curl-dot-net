using System;
using System.Threading.Tasks;
using CurlDotNet;

namespace CurlDotNet.Examples.AdvancedScenarios
{
    /// <summary>
    /// Example: Using proxies with CurlDotNet
    /// </summary>
    public class ProxyUsageExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Proxy Usage Example ===\n");

            // Example 1: Basic HTTP proxy
            Console.WriteLine("Example 1: HTTP Proxy");
            var result1 = await Curl.ExecuteAsync(@"
                curl -x http://proxy.company.com:8080 https://api.example.com/data
            ");
            Console.WriteLine($"Status via HTTP proxy: {result1.StatusCode}\n");

            // Example 2: Proxy with authentication
            Console.WriteLine("Example 2: Proxy with authentication");
            var result2 = await Curl.GetAsync("https://api.example.com/secure")
                .WithProxy("http://proxy.company.com:8080")
                .WithProxyAuth("proxyuser", "proxypass")
                .ExecuteAsync();
            Console.WriteLine($"Status via authenticated proxy: {result2.StatusCode}\n");

            // Example 3: SOCKS5 proxy (Tor)
            Console.WriteLine("Example 3: SOCKS5 Proxy (Tor)");
            var result3 = await Curl.GetAsync("https://check.torproject.org")
                .WithSocks5Proxy("socks5://127.0.0.1:9050")
                .ExecuteAsync();
            Console.WriteLine($"Tor connection: {(result3.Body.Contains("Congratulations") ? "Success" : "Not using Tor")}\n");

            // Example 4: Rotating proxy / Backconnect
            Console.WriteLine("Example 4: Rotating proxy for web scraping");
            string[] proxies = {
                "http://proxy1.provider.com:8000",
                "http://proxy2.provider.com:8000",
                "http://proxy3.provider.com:8000"
            };

            for (int i = 0; i < 3; i++)
            {
                var proxy = proxies[i % proxies.Length];
                var result = await Curl.GetAsync($"https://httpbin.org/ip")
                    .WithProxy(proxy)
                    .WithProxyAuth("user-session-" + Guid.NewGuid(), "password")
                    .ExecuteAsync();

                if (result.IsSuccess)
                {
                    Console.WriteLine($"Request {i + 1} via {proxy}: Success");
                }
            }

            // Example 5: Proxy with custom headers (residential proxies)
            Console.WriteLine("\nExample 5: Residential proxy with session");
            var sessionId = Guid.NewGuid().ToString();
            var result5 = await Curl.GetAsync("https://api.example.com/data")
                .WithProxy("http://gate.smartproxy.com:10000")
                .WithProxyAuth($"user-country-us-session-{sessionId}", "password")
                .WithProxyHeader("X-Session-ID", sessionId)
                .WithProxyHeader("X-Country", "US")
                .ExecuteAsync();
            Console.WriteLine($"Residential proxy status: {result5.StatusCode}");

            // Example 6: No proxy for internal domains
            Console.WriteLine("\nExample 6: Bypass proxy for internal");
            var result6 = await Curl.ExecuteAsync(@"
                curl --noproxy '*.internal.company.com,192.168.*,localhost' \
                     -x http://proxy.company.com:8080 \
                     https://api.internal.company.com/data
            ");
            Console.WriteLine($"Internal request (no proxy): {result6.StatusCode}");

            // Example 7: Proxy chains
            Console.WriteLine("\nExample 7: Chained proxies");
            var result7 = await Curl.ExecuteAsync(@"
                curl --proxy1.0 http://first-proxy:8080 \
                     --proxy http://second-proxy:8888 \
                     https://api.example.com
            ");
            Console.WriteLine($"Via proxy chain: {result7.StatusCode}");
        }
    }
}