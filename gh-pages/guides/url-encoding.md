# URL Encoding Guide

## Overview

URL encoding (also called percent encoding) converts special characters into a format that can be safely transmitted over the internet. This guide covers everything you need to know about URL encoding when using CurlDotNet.

## Why URL Encoding Matters

URLs can only contain certain characters. Special characters must be encoded to avoid breaking the URL or causing security issues.

### Characters That Need Encoding

| Character | Encoded | Why | Example |
|-----------|---------|-----|---------|
| Space | `%20` or `+` | Separator | `Hello World` → `Hello%20World` |
| `?` | `%3F` | Query string delimiter | `what?` → `what%3F` |
| `#` | `%23` | Fragment identifier | `tag#1` → `tag%231` |
| `&` | `%26` | Parameter separator | `Tom&Jerry` → `Tom%26Jerry` |
| `=` | `%3D` | Key-value separator | `a=b` → `a%3Db` (in value) |
| `%` | `%25` | Encoding prefix | `100%` → `100%25` |
| `/` | `%2F` | Path separator | `a/b` → `a%2Fb` (in param) |
| `+` | `%2B` | Plus sign | `1+1` → `1%2B1` |

## Understanding URL Components

A URL has different parts, each with different encoding rules:

```
https://api.example.com:443/search/items?q=hello+world&category=books#results
└─┬─┘   └──────┬──────┘ └┬┘ └─────┬──────┘ └──────────┬───────────┘ └──┬──┘
scheme      authority    port    path              query string      fragment
```

### Component-Specific Encoding

```csharp
using System;
using System.Web;
using CurlDotNet;

// Path: Encode spaces and special chars
var path = "/search/hello world";
var encodedPath = Uri.EscapeDataString("hello world");
// Result: /search/hello%20world

// Query parameters: Most special chars encoded
var query = "search query with spaces & symbols";
var encodedQuery = Uri.EscapeDataString(query);
// Result: search%20query%20with%20spaces%20%26%20symbols

// Fragment: Similar to query
var fragment = "section #1";
var encodedFragment = Uri.EscapeDataString("section #1");
// Result: section%20%231
```

## Encoding in C#

### Method 1: Uri.EscapeDataString (Recommended)

```csharp
using System;
using CurlDotNet;

// Best for query parameters
var searchTerm = "C# programming";
var encoded = Uri.EscapeDataString(searchTerm);
Console.WriteLine(encoded); // C%23%20programming

var result = await Curl.ExecuteAsync(
    $"curl https://api.example.com/search?q={encoded}"
);
```

### Method 2: HttpUtility.UrlEncode

```csharp
using System.Web;
using CurlDotNet;

// Alternative method (requires System.Web)
var searchTerm = "hello world";
var encoded = HttpUtility.UrlEncode(searchTerm);
Console.WriteLine(encoded); // hello+world (uses + for spaces)

var result = await Curl.ExecuteAsync(
    $"curl https://api.example.com/search?q={encoded}"
);
```

### Method 3: WebUtility.UrlEncode

```csharp
using System.Net;
using CurlDotNet;

// Available without System.Web reference
var searchTerm = "Tom & Jerry";
var encoded = WebUtility.UrlEncode(searchTerm);
Console.WriteLine(encoded); // Tom+%26+Jerry

var result = await Curl.ExecuteAsync(
    $"curl https://api.example.com/search?q={encoded}"
);
```

## Common Scenarios

### Scenario 1: Search Query

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class SearchExample
{
    public static async Task<CurlResult> SearchAsync(string query)
    {
        // User input: "C# .NET framework"
        var encoded = Uri.EscapeDataString(query);
        // Result: C%23%20.NET%20framework

        var url = $"https://api.github.com/search/repositories?q={encoded}";
        return await Curl.ExecuteAsync($"curl {url}");
    }
}

// Usage
var result = await SearchExample.SearchAsync("C# .NET framework");
```

### Scenario 2: Multiple Query Parameters

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurlDotNet;

public class QueryBuilder
{
    public static string BuildQueryString(Dictionary<string, string> parameters)
    {
        var pairs = parameters.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"
        );
        return string.Join("&", pairs);
    }

    public static async Task<CurlResult> SearchWithFiltersAsync(
        string query,
        Dictionary<string, string> filters)
    {
        var baseUrl = "https://api.example.com/search";

        var allParams = new Dictionary<string, string>(filters)
        {
            ["q"] = query
        };

        var queryString = BuildQueryString(allParams);
        var url = $"{baseUrl}?{queryString}";

        return await Curl.ExecuteAsync($"curl {url}");
    }
}

// Usage
var filters = new Dictionary<string, string>
{
    ["category"] = "books & magazines",
    ["price"] = "< $50",
    ["condition"] = "new"
};

var result = await QueryBuilder.SearchWithFiltersAsync("C# programming", filters);
// URL: https://api.example.com/search?q=C%23%20programming&category=books%20%26%20magazines&price=%3C%20%2450&condition=new
```

