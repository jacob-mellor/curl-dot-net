#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlSettings Class

Fluent builder for \.NET\-specific curl settings\.

```csharp
public class CurlSettings
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlSettings

### Example

```csharp
var settings = new CurlSettings()
    .WithTimeout(30)
    .WithRetries(3)
    .WithProgress((percent, total, current) => Console.WriteLine($"{percent}%"))
    .WithCancellation(cancellationToken);

var result = await Curl.Execute("curl https://example.com", settings);
```

### Remarks

These settings complement curl commands with .NET-specific features.

AI-Usage: Use this for cancellation, progress, retries, and other .NET features.

| Properties | |
| :--- | :--- |
| [AutomaticDecompression](CurlDotNet.Core.CurlSettings.AutomaticDecompression.md 'CurlDotNet\.Core\.CurlSettings\.AutomaticDecompression') | Whether to automatically decompress response\. |
| [BufferSize](CurlDotNet.Core.CurlSettings.BufferSize.md 'CurlDotNet\.Core\.CurlSettings\.BufferSize') | Buffer size for download operations\. |
| [CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken') | Cancellation token for the operation\. |
| [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds') | Connection timeout in seconds\. |
| [Cookies](CurlDotNet.Core.CurlSettings.Cookies.md 'CurlDotNet\.Core\.CurlSettings\.Cookies') | Cookie container for maintaining session\. |
| [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects') | Whether to follow redirects\. |
| [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') | Additional headers to add to the request\. |
| [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure') | Whether to ignore SSL certificate errors\. |
| [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds') | Maximum time in seconds for the entire operation\. |
| [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress') | Progress callback \(percent, totalBytes, currentBytes\)\. |
| [OnRedirect](CurlDotNet.Core.CurlSettings.OnRedirect.md 'CurlDotNet\.Core\.CurlSettings\.OnRedirect') | Callback for each redirect\. |
| [Proxy](CurlDotNet.Core.CurlSettings.Proxy.md 'CurlDotNet\.Core\.CurlSettings\.Proxy') | Proxy settings\. |
| [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount') | Number of retry attempts on failure\. |
| [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs') | Delay between retries in milliseconds\. |
| [UserAgent](CurlDotNet.Core.CurlSettings.UserAgent.md 'CurlDotNet\.Core\.CurlSettings\.UserAgent') | Custom user agent string\. |

| Methods | |
| :--- | :--- |
| [FromDefaults\(\)](CurlDotNet.Core.CurlSettings.FromDefaults().md 'CurlDotNet\.Core\.CurlSettings\.FromDefaults\(\)') | Create default settings from global Curl settings\. |
| [WithAutoDecompression\(bool\)](CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithAutoDecompression\(bool\)') | Set automatic decompression\. |
| [WithBufferSize\(int\)](CurlDotNet.Core.CurlSettings.WithBufferSize(int).md 'CurlDotNet\.Core\.CurlSettings\.WithBufferSize\(int\)') | Set buffer size for downloads\. |
| [WithCancellation\(CancellationToken\)](CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).md 'CurlDotNet\.Core\.CurlSettings\.WithCancellation\(System\.Threading\.CancellationToken\)') | Set cancellation token\. |
| [WithConnectTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)') | Set connection timeout\. |
| [WithCookies\(CookieContainer\)](CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).md 'CurlDotNet\.Core\.CurlSettings\.WithCookies\(System\.Net\.CookieContainer\)') | Use cookie container for session management\. |
| [WithFollowRedirects\(bool\)](CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithFollowRedirects\(bool\)') | Enable or disable following redirects\. |
| [WithHeader\(string, string\)](CurlDotNet.Core.CurlSettings.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlSettings\.WithHeader\(string, string\)') | Add a header\. |
| [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlSettings.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlSettings\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') | Add multiple headers\. |
| [WithInsecure\(bool\)](CurlDotNet.Core.CurlSettings.WithInsecure(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithInsecure\(bool\)') | Enable or disable SSL certificate validation\. |
| [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)') | Set progress callback\. |
| [WithProxy\(string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)') | Set proxy\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)') | Set proxy with credentials\. |
| [WithRedirectHandler\(Action&lt;string&gt;\)](CurlDotNet.Core.CurlSettings.WithRedirectHandler(System.Action_string_).md 'CurlDotNet\.Core\.CurlSettings\.WithRedirectHandler\(System\.Action\<string\>\)') | Set redirect callback\. |
| [WithRetries\(int, int\)](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)') | Set retry behavior\. |
| [WithTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)') | Set maximum time for operation\. |
| [WithUserAgent\(string\)](CurlDotNet.Core.CurlSettings.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlSettings\.WithUserAgent\(string\)') | Set custom user agent\. |
