using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurlDotNet.Core;

namespace CurlDotNet.Examples.RealWorld
{
    /// <summary>
    /// Example: GitHub REST API integration with the fluent builder and typed JSON parsing.
    /// Set GITHUB_TOKEN to enable the authenticated calls and a higher rate limit.
    /// </summary>
    public class GitHubApiExample
    {
        private const string GITHUB_API = "https://api.github.com";
        private readonly string _token;

        public GitHubApiExample(string token = null)
        {
            _token = token ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        }

        // Every call shares the same headers, so build them in one place
        private CurlRequestBuilder GitHub(string method, string path)
        {
            var builder = CurlRequestBuilder.Request(method, GITHUB_API + path)
                .WithHeader("Accept", "application/vnd.github+json")
                .WithUserAgent("CurlDotNet-Example/1.0"); // GitHub rejects requests without one
            return string.IsNullOrEmpty(_token) ? builder : builder.WithBearerToken(_token);
        }

        public async Task RunAsync()
        {
            Console.WriteLine("=== GitHub API Integration Example ===\n");

            // Example 1: Get user information
            Console.WriteLine("Example 1: Get user profile");
            var userResult = await GitHub("GET", "/users/octocat").ExecuteAsync();
            if (userResult.IsSuccess)
            {
                var user = userResult.ParseJson<GitHubUser>();
                Console.WriteLine($"User: {user.name} (@{user.login})");
                Console.WriteLine($"Followers: {user.followers}, Following: {user.following}\n");
            }

            // Example 2: List repositories - query parameters go in the URL
            Console.WriteLine("Example 2: List user repositories");
            var reposResult = await GitHub("GET", "/users/jacob-mellor/repos?sort=updated&per_page=5").ExecuteAsync();
            if (reposResult.IsSuccess)
            {
                foreach (var repo in reposResult.ParseJson<List<GitHubRepo>>())
                {
                    Console.WriteLine($"- {repo.name}: {repo.stargazers_count} stars, {repo.forks_count} forks");
                }
                Console.WriteLine();
            }

            // Example 3: Create an issue (requires a token; replace owner/repo with your repository)
            if (!string.IsNullOrEmpty(_token))
            {
                Console.WriteLine("Example 3: Create an issue");
                var createResult = await GitHub("POST", "/repos/owner/repo/issues")
                    .WithJson(new { title = "Test issue from CurlDotNet", body = "Created with CurlDotNet" })
                    .ExecuteAsync();
                Console.WriteLine(createResult.StatusCode == 201
                    ? "Issue created successfully"
                    : $"Issue creation status: {createResult.StatusCode}");
            }

            // Example 4: Search repositories - escape query values yourself
            Console.WriteLine("\nExample 4: Search repositories");
            var q = Uri.EscapeDataString("curl language:csharp");
            var searchResult = await GitHub("GET", $"/search/repositories?q={q}&sort=stars&order=desc&per_page=3").ExecuteAsync();
            if (searchResult.IsSuccess)
            {
                var search = searchResult.ParseJson<GitHubSearchResult>();
                Console.WriteLine($"Found {search.total_count} repositories");
                foreach (var repo in search.items.Take(3))
                {
                    var description = repo.description ?? "";
                    Console.WriteLine($"- {repo.full_name}: {description.Substring(0, Math.Min(50, description.Length))}");
                }
            }

            // Example 5: Get rate limit status
            Console.WriteLine("\nExample 5: Check API rate limit");
            var rateLimitResult = await GitHub("GET", "/rate_limit").ExecuteAsync();
            if (rateLimitResult.IsSuccess)
            {
                var rate = rateLimitResult.ParseJson<RateLimitResponse>().rate;
                Console.WriteLine($"API calls remaining: {rate.remaining}/{rate.limit}");
                Console.WriteLine($"Reset time: {DateTimeOffset.FromUnixTimeSeconds(rate.reset).LocalDateTime}");
            }

            // Example 6: Download repository archive (GitHub redirects to the zip)
            Console.WriteLine("\nExample 6: Download repository archive");
            var archiveResult = await GitHub("GET", "/repos/jacob-mellor/curl-dot-net/zipball/master")
                .FollowRedirects()
                .ExecuteAsync();
            if (archiveResult.IsBinary)
            {
                archiveResult.SaveToFile("curl-dot-net-master.zip");
                Console.WriteLine($"Repository downloaded: curl-dot-net-master.zip ({archiveResult.BinaryData.Length:N0} bytes)");
            }
        }

        // Data models for JSON deserialization (property names match GitHub's JSON)
        class GitHubUser { public string login { get; set; } public string name { get; set; } public int followers { get; set; } public int following { get; set; } }

        class GitHubRepo
        {
            public string name { get; set; }
            public string full_name { get; set; }
            public string description { get; set; }
            public int stargazers_count { get; set; }
            public int forks_count { get; set; }
        }

        class GitHubSearchResult { public int total_count { get; set; } public List<GitHubRepo> items { get; set; } }

        class RateLimitResponse { public RateInfo rate { get; set; } }

        class RateInfo { public int limit { get; set; } public int remaining { get; set; } public long reset { get; set; } }

        public static async Task Main()
        {
            var example = new GitHubApiExample();
            await example.RunAsync();
        }
    }
}