### Scenario 3: Email Addresses

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class EmailExample
{
    public static async Task<CurlResult> LookupUserByEmailAsync(string email)
    {
        // Email: user+tag@example.com
        var encoded = Uri.EscapeDataString(email);
        // Result: user%2Btag%40example.com

        var url = $"https://api.example.com/users?email={encoded}";
        return await Curl.ExecuteAsync($"curl {url}");
    }
}

// Usage
var result = await EmailExample.LookupUserByEmailAsync("john+test@example.com");
```

### Scenario 4: File Paths

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class FilePathExample
{
    public static async Task<CurlResult> GetFileAsync(string filePath)
    {
        // Path: documents/my resume.pdf
        var encoded = Uri.EscapeDataString(filePath);
        // Result: documents%2Fmy%20resume.pdf

        var url = $"https://api.example.com/files/{encoded}";
        return await Curl.ExecuteAsync($"curl {url}");
    }
}

// Usage
var result = await FilePathExample.GetFileAsync("documents/my resume.pdf");
```

### Scenario 5: JSON in Query Parameters

```csharp
using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

public class JsonQueryExample
{
    public static async Task<CurlResult> FilterWithJsonAsync(object filter)
    {
        // Serialize to JSON
        var json = JsonSerializer.Serialize(filter);
        // Result: {"name":"John","age":30}

        // Encode for URL
        var encoded = Uri.EscapeDataString(json);
        // Result: %7B%22name%22%3A%22John%22%2C%22age%22%3A30%7D

        var url = $"https://api.example.com/users?filter={encoded}";
        return await Curl.ExecuteAsync($"curl {url}");
    }
}

// Usage
var filter = new { name = "John", age = 30 };
var result = await JsonQueryExample.FilterWithJsonAsync(filter);
```

## URL Builder Helper Class

Here's a comprehensive helper class for building URLs safely:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UrlBuilder
{
    private readonly string _baseUrl;
    private readonly Dictionary<string, string> _queryParams = new();
    private string _fragment;

    public UrlBuilder(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public UrlBuilder AddParameter(string key, string value)
    {
        if (string.IsNullOrEmpty(value))
            return this;

        _queryParams[key] = value;
        return this;
    }

    public UrlBuilder AddParameters(Dictionary<string, string> parameters)
    {
        foreach (var kvp in parameters)
        {
            AddParameter(kvp.Key, kvp.Value);
        }
        return this;
    }

    public UrlBuilder SetFragment(string fragment)
    {
        _fragment = fragment;
        return this;
    }

    public string Build()
    {
        var url = new StringBuilder(_baseUrl);

        if (_queryParams.Any())
        {
            url.Append('?');
            var pairs = _queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"
            );
            url.Append(string.Join("&", pairs));
        }

        if (!string.IsNullOrEmpty(_fragment))
        {
            url.Append('#');
            url.Append(Uri.EscapeDataString(_fragment));
        }

        return url.ToString();
    }

    public override string ToString() => Build();
}

// Usage examples
public class UrlBuilderExamples
{
    public static void Example1()
    {
        var url = new UrlBuilder("https://api.example.com/search")
            .AddParameter("q", "C# programming")
            .AddParameter("category", "books & magazines")
            .AddParameter("price", "< $50")
            .Build();

        Console.WriteLine(url);
        // https://api.example.com/search?q=C%23%20programming&category=books%20%26%20magazines&price=%3C%20%2450
    }

    public static void Example2()
    {
        var filters = new Dictionary<string, string>
        {
            ["user"] = "john@example.com",
            ["status"] = "active",
            ["role"] = "admin/user"
        };

        var url = new UrlBuilder("https://api.example.com/users")
            .AddParameters(filters)
            .SetFragment("results")
            .Build();

        Console.WriteLine(url);
        // https://api.example.com/users?user=john%40example.com&status=active&role=admin%2Fuser#results
    }

