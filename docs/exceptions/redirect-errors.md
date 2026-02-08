# Redirect Handling Errors

## Overview

Redirect errors occur when CurlDotNet encounters too many HTTP redirects or gets stuck in a redirect loop. By default, curl follows up to 50 redirects, but this can be configured.

## Common Error Scenarios

### 1. Redirect Loops

**Problem:** The server redirects in a circular pattern (A → B → C → A).

**Common Causes:**
- Misconfigured web server
- Incorrect rewrite rules
- Authentication redirect loops
- Protocol switching loops (HTTP ↔ HTTPS)

**Solutions:**

```csharp
try
{
    var result = await curl.ExecuteAsync("curl -L https://example.com/page");
}
catch (CurlRedirectException ex)
{
    Console.WriteLine($"Redirect loop detected after {ex.RedirectCount} redirects");
    Console.WriteLine($"Last URL: {ex.LastUrl}");

    // Try with limited redirects
    result = await curl.ExecuteAsync("curl --max-redirs 5 https://example.com/page");

    // Or don't follow redirects at all
    result = await curl.ExecuteAsync("curl https://example.com/page"); // No -L flag
}
```

### 2. Excessive Redirect Chains

**Problem:** Server uses long chains of redirects (more than necessary).

**Common Causes:**
- URL shortener chains
- Tracking redirects
- Load balancer redirects
- CDN redirects

**Solutions:**

```csharp
// Limit redirect count
var maxRedirects = 10;
var result = await curl.ExecuteAsync($"curl --max-redirs {maxRedirects} -L {url}");

// Or handle redirects manually
var response = await curl.ExecuteAsync("curl -I " + url); // Get headers only
if (response.Headers.ContainsKey("Location"))
{
    var newUrl = response.Headers["Location"];
    // Manually follow with control
    result = await curl.ExecuteAsync($"curl {newUrl}");
}
```

### 3. Cross-Domain Redirect Issues

**Problem:** Redirects across different domains cause security or cookie issues.

**Common Causes:**
- OAuth flows
- Payment gateways
- SSO systems
- CDN redirects

**Solutions:**

```csharp
// Follow redirects but be aware of domain changes
public async Task<CurlResult> FollowRedirectsCarefully(string url)
{
    var curl = new Curl();
    var visitedUrls = new HashSet<string>();
    var currentUrl = url;
    var redirectCount = 0;
    var maxRedirects = 10;

    while (redirectCount < maxRedirects)
    {
        if (visitedUrls.Contains(currentUrl))
        {
            throw new Exception($"Redirect loop detected at: {currentUrl}");
        }
        visitedUrls.Add(currentUrl);

        // Get headers only
        var result = await curl.ExecuteAsync($"curl -I {currentUrl}");

        // Check for redirect
        if (result.StatusCode >= 300 && result.StatusCode < 400)
        {
            var location = result.Headers["Location"];
            Console.WriteLine($"Redirecting from {currentUrl} to {location}");
            currentUrl = location;
            redirectCount++;
        }
        else
        {
            // Final request
            return await curl.ExecuteAsync($"curl {currentUrl}");
        }
    }

    throw new CurlRedirectException($"Too many redirects", redirectCount, currentUrl);
}
```

## Best Practices

### 1. Smart Redirect Handling

```csharp
public class SmartRedirectHandler
{
    private readonly int _maxRedirects;
    private readonly bool _followHttps;

    public SmartRedirectHandler(int maxRedirects = 10, bool followHttpsOnly = true)
    {
        _maxRedirects = maxRedirects;
        _followHttps = followHttpsOnly;
    }

    public async Task<CurlResult> Execute(string url)
    {
        var curl = new Curl();

        // First, check without following redirects
        var checkResult = await curl.ExecuteAsync($"curl -I {url}");

        if (checkResult.StatusCode >= 300 && checkResult.StatusCode < 400)
        {
            var location = checkResult.Headers["Location"];

            // Security check: only follow HTTPS redirects if configured
            if (_followHttps && !location.StartsWith("https://"))
            {
                throw new SecurityException($"Refusing to follow non-HTTPS redirect: {location}");
            }

            // Follow with limit
            return await curl.ExecuteAsync($"curl --max-redirs {_maxRedirects} -L {url}");
        }

        return await curl.ExecuteAsync($"curl {url}");
    }
}
```

### 2. Redirect Chain Analysis

