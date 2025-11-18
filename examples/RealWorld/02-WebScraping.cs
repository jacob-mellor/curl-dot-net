using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CurlDotNet;

namespace CurlDotNet.Examples.RealWorld
{
    /// <summary>
    /// Example: Web scraping with CurlDotNet
    /// </summary>
    public class WebScrapingExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Web Scraping Example ===\n");

            // Example 1: Basic scraping with user agent
            Console.WriteLine("Example 1: Scrape with proper user agent");
            var result1 = await Curl.GetAsync("https://example.com")
                .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")
                .WithHeader("Accept", "text/html,application/xhtml+xml")
                .WithHeader("Accept-Language", "en-US,en;q=0.9")
                .ExecuteAsync();

            if (result1.IsSuccess)
            {
                // Extract title using regex
                var titleMatch = Regex.Match(result1.Body, @"<title>(.*?)</title>", RegexOptions.IgnoreCase);
                if (titleMatch.Success)
                {
                    Console.WriteLine($"Page title: {titleMatch.Groups[1].Value}");
                }

                // Extract all links
                var linkMatches = Regex.Matches(result1.Body, @"href=[""'](https?://[^""']+)[""']", RegexOptions.IgnoreCase);
                Console.WriteLine($"Found {linkMatches.Count} external links\n");
            }

            // Example 2: Scrape with session cookies
            Console.WriteLine("Example 2: Maintain session with cookies");

            // First request - login
            var loginResult = await Curl.PostAsync("https://example.com/login")
                .WithFormData(new {
                    username = "user@example.com",
                    password = "password123"
                })
                .ExecuteAsync();

            // Extract cookies from response
            string cookies = "";
            if (loginResult.Headers.ContainsKey("Set-Cookie"))
            {
                cookies = loginResult.Headers["Set-Cookie"];
                Console.WriteLine($"Session cookie received: {cookies.Substring(0, Math.Min(50, cookies.Length))}...");
            }

            // Second request - use session cookie
            var protectedResult = await Curl.GetAsync("https://example.com/dashboard")
                .WithHeader("Cookie", cookies)
                .ExecuteAsync();

            Console.WriteLine($"Dashboard access: {(protectedResult.IsSuccess ? "Success" : "Failed")}\n");

            // Example 3: Parallel scraping with rate limiting
            Console.WriteLine("Example 3: Parallel scraping with rate limit");
            var urls = new[] {
                "https://example.com/page1",
                "https://example.com/page2",
                "https://example.com/page3",
                "https://example.com/page4",
                "https://example.com/page5"
            };

            var semaphore = new System.Threading.SemaphoreSlim(2); // Max 2 concurrent requests
            var tasks = new List<Task>();

            foreach (var url in urls)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var result = await Curl.GetAsync(url)
                            .WithUserAgent("CurlDotNet-Scraper/1.0")
                            .ExecuteAsync();
                        Console.WriteLine($"Scraped {url}: {result.StatusCode}");

                        // Add delay to respect rate limits
                        await Task.Delay(1000);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine();

            // Example 4: Scrape JavaScript-rendered content (using headless browser endpoint)
            Console.WriteLine("Example 4: Scrape JS-rendered content via API");
            var jsResult = await Curl.PostAsync("https://api.browserless.io/content")
                .WithBearerToken("YOUR_BROWSERLESS_TOKEN")
                .WithJson(new {
                    url = "https://example.com/spa-app",
                    waitFor = 3000, // Wait for JS to render
                    screenshot = false,
                    elements = new[] {
                        new { selector = "h1", property = "innerText" },
                        new { selector = ".price", property = "innerText" },
                        new { selector = "img", property = "src" }
                    }
                })
                .ExecuteAsync();

            if (jsResult.IsSuccess)
            {
                Console.WriteLine("JavaScript content scraped successfully");
            }

            // Example 5: Scrape with proxy rotation
            Console.WriteLine("\nExample 5: Scrape with rotating proxies");
            string[] proxies = {
                "http://proxy1.provider.com:8080",
                "http://proxy2.provider.com:8080",
                "http://proxy3.provider.com:8080"
            };

            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                var proxy = proxies[random.Next(proxies.Length)];
                var result = await Curl.GetAsync("https://httpbin.org/headers")
                    .WithProxy(proxy)
                    .WithUserAgent($"Bot-{i}")
                    .ExecuteAsync();

                if (result.IsSuccess)
                {
                    Console.WriteLine($"Request {i + 1} via {proxy}: Success");
                }

                await Task.Delay(500); // Be respectful
            }

            // Example 6: Extract structured data
            Console.WriteLine("\nExample 6: Extract structured data");
            var productResult = await Curl.GetAsync("https://example.com/product/123")
                .WithUserAgent("Mozilla/5.0 Compatible CurlDotNet")
                .ExecuteAsync();

            if (productResult.IsSuccess)
            {
                // Extract product information
                var product = new {
                    Title = ExtractContent(productResult.Body, @"<h1[^>]*>(.*?)</h1>"),
                    Price = ExtractContent(productResult.Body, @"class=[""']price[""'][^>]*>(.*?)</"),
                    Description = ExtractContent(productResult.Body, @"class=[""']description[""'][^>]*>(.*?)</"),
                    ImageUrl = ExtractContent(productResult.Body, @"<img[^>]*src=[""'](.*?)[""']")
                };

                Console.WriteLine($"Product: {product.Title}");
                Console.WriteLine($"Price: {product.Price}");
                Console.WriteLine($"Image: {product.ImageUrl}");
            }

            // Example 7: Handle pagination
            Console.WriteLine("\nExample 7: Scrape paginated results");
            var allResults = new List<string>();
            int page = 1;
            bool hasMore = true;

            while (hasMore && page <= 5) // Limit to 5 pages
            {
                var pageResult = await Curl.GetAsync($"https://example.com/search?page={page}")
                    .WithUserAgent("CurlDotNet-Pagination")
                    .ExecuteAsync();

                if (pageResult.IsSuccess)
                {
                    var items = Regex.Matches(pageResult.Body, @"class=[""']item[""'][^>]*>(.*?)</div>", RegexOptions.Singleline);
                    Console.WriteLine($"Page {page}: Found {items.Count} items");

                    foreach (Match item in items)
                    {
                        allResults.Add(item.Groups[1].Value);
                    }

                    // Check if there's a next page
                    hasMore = pageResult.Body.Contains($"page={page + 1}");
                    page++;

                    await Task.Delay(1000); // Rate limiting
                }
                else
                {
                    hasMore = false;
                }
            }

            Console.WriteLine($"Total items collected: {allResults.Count}");
        }

        private static string ExtractContent(string html, string pattern)
        {
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }
    }
}