    public static async Task Example3()
    {
        var builder = new UrlBuilder("https://api.github.com/search/repositories")
            .AddParameter("q", "language:C# stars:>1000")
            .AddParameter("sort", "stars")
            .AddParameter("order", "desc");

        var result = await Curl.ExecuteAsync($"curl {builder}");
        Console.WriteLine($"Status: {result.StatusCode}");
    }
}
```

## Common Pitfalls

### Pitfall 1: Double Encoding

```csharp
// WRONG: Double encoding
var query = "hello world";
var encoded1 = Uri.EscapeDataString(query);  // hello%20world
var encoded2 = Uri.EscapeDataString(encoded1); // hello%2520world (WRONG!)

// RIGHT: Encode once
var query = "hello world";
var encoded = Uri.EscapeDataString(query);  // hello%20world
var url = $"https://api.example.com/search?q={encoded}";
```

### Pitfall 2: Not Encoding User Input

```csharp
// WRONG: Using user input directly
var userInput = "C# & .NET";
var url = $"https://api.example.com/search?q={userInput}"; // BROKEN URL!

// RIGHT: Always encode user input
var userInput = "C# & .NET";
var encoded = Uri.EscapeDataString(userInput);
var url = $"https://api.example.com/search?q={encoded}";
```

### Pitfall 3: Encoding the Entire URL

```csharp
// WRONG: Encoding the entire URL
var url = "https://api.example.com/search?q=hello world";
var encoded = Uri.EscapeDataString(url); // Breaks the URL structure!

// RIGHT: Encode only the parameter values
var baseUrl = "https://api.example.com/search";
var query = Uri.EscapeDataString("hello world");
var url = $"{baseUrl}?q={query}";
```

### Pitfall 4: Forgetting Special Characters

```csharp
// These characters need encoding in query values:
var testCases = new Dictionary<string, string>
{
    ["Space"] = "hello world",           // hello%20world
    ["Ampersand"] = "Tom & Jerry",       // Tom%20%26%20Jerry
    ["Plus"] = "1+1=2",                  // 1%2B1%3D2
    ["Equals"] = "a=b",                  // a%3Db
    ["Hash"] = "tag#1",                  // tag%231
    ["Percent"] = "100%",                // 100%25
    ["Question"] = "why?",               // why%3F
    ["Slash"] = "path/to/file",          // path%2Fto%2Ffile
    ["At"] = "user@domain.com",          // user%40domain.com
    ["Bracket"] = "array[0]",            // array%5B0%5D
};

foreach (var kvp in testCases)
{
    var encoded = Uri.EscapeDataString(kvp.Value);
    Console.WriteLine($"{kvp.Key}: {kvp.Value} → {encoded}");
}
```

## Decoding URLs

### Decoding in C#

```csharp
using System;
using System.Net;
using System.Web;

// Method 1: Uri.UnescapeDataString
var encoded = "C%23%20programming";
var decoded = Uri.UnescapeDataString(encoded);
Console.WriteLine(decoded); // C# programming

// Method 2: HttpUtility.UrlDecode
var encoded2 = "hello+world";
var decoded2 = HttpUtility.UrlDecode(encoded2);
Console.WriteLine(decoded2); // hello world

// Method 3: WebUtility.UrlDecode
var encoded3 = "Tom+%26+Jerry";
var decoded3 = WebUtility.UrlDecode(encoded3);
Console.WriteLine(decoded3); // Tom & Jerry
```

### Extracting Query Parameters

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class UrlParser
{
    public static Dictionary<string, string> ParseQueryString(string url)
    {
        var uri = new Uri(url);
        var query = uri.Query.TrimStart('?');

        return query
            .Split('&')
            .Where(part => part.Contains('='))
            .Select(part => part.Split('=', 2))
            .ToDictionary(
                parts => Uri.UnescapeDataString(parts[0]),
                parts => Uri.UnescapeDataString(parts[1])
            );
    }
}

// Usage
var url = "https://api.example.com/search?q=C%23%20programming&category=books%20%26%20magazines";
var params = UrlParser.ParseQueryString(url);

foreach (var kvp in params)
{
    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
}
// Output:
// q = C# programming
// category = books & magazines
```

## Special Cases

### International Characters (UTF-8)

```csharp
using System;
using System.Text;
using CurlDotNet;

// Encoding international characters
var query = "日本語"; // Japanese
var encoded = Uri.EscapeDataString(query);
Console.WriteLine(encoded); // %E6%97%A5%E6%9C%AC%E8%AA%9E

var url = $"https://api.example.com/search?q={encoded}";
var result = await Curl.ExecuteAsync($"curl {url}");
```

### Emoji in URLs

