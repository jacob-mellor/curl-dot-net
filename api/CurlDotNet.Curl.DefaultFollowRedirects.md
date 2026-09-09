#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultFollowRedirects Property


<b>Controls whether curl automatically follows HTTP redirects (301, 302, etc).</b>

When true, acts like adding `-L` or `--location` to every command.
             Many APIs use redirects, so you often want this enabled.

<b>Example:</b>

```csharp
// Enable redirect following globally
Curl.DefaultFollowRedirects = true;

// Now shortened URLs work automatically
var response = await Curl.Execute("curl https://bit.ly/example");  // Follows to final destination

// Or use -L flag per command
var response = await Curl.Execute("curl -L https://bit.ly/example");
```

<b>Security note:</b> Be careful following redirects to untrusted sources.

<b>Learn more:</b>[curl \-L documentation](https://curl.se/docs/manpage.html#-L 'https://curl\.se/docs/manpage\.html\#\-L')

```csharp
public static bool DefaultFollowRedirects { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true to follow redirects, false to stop at first response\. Default is false\.