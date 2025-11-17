#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib')

## LibCurl Class


<b>🔧 LibCurl - Object-Oriented API for Programmatic HTTP</b>

Provides a libcurl-style object-oriented API for .NET developers who prefer
             programmatic control over curl command strings. Perfect for building HTTP clients
             where you need fine-grained control and reusable configurations.

<b>When to use LibCurl vs Curl String vs CurlRequestBuilder:</b>
- <b>LibCurl</b> - When you need persistent configuration, reusable instances, or object-oriented patterns
- <b>CurlRequestBuilder</b> - When you want fluent method chaining with one-off requests
- <b>Curl String</b> - When you have curl commands from docs/examples (paste and go!)

<b>Quick Example:</b>

```csharp
using (var curl = new LibCurl())
{
    // Configure defaults for all requests
    curl.WithHeader("Accept", "application/json")
        .WithBearerToken("your-token")
        .WithTimeout(TimeSpan.FromSeconds(30));

    // Make multiple requests with same configuration
    var user = await curl.GetAsync("https://api.example.com/user");
    var posts = await curl.GetAsync("https://api.example.com/posts");
}
```

```csharp
public class LibCurl : System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; LibCurl

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

### Remarks

LibCurl provides a stateful, reusable client with default configurations
             that persist across requests. This is ideal for scenarios where you make multiple
             requests to the same API with the same authentication and headers.

All methods are thread-safe and can be used concurrently.

Implements IDisposable - always use with `using` statement or dispose manually.

| Constructors | |
| :--- | :--- |
| [LibCurl\(\)](CurlDotNet.Lib.LibCurl.LibCurl().md 'CurlDotNet\.Lib\.LibCurl\.LibCurl\(\)') | Creates a new LibCurl instance\. |

| Methods | |
| :--- | :--- |
| [Configure\(Action&lt;CurlOptions&gt;\)](CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_).md 'CurlDotNet\.Lib\.LibCurl\.Configure\(System\.Action\<CurlDotNet\.Core\.CurlOptions\>\)') | Configure default options using an action\. |
| [DeleteAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.DeleteAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Perform a DELETE request\. |
| [Dispose\(\)](CurlDotNet.Lib.LibCurl.Dispose().md 'CurlDotNet\.Lib\.LibCurl\.Dispose\(\)') | Dispose resources\. |
| [GetAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.GetAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Perform a GET request\. |
| [HeadAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.HeadAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Perform a HEAD request\. |
| [PatchAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PatchAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Perform a PATCH request\. |
| [PerformAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PerformAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)') | Perform a custom request with full control\. |
| [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Perform a POST request\. |
| [PutAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PutAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Perform a PUT request\. |
| [WithBasicAuth\(string, string\)](CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithBasicAuth\(string, string\)') | Set basic authentication for all requests\. |
| [WithBearerToken\(string\)](CurlDotNet.Lib.LibCurl.WithBearerToken(string).md 'CurlDotNet\.Lib\.LibCurl\.WithBearerToken\(string\)') | Set bearer token authentication for all requests\. |
| [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithConnectTimeout\(System\.TimeSpan\)') | Set connection timeout for all requests\. |
| [WithFollowRedirects\(int\)](CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).md 'CurlDotNet\.Lib\.LibCurl\.WithFollowRedirects\(int\)') | Enable following redirects for all requests\. |
| [WithHeader\(string, string\)](CurlDotNet.Lib.LibCurl.WithHeader(string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithHeader\(string, string\)') | Set a default header for all requests\. |
| [WithInsecureSsl\(\)](CurlDotNet.Lib.LibCurl.WithInsecureSsl().md 'CurlDotNet\.Lib\.LibCurl\.WithInsecureSsl\(\)') | Enable ignoring SSL certificate errors \(for testing only\)\. |
| [WithOutputFile\(string\)](CurlDotNet.Lib.LibCurl.WithOutputFile(string).md 'CurlDotNet\.Lib\.LibCurl\.WithOutputFile\(string\)') | Set default output file for all requests\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithProxy\(string, string, string\)') | Set proxy for all requests\. |
| [WithTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithTimeout\(System\.TimeSpan\)') | Set timeout for all requests\. |
| [WithUserAgent\(string\)](CurlDotNet.Lib.LibCurl.WithUserAgent(string).md 'CurlDotNet\.Lib\.LibCurl\.WithUserAgent\(string\)') | Set user agent for all requests\. |
| [WithVerbose\(\)](CurlDotNet.Lib.LibCurl.WithVerbose().md 'CurlDotNet\.Lib\.LibCurl\.WithVerbose\(\)') | Enable verbose output for debugging\. |
