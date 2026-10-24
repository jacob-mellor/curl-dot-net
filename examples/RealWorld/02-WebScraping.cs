using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Examples.RealWorld
{
    /// <summary>
    /// Example: Web scraping with CurlDotNet - headers, cookies, polite parallelism,
    /// proxies, data extraction and pagination. Regex keeps the sample dependency-free;
    /// use an HTML parser such as AngleSharp for real projects.
    /// </summary>
    public class WebScrapingExample
    {
        private const string BrowserUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";

        public static async Task RunAsync()
        {
            Console.WriteLine("=== Web Scraping Example ===\n");

            // Example 1: Basic scraping with a browser-like user agent
            // (quotes.toscrape.com is a sandbox site built for scraping practice)
            Console.WriteLine("Example 1: Scrape with proper user agent");
            var result1 = await CurlRequestBuilder.Get("https://quotes.toscrape.com/")
                .WithUserAgent(BrowserUserAgent)
                .WithHeader("Accept", "text/html,application/xhtml+xml")
                .WithHeader("Accept-Language", "en-US,en;q=0.9")
                .ExecuteAsync();

            if (result1.IsSuccess)
            {
                Console.WriteLine($"Page title: {ExtractContent(result1.Body, @"<title>(.*?)</title>")}");
                var links = Regex.Matches(result1.Body, @"href=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                Console.WriteLine($"Found {links.Count} links\n");
            }

            // Example 2: Session cookies - read Set-Cookie, send it back on the next request
            Console.WriteLine("Example 2: Maintain session with cookies");
            var setResult = await Curl.ExecuteAsync("curl https://httpbin.org/cookies/set?session=abc123");
            var setCookie = setResult.GetHeader("Set-Cookie") ?? "";   // e.g. "session=abc123; Path=/"
            var sessionCookie = setCookie.Split(';')[0];
            Console.WriteLine($"Session cookie received: {sessionCookie}");

            var echoResult = await CurlRequestBuilder.Get("https://httpbin.org/cookies")
                .WithCookie(sessionCookie)
                .ExecuteAsync();
            Console.WriteLine($"Server saw cookies: {echoResult.Body.Replace("\n", "").Replace(" ", "")}\n");

            // Example 3: Parallel scraping with a concurrency limit
            Console.WriteLine("Example 3: Parallel scraping with rate limit");
            var urls = Enumerable.Range(1, 5).Select(i => $"https://httpbin.org/anything/page{i}");
            using var semaphore = new SemaphoreSlim(2); // at most 2 requests in flight

            var tasks = urls.Select(async url =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var result = await CurlRequestBuilder.Get(url).WithUserAgent("CurlDotNet-Scraper/1.0").ExecuteAsync();
                    Console.WriteLine($"Scraped {url}: {result.StatusCode}");
                    await Task.Delay(1000); // be polite between requests
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(tasks);

            // Example 4: Route a request through a proxy (placeholder host - replace with yours)
            Console.WriteLine("\nExample 4: Scrape through a proxy");
            try
            {
                var viaProxy = await CurlRequestBuilder.Get("https://httpbin.org/headers")
                    .WithProxy("http://proxy1.provider.com:8080")
                    .WithTimeout(TimeSpan.FromSeconds(10))
                    .ExecuteAsync();
                Console.WriteLine($"Via proxy: {viaProxy.StatusCode}");
            }
            catch (CurlException ex)
            {
                Console.WriteLine($"Proxy unavailable: {ex.GetType().Name}");
            }

            // Example 5: Extract structured data from a page
            Console.WriteLine("\nExample 5: Extract structured data");
            var bookResult = await Curl.GetAsync("https://httpbin.org/html");
            if (bookResult.IsSuccess)
            {
                var heading = ExtractContent(bookResult.Body, @"<h1[^>]*>(.*?)</h1>");
                var firstParagraph = ExtractContent(bookResult.Body, @"<p[^>]*>(.*?)</p>");
                Console.WriteLine($"Heading: {heading}");
                Console.WriteLine($"Opening: {firstParagraph.Substring(0, Math.Min(80, firstParagraph.Length))}...");
            }

            // Example 6: Handle pagination - follow "next" links until there are none
            Console.WriteLine("\nExample 6: Scrape paginated results");
            var allQuotes = new List<string>();
            string nextPath = "/page/1/";

            for (int page = 1; nextPath != null && page <= 3; page++) // limit to 3 pages
            {
                var pageResult = await Curl.GetAsync("https://quotes.toscrape.com" + nextPath);
                if (!pageResult.IsSuccess) break;

                var quotes = Regex.Matches(pageResult.Body, @"<span class=""text""[^>]*>(.*?)</span>");
                allQuotes.AddRange(quotes.Select(m => WebUtility.HtmlDecode(m.Groups[1].Value)));
                Console.WriteLine($"Page {page}: found {quotes.Count} quotes");

                var next = Regex.Match(pageResult.Body, @"<li class=""next"">\s*<a href=""([^""]+)""");
                nextPath = next.Success ? next.Groups[1].Value : null;
                await Task.Delay(500); // rate limiting
            }

            Console.WriteLine($"Total quotes collected: {allQuotes.Count}");
        }

        private static string ExtractContent(string html, string pattern)
        {
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }
    }
}
