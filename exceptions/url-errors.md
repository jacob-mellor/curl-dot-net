# URL Syntax Errors

## Overview

URL errors occur when the provided URL is malformed or contains invalid characters.

## Common URL Issues

### 1. Missing Protocol
```csharp
// Wrong
"example.com"

// Correct
"https://example.com"
```

### 2. Invalid Characters
```csharp
// Spaces need encoding
var url = "https://example.com/my file.pdf";
var encoded = Uri.EscapeUriString(url);
// Results in: https://example.com/my%20file.pdf
```

### 3. Invalid Port
```csharp
// Port must be numeric
"https://example.com:abc" // Wrong
"https://example.com:8080" // Correct
```

## URL Validation

```csharp
public bool IsValidUrl(string url)
{
    if (string.IsNullOrWhiteSpace(url))
        return false;

    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        return false;

    return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
}
```

## URL Building

```csharp
public string BuildUrl(string host, string path, Dictionary<string, string> queryParams)
{
    var uriBuilder = new UriBuilder
    {
        Scheme = "https",
        Host = host,
        Path = path
    };

    if (queryParams?.Any() == true)
    {
        var query = string.Join("&",
            queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"
            )
        );
        uriBuilder.Query = query;
    }

    return uriBuilder.ToString();
}
```

## Related Documentation
- [URL Encoding Guide](/guides/url-encoding.html)
- [HTTP Protocol Reference](https://tools.ietf.org/html/rfc3986)