using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

namespace CurlDotNet.Examples.RealWorld
{
    /// <summary>
    /// Example: Complete GitHub API integration
    /// </summary>
    public class GitHubApiExample
    {
        private const string GITHUB_API = "https://api.github.com";
        private readonly string _token;

        public GitHubApiExample(string token = null)
        {
            _token = token ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        }

        public async Task RunAsync()
        {
            Console.WriteLine("=== GitHub API Integration Example ===\n");

            // Example 1: Get user information
            Console.WriteLine("Example 1: Get user profile");
            var userResult = await Curl.GetAsync($"{GITHUB_API}/users/octocat")
                .WithHeader("Accept", "application/vnd.github.v3+json")
                .WithUserAgent("CurlDotNet-Example/1.0")
                .ExecuteAsync();

            if (userResult.IsSuccess)
            {
                var user = JsonSerializer.Deserialize<GitHubUser>(userResult.Body);
                Console.WriteLine($"User: {user.name} (@{user.login})");
                Console.WriteLine($"Bio: {user.bio}");
                Console.WriteLine($"Followers: {user.followers}, Following: {user.following}\n");
            }

            // Example 2: List repositories
            Console.WriteLine("Example 2: List user repositories");
            var reposResult = await Curl.GetAsync($"{GITHUB_API}/users/jacob-mellor/repos")
                .WithHeader("Accept", "application/vnd.github.v3+json")
                .WithQueryParam("sort", "updated")
                .WithQueryParam("per_page", "5")
                .ExecuteAsync();

            if (reposResult.IsSuccess)
            {
                var repos = JsonSerializer.Deserialize<List<GitHubRepo>>(reposResult.Body);
                foreach (var repo in repos.Take(5))
                {
                    Console.WriteLine($"- {repo.name}: ⭐ {repo.stargazers_count}, 🍴 {repo.forks_count}");
                }
                Console.WriteLine();
            }

            // Example 3: Create an issue (requires authentication)
            if (!string.IsNullOrEmpty(_token))
            {
                Console.WriteLine("Example 3: Create an issue");
                var issueData = new
                {
                    title = "Test issue from CurlDotNet",
                    body = "This issue was created using CurlDotNet library",
                    labels = new[] { "documentation", "example" }
                };

                var createResult = await Curl.PostAsync($"{GITHUB_API}/repos/owner/repo/issues")
                    .WithBearerToken(_token)
                    .WithHeader("Accept", "application/vnd.github.v3+json")
                    .WithJson(issueData)
                    .ExecuteAsync();

                if (createResult.StatusCode == 201)
                {
                    Console.WriteLine("✅ Issue created successfully!");
                }
                else
                {
                    Console.WriteLine($"Issue creation status: {createResult.StatusCode}");
                }
            }

            // Example 4: Search repositories
            Console.WriteLine("\nExample 4: Search repositories");
            var searchResult = await Curl.GetAsync($"{GITHUB_API}/search/repositories")
                .WithQueryParam("q", "curl language:csharp")
                .WithQueryParam("sort", "stars")
                .WithQueryParam("order", "desc")
                .WithHeader("Accept", "application/vnd.github.v3+json")
                .ExecuteAsync();

            if (searchResult.IsSuccess)
            {
                var searchResponse = JsonSerializer.Deserialize<GitHubSearchResult>(searchResult.Body);
                Console.WriteLine($"Found {searchResponse.total_count} repositories");
                foreach (var repo in searchResponse.items.Take(3))
                {
                    Console.WriteLine($"- {repo.full_name}: {repo.description?.Substring(0, Math.Min(50, repo.description.Length))}...");
                }
            }

            // Example 5: Get rate limit status
            Console.WriteLine("\nExample 5: Check API rate limit");
            var rateLimitResult = await Curl.GetAsync($"{GITHUB_API}/rate_limit")
                .WithHeader("Accept", "application/vnd.github.v3+json")
                .ExecuteAsync();

            if (rateLimitResult.IsSuccess)
            {
                dynamic rateLimit = JsonSerializer.Deserialize<dynamic>(rateLimitResult.Body);
                Console.WriteLine($"API calls remaining: {rateLimit.GetProperty("rate").GetProperty("remaining")}");
                Console.WriteLine($"Reset time: {DateTimeOffset.FromUnixTimeSeconds(rateLimit.GetProperty("rate").GetProperty("reset").GetInt64()).LocalDateTime}");
            }

            // Example 6: Download repository archive
            Console.WriteLine("\nExample 6: Download repository archive");
            var archiveResult = await Curl.GetAsync($"{GITHUB_API}/repos/jacob-mellor/curl-dot-net/zipball/master")
                .WithHeader("Accept", "application/vnd.github.v3+json")
                .FollowRedirects()
                .ExecuteAsync();

            if (archiveResult.BinaryData != null)
            {
                var fileName = "curl-dot-net-master.zip";
                await System.IO.File.WriteAllBytesAsync(fileName, archiveResult.BinaryData);
                Console.WriteLine($"Repository downloaded: {fileName} ({archiveResult.BinaryData.Length:N0} bytes)");
            }
        }

        // Data models for JSON deserialization
        class GitHubUser
        {
            public string login { get; set; }
            public string name { get; set; }
            public string bio { get; set; }
            public int followers { get; set; }
            public int following { get; set; }
            public int public_repos { get; set; }
        }

        class GitHubRepo
        {
            public string name { get; set; }
            public string full_name { get; set; }
            public string description { get; set; }
            public int stargazers_count { get; set; }
            public int forks_count { get; set; }
            public string language { get; set; }
        }

        class GitHubSearchResult
        {
            public int total_count { get; set; }
            public List<GitHubRepo> items { get; set; }
        }

        public static async Task Main()
        {
            var example = new GitHubApiExample();
            await example.RunAsync();
        }
    }
}