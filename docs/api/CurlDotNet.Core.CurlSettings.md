#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlSettings Class

Per\-call \.NET settings \(cancellation, timeouts, retries, a progress callback, extra headers\) that you pass
next to a curl command string instead of writing them as flags\.

```csharp
public class CurlSettings
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlSettings

### Example

```csharp
var settings = new CurlSettings()
    .WithTimeout(30)
    .WithRetries(3)
    .WithProgress((percent, received, total) => Console.WriteLine(percent < 0 ? $"{received} bytes" : $"{percent:F0}%"))
    .WithCancellation(cancellationToken);

var result = await Curl.ExecuteAsync("curl https://example.com", settings);
```

### Remarks

Pass an instance to [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)'),
             [Execute\(string, CurlSettings\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)') or [ExecuteAsync\(CurlSettings, CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(CurlDotNet\.Core\.CurlSettings, System\.Threading\.CancellationToken\)').
             Values set here override the same option in the command: [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds') wins over `-m`,
             [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') replace a `-H` header of the same name, and so on. Settings left at null/0 leave the
             command (and the `Curl.Default*` globals) in charge. A command with several URLs (globs, `--next`) gets the
             settings on every one of its transfers.

Every member is applied: [CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken'), [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds') (`-m`),
             [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds') (`--connect-timeout`), [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects') (`-L`),
             [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure') (`-k`), [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount')/[RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs')
             (`--retry N --retry-delay S --retry-all-errors`), [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress'), [OnRedirect](CurlDotNet.Core.CurlSettings.OnRedirect.md 'CurlDotNet\.Core\.CurlSettings\.OnRedirect') (called
             for every redirect hop), [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') (`-H`), [Proxy](CurlDotNet.Core.CurlSettings.Proxy.md 'CurlDotNet\.Core\.CurlSettings\.Proxy') (`-x`/`-U`),
             [UserAgent](CurlDotNet.Core.CurlSettings.UserAgent.md 'CurlDotNet\.Core\.CurlSettings\.UserAgent') (`-A`), [Cookies](CurlDotNet.Core.CurlSettings.Cookies.md 'CurlDotNet\.Core\.CurlSettings\.Cookies') (a .NET cookie container that is sent and updated),
             [AutomaticDecompression](CurlDotNet.Core.CurlSettings.AutomaticDecompression.md 'CurlDotNet\.Core\.CurlSettings\.AutomaticDecompression') (`--compressed`, only when you set it), [BufferSize](CurlDotNet.Core.CurlSettings.BufferSize.md 'CurlDotNet\.Core\.CurlSettings\.BufferSize') (the
             body read size, only when you set it) and [Flavor](CurlDotNet.Core.CurlSettings.Flavor.md 'CurlDotNet\.Core\.CurlSettings\.Flavor').

An instance is a plain mutable object and is not thread-safe while you are changing it; once built
             it can be shared by concurrent calls because the engine only reads it.

Example: `var settings = new CurlSettings().WithTimeout(30).WithRetries(3).WithProgress((pct, got, total) => Console.Write($"\r{pct:F0}%")).WithCancellation(token);`
             then `var result = await Curl.ExecuteAsync("curl -O https://example.com/big.zip", settings);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

