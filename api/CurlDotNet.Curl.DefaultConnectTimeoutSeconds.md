#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultConnectTimeoutSeconds Property


<b>Sets how long to wait for a connection to be established (in seconds).</b>

This is different from the total timeout - it only applies to making the initial connection.
             Like adding `--connect-timeout` to every curl command.

<b>Example:</b>

```csharp
// Give servers 10 seconds to accept connection
Curl.DefaultConnectTimeoutSeconds = 10;

// If server doesn't respond in 10 seconds, fails fast
await Curl.Execute("curl https://overloaded-server.example.com");
```

<b>Tip:</b> Set this lower than DefaultMaxTimeSeconds to fail fast on dead servers.

<b>Learn more:</b>[curl \-\-connect\-timeout documentation](https://curl.se/docs/manpage.html#--connect-timeout 'https://curl\.se/docs/manpage\.html\#\-\-connect\-timeout')

```csharp
public static int DefaultConnectTimeoutSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Connection timeout in seconds\. 0 = no timeout\. Default is 0\.