```csharp
using System;
using CurlDotNet;

// Emoji encoding
var query = "😀 happy";
var encoded = Uri.EscapeDataString(query);
Console.WriteLine(encoded); // %F0%9F%98%80%20happy

var url = $"https://api.example.com/search?q={encoded}";
var result = await Curl.ExecuteAsync($"curl {url}");
```

### Reserved Characters in Different Contexts

```csharp
// Slash in path vs query parameter
var pathSegment = "docs/guide"; // Don't encode slash in path
var url1 = $"https://api.example.com/{pathSegment}";
// Result: https://api.example.com/docs/guide

var queryValue = "docs/guide"; // DO encode slash in query
var encoded = Uri.EscapeDataString(queryValue);
var url2 = $"https://api.example.com/files?path={encoded}";
// Result: https://api.example.com/files?path=docs%2Fguide
```

## Testing URL Encoding

### Unit Test Examples

```csharp
using System;
using System.Collections.Generic;

public class UrlEncodingTests
{
    public static void TestBasicEncoding()
    {
        var testCases = new Dictionary<string, string>
        {
            ["hello world"] = "hello%20world",
            ["C# programming"] = "C%23%20programming",
            ["Tom & Jerry"] = "Tom%20%26%20Jerry",
            ["100%"] = "100%25",
            ["user@example.com"] = "user%40example.com",
        };

        foreach (var test in testCases)
        {
            var encoded = Uri.EscapeDataString(test.Key);
            var passed = encoded == test.Value;
            Console.WriteLine($"{(passed ? "✓" : "✗")} {test.Key} → {encoded}");
        }
    }

    public static void TestRoundTrip()
    {
        var testStrings = new[]
        {
            "hello world",
            "C# & .NET",
            "path/to/file",
            "key=value",
            "100% complete",
            "user+test@example.com"
        };

        foreach (var original in testStrings)
        {
            var encoded = Uri.EscapeDataString(original);
            var decoded = Uri.UnescapeDataString(encoded);
            var passed = original == decoded;
            Console.WriteLine($"{(passed ? "✓" : "✗")} Round trip: {original}");
        }
    }
}

// Run tests
UrlEncodingTests.TestBasicEncoding();
UrlEncodingTests.TestRoundTrip();
```

## Best Practices

### 1. Always Encode User Input

```csharp
// ALWAYS encode user input before using in URLs
public async Task<CurlResult> SafeSearchAsync(string userQuery)
{
    var encoded = Uri.EscapeDataString(userQuery);
    return await Curl.ExecuteAsync($"curl https://api.example.com/search?q={encoded}");
}
```

### 2. Use a URL Builder

```csharp
// Use a builder to handle encoding automatically
var url = new UrlBuilder("https://api.example.com/search")
    .AddParameter("q", userInput)  // Automatically encoded
    .Build();
```

### 3. Validate After Encoding

```csharp
public string BuildSafeUrl(string baseUrl, Dictionary<string, string> parameters)
{
    var builder = new UrlBuilder(baseUrl);

    foreach (var param in parameters)
    {
        builder.AddParameter(param.Key, param.Value);
    }

    var url = builder.Build();

    // Validate the URL is well-formed
    if (!Uri.TryCreate(url, UriKind.Absolute, out var validatedUri))
    {
        throw new ArgumentException($"Invalid URL generated: {url}");
    }

    return url;
}
```

### 4. Log Encoded URLs

```csharp
public async Task<CurlResult> ExecuteWithLoggingAsync(string baseUrl, string query)
{
    var encoded = Uri.EscapeDataString(query);
    var url = $"{baseUrl}?q={encoded}";

    Console.WriteLine($"Original query: {query}");
    Console.WriteLine($"Encoded URL: {url}");

    return await Curl.ExecuteAsync($"curl {url}");
}
```

## Summary

Key takeaways for URL encoding:

1. **Always encode user input** before using in URLs
2. **Encode parameter values**, not the entire URL
3. **Use `Uri.EscapeDataString`** for query parameters
4. **Don't double-encode** - encode once at the right time
5. **Test with special characters** including spaces, &, =, #, %, +
6. **Use a URL builder** for complex URLs
7. **Validate** the final URL is well-formed

## Related Resources

- [Headers Explained Tutorial](../tutorials/08-headers-explained.md)
- [Forms and Data Tutorial](../tutorials/11-forms-and-data.md)
- [Common Issues](../troubleshooting/common-issues.md)
- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)

---

**Have questions about URL encoding?** Ask in our [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions).
