#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultMaxTimeSeconds Property


<b>Sets a global timeout for all curl operations (in seconds).</b>

This is like adding `--max-time` to every curl command automatically.
             Set to 0 (default) for no timeout. Individual commands can still override this.

<b>Example:</b>

```csharp
// Set 30 second timeout for all operations
Curl.DefaultMaxTimeSeconds = 30;

// Now all commands timeout after 30 seconds
await Curl.Execute("curl https://slow-api.example.com");  // Times out after 30s

// Override for specific command
await Curl.Execute("curl --max-time 60 https://very-slow-api.example.com");  // 60s timeout
```

<b>Learn more:</b>[curl \-\-max\-time documentation](https://curl.se/docs/manpage.html#-m 'https://curl\.se/docs/manpage\.html\#\-m')

```csharp
public static int DefaultMaxTimeSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Timeout in seconds\. 0 = no timeout \(wait forever\)\. Default is 0\.