```csharp
public class RedirectChainAnalyzer
{
    public class RedirectInfo
    {
        public string From { get; set; }
        public string To { get; set; }
        public int StatusCode { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public async Task<List<RedirectInfo>> AnalyzeRedirectChain(string url)
    {
        var chain = new List<RedirectInfo>();
        var curl = new Curl();
        var currentUrl = url;
        var maxRedirects = 20;

        for (int i = 0; i < maxRedirects; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await curl.ExecuteAsync($"curl -I {currentUrl}");
            stopwatch.Stop();

            if (result.StatusCode >= 300 && result.StatusCode < 400)
            {
                var location = result.Headers["Location"];
                chain.Add(new RedirectInfo
                {
                    From = currentUrl,
                    To = location,
                    StatusCode = result.StatusCode,
                    Duration = stopwatch.Elapsed
                });
                currentUrl = location;
            }
            else
            {
                break; // No more redirects
            }
        }

        return chain;
    }

    public void PrintChain(List<RedirectInfo> chain)
    {
        Console.WriteLine("Redirect Chain Analysis:");
        Console.WriteLine("========================");
        foreach (var redirect in chain)
        {
            Console.WriteLine($"{redirect.StatusCode}: {redirect.From}");
            Console.WriteLine($"  → {redirect.To}");
            Console.WriteLine($"  Time: {redirect.Duration.TotalMilliseconds}ms");
        }
        Console.WriteLine($"Total redirects: {chain.Count}");
    }
}
```

## Debugging Tips

### 1. Trace Redirect Path

```csharp
// Use verbose mode to see all redirects
var result = await curl.ExecuteAsync("curl -v -L --max-redirs 10 " + url);
// Parse stderr for redirect information
```

### 2. Detect Redirect Type

```csharp
public static string GetRedirectType(int statusCode)
{
    return statusCode switch
    {
        301 => "Moved Permanently",
        302 => "Found (Temporary)",
        303 => "See Other",
        304 => "Not Modified",
        307 => "Temporary Redirect",
        308 => "Permanent Redirect",
        _ => "Unknown"
    };
}
```

### 3. Common Redirect Patterns

```csharp
public class RedirectPatternDetector
{
    public enum RedirectPattern
    {
        None,
        HttpToHttps,
        WwwRedirect,
        TrailingSlash,
        AuthenticationFlow,
        Loop,
        Chain
    }

    public RedirectPattern DetectPattern(List<string> urlChain)
    {
        if (urlChain.Count < 2) return RedirectPattern.None;

        // Check for loops
        if (urlChain.Distinct().Count() != urlChain.Count)
            return RedirectPattern.Loop;

        // Check for HTTP to HTTPS
        if (urlChain[0].StartsWith("http://") && urlChain[1].StartsWith("https://"))
            return RedirectPattern.HttpToHttps;

        // Check for www redirect
        var first = new Uri(urlChain[0]);
        var second = new Uri(urlChain[1]);
        if (first.Host.StartsWith("www.") != second.Host.StartsWith("www."))
            return RedirectPattern.WwwRedirect;

        // Check for trailing slash
        if (first.AbsolutePath + "/" == second.AbsolutePath)
            return RedirectPattern.TrailingSlash;

        return RedirectPattern.Chain;
    }
}
```

## Configuration Options

### Curl Redirect Options

```csharp
// Different redirect handling strategies
public class RedirectOptions
{
    // Follow all redirects (default limit: 50)
    public async Task<CurlResult> FollowAll(string url)
        => await curl.ExecuteAsync($"curl -L {url}");

    // Follow with custom limit
    public async Task<CurlResult> FollowLimited(string url, int limit)
        => await curl.ExecuteAsync($"curl -L --max-redirs {limit} {url}");

    // Don't follow redirects
    public async Task<CurlResult> NoFollow(string url)
        => await curl.ExecuteAsync($"curl {url}");

    // Follow and preserve method (307/308 compliance)
    public async Task<CurlResult> FollowPreserveMethod(string url)
        => await curl.ExecuteAsync($"curl -L --post301 --post302 --post303 {url}");
}
```

## Related Documentation

- [HTTP Error Handling](http-errors.html)
- [DNS Errors](dns-errors.html)
- [Timeout Errors](timeout-errors.html)
- [CurlDotNet Redirect API Reference](/api/CurlDotNet.Exceptions.CurlRedirectException.html)

## Error Prevention Checklist

- [ ] Set reasonable redirect limits (5-10 for most cases)
- [ ] Implement loop detection for manual redirect following
- [ ] Log redirect chains for debugging
- [ ] Consider security implications of cross-domain redirects
- [ ] Handle protocol changes (HTTP to HTTPS) appropriately
- [ ] Test with various redirect scenarios
- [ ] Monitor redirect performance impact
- [ ] Implement timeout for redirect chains
- [ ] Validate final destination URLs
- [ ] Consider caching redirect destinations