### See Also
- [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')
- [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')
- [FromDefaults\(\)](CurlDotNet.Core.CurlSettings.FromDefaults().md 'CurlDotNet\.Core\.CurlSettings\.FromDefaults\(\)')

| Properties | |
| :--- | :--- |
| [AutomaticDecompression](CurlDotNet.Core.CurlSettings.AutomaticDecompression.md 'CurlDotNet\.Core\.CurlSettings\.AutomaticDecompression') | Requests and decodes compressed responses \(true\) or turns that off \(false\), like curl's `--compressed` and `--no-compressed`; applied only when you set it\. |
| [BufferSize](CurlDotNet.Core.CurlSettings.BufferSize.md 'CurlDotNet\.Core\.CurlSettings\.BufferSize') | The size, in bytes, of each read of an http/https response body \(and so of each [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress') report\); applied only when you set it\. |
| [CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken') | The token that cancels the transfer; when it fires, the call throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') \(its InnerException is a [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException'), curl exit code 42\)\. |
| [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds') | Limit for establishing the connection \(DNS, TCP and TLS\) in whole seconds, like curl's `--connect-timeout`; exceeding it throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') \(exit code 28\)\. |
| [Cookies](CurlDotNet.Core.CurlSettings.Cookies.md 'CurlDotNet\.Core\.CurlSettings\.Cookies') | A \.NET cookie container that supplies the `Cookie` header of every request and receives every `Set-Cookie` of the responses, so a session carries over from one call to the next\. |
| [Flavor](CurlDotNet.Core.CurlSettings.Flavor.md 'CurlDotNet\.Core\.CurlSettings\.Flavor') | The shell the command string was written for, which decides how its quotes, escapes, line continuations and variables are read; `null` \(the default\) means [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')\. |
| [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects') | Whether to follow HTTP 3xx redirects, like curl's `-L`/`--location`; null leaves the command \(or [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects')\) in charge\. |
| [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') | Extra request headers added to \(or replacing headers of the same name in\) the command, like extra `-H "Name: value"` flags\. |
| [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure') | Whether to skip TLS certificate verification, like curl's `-k`/`--insecure`; only for testing against self\-signed or local certificates\. |
| [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds') | Limit for the whole transfer in whole seconds, like curl's `-m`/`--max-time`; when it expires the call throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') \(exit code 28\)\. |
| [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress') | Callback invoked for every chunk of the response body as it downloads, with `(percent, bytesReceived, totalBytes)`\. |
| [OnRedirect](CurlDotNet.Core.CurlSettings.OnRedirect.md 'CurlDotNet\.Core\.CurlSettings\.OnRedirect') | Called with the absolute target URL of every redirect that is followed \(each `Location` hop with `-L`\), before the next request is sent\. |
| [Proxy](CurlDotNet.Core.CurlSettings.Proxy.md 'CurlDotNet\.Core\.CurlSettings\.Proxy') | A \.NET proxy to send the request through, like curl's `-x`/`--proxy` \(with `-U` for its credentials\); it replaces a proxy given in the command\. |
| [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount') | How many times to retry a failed transfer \(any failure, not only transient ones\), like curl's `--retry N --retry-all-errors`; 0 \(the default\) means no retries\. |
| [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs') | Pause between retries in milliseconds, like curl's `--retry-delay` \(which takes seconds\); only used when [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount') is greater than 0\. |
| [UserAgent](CurlDotNet.Core.CurlSettings.UserAgent.md 'CurlDotNet\.Core\.CurlSettings\.UserAgent') | The `User-Agent` header to send, like curl's `-A`/`--user-agent`; it replaces the command's `-A` and any `-H "User-Agent: ..."`\. |

| Methods | |
| :--- | :--- |
| [FromDefaults\(\)](CurlDotNet.Core.CurlSettings.FromDefaults().md 'CurlDotNet\.Core\.CurlSettings\.FromDefaults\(\)') | Creates settings pre\-filled from the global `Curl.Default*` properties \(timeouts, redirects, certificate checking, flavor\), so you can start from the process\-wide defaults and tweak one value\. |
| [WithAutoDecompression\(bool\)](CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithAutoDecompression\(bool\)') | Requests and decodes compressed responses \([enable](CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).md#CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).enable 'CurlDotNet\.Core\.CurlSettings\.WithAutoDecompression\(bool\)\.enable') true, like `--compressed`\) or turns that off \(false, like `--no-compressed`\), and returns these settings for chaining\. |
| [WithBufferSize\(int\)](CurlDotNet.Core.CurlSettings.WithBufferSize(int).md 'CurlDotNet\.Core\.CurlSettings\.WithBufferSize\(int\)') | Sets [BufferSize](CurlDotNet.Core.CurlSettings.BufferSize.md 'CurlDotNet\.Core\.CurlSettings\.BufferSize'), the size of each response\-body read, and returns these settings for chaining\. |
| [WithCancellation\(CancellationToken\)](CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).md 'CurlDotNet\.Core\.CurlSettings\.WithCancellation\(System\.Threading\.CancellationToken\)') | Sets [CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken') so that cancelling [token](CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).md#CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).token 'CurlDotNet\.Core\.CurlSettings\.WithCancellation\(System\.Threading\.CancellationToken\)\.token') stops the transfer, and returns these settings for chaining\. |
| [WithConnectTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)') | Limits only the connection phase to [seconds](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md#CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).seconds 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)\.seconds') \(curl `--connect-timeout`\) and returns these settings for chaining\. |
| [WithCookies\(CookieContainer\)](CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).md 'CurlDotNet\.Core\.CurlSettings\.WithCookies\(System\.Net\.CookieContainer\)') | Uses [container](CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).md#CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).container 'CurlDotNet\.Core\.CurlSettings\.WithCookies\(System\.Net\.CookieContainer\)\.container') \(see [Cookies](CurlDotNet.Core.CurlSettings.Cookies.md 'CurlDotNet\.Core\.CurlSettings\.Cookies')\) to send and collect cookies, and returns these settings for chaining\. |
| [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)') | Sets the shell the command string was written for \(see [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')\) and returns these settings for chaining\. |
| [WithFollowRedirects\(bool\)](CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithFollowRedirects\(bool\)') | Turns following of 3xx redirects on or off \(curl `-L`\) and returns these settings for chaining\. |
| [WithHeader\(string, string\)](CurlDotNet.Core.CurlSettings.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlSettings\.WithHeader\(string, string\)') | Adds \(or replaces\) one request header, like an extra `-H "key: value"`, and returns these settings for chaining\. |
| [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlSettings.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlSettings\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') | Adds \(or replaces\) several request headers at once, like several `-H` flags, and returns these settings for chaining\. |
| [WithInsecure\(bool\)](CurlDotNet.Core.CurlSettings.WithInsecure(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithInsecure\(bool\)') | Turns TLS certificate verification off \(curl `-k`\) or back on, and returns these settings for chaining\. |
| [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)') | Registers a callback that receives `(percent, bytesReceived, totalBytes)` for every chunk of the response body, and returns these settings for chaining\. |
| [WithProxy\(string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)') | Sends the request through the proxy at [proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)\.proxyUrl') \(sets [Proxy](CurlDotNet.Core.CurlSettings.Proxy.md 'CurlDotNet\.Core\.CurlSettings\.Proxy') to a [System\.Net\.WebProxy](https://learn.microsoft.com/en-us/dotnet/api/system.net.webproxy 'System\.Net\.WebProxy')\), like curl's `-x URL`, and returns these settings for chaining\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)') | Sends the request through the proxy at [proxyUrl](CurlDotNet.Core.CurlSettings.md#CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).proxyUrl 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string, string, string\)\.proxyUrl') with Basic proxy credentials, like curl's `-x URL -U user:pass`, and returns these settings for chaining\. |
| [WithRedirectHandler\(Action&lt;string&gt;\)](CurlDotNet.Core.CurlSettings.WithRedirectHandler(System.Action_string_).md 'CurlDotNet\.Core\.CurlSettings\.WithRedirectHandler\(System\.Action\<string\>\)') | Sets [OnRedirect](CurlDotNet.Core.CurlSettings.OnRedirect.md 'CurlDotNet\.Core\.CurlSettings\.OnRedirect'), called with the absolute URL of every redirect that is followed, and returns these settings for chaining\. |
| [WithRetries\(int, int\)](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)') | Retries any failed transfer up to [count](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md#CurlDotNet.Core.CurlSettings.WithRetries(int,int).count 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)\.count') more times with a fixed pause \(curl `--retry N --retry-delay S --retry-all-errors`\), and returns these settings for chaining\. |
| [WithTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)') | Limits the whole transfer to [seconds](CurlDotNet.Core.CurlSettings.WithTimeout(int).md#CurlDotNet.Core.CurlSettings.WithTimeout(int).seconds 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)\.seconds') \(curl `-m`/`--max-time`\) and returns these settings for chaining\. |
| [WithUserAgent\(string\)](CurlDotNet.Core.CurlSettings.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlSettings\.WithUserAgent\(string\)') | Sets the `User-Agent` header \(see [UserAgent](CurlDotNet.Core.CurlSettings.UserAgent.md 'CurlDotNet\.Core\.CurlSettings\.UserAgent')\), like curl's `-A`, and returns these settings for chaining\. |
