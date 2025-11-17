#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultInsecure Property


<b>⚠️ WARNING: Disables SSL certificate validation - ONLY use for development/testing!</b>

When true, acts like adding `-k` or `--insecure` to every command.
             This accepts any SSL certificate, even self-signed or expired ones.

<b>Example (DEVELOPMENT ONLY):</b>

```csharp
#if DEBUG
// Only in debug builds for local testing
Curl.DefaultInsecure = true;

// Now works with self-signed certificates
await Curl.Execute("curl https://localhost:5001");  // Works even with invalid cert
#endif
```

<b>🔴 NEVER use this in production!</b> It makes you vulnerable to man-in-the-middle attacks.

<b>Learn more:</b>[curl \-k documentation](https://curl.se/docs/manpage.html#-k 'https://curl\.se/docs/manpage\.html\#\-k')

```csharp
public static bool DefaultInsecure { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true to skip SSL validation \(DANGEROUS\), false to validate \(safe\)\. Default is false\.