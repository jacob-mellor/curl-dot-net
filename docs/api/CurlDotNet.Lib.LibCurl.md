#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib')

## LibCurl Class

A reusable HTTP client in the style of libcurl's easy handle: set defaults once \(headers, token,
timeouts, proxy\) and make many requests with [GetAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.GetAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)'), [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') and friends\.
Transfer failures are returned in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') instead of being thrown\.

```csharp
public class LibCurl : System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; LibCurl

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

### Example

```csharp
using (var curl = new LibCurl())
{
    curl.WithHeader("Accept", "application/json")
        .WithBearerToken("your-token")
        .WithTimeout(TimeSpan.FromSeconds(30));

    var user = await curl.GetAsync("https://api.example.com/user");
    if (user.Exception != null)
        Console.WriteLine(user.Exception.Message);   // what happened, how to fix it, documentation link
    else if (user.IsSuccess)
        Console.WriteLine(user.Body);
}
```

### Remarks

Configure the instance with the fluent `With...` methods (or [Configure\(Action&lt;CurlOptions&gt;\)](CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_).md 'CurlDotNet\.Lib\.LibCurl\.Configure\(System\.Action\<CurlDotNet\.Core\.CurlOptions\>\)')), then call the
             request methods. Each request starts from its own [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions'); the defaults fill in only what
             the request did not set itself, and the optional `configure` callback of each request method runs last,
             so it can override anything for that one call.

Error model, like libcurl's `CURLcode`: a failed transfer (DNS, connection refused, timeout, TLS,
             write error) does not throw. The method returns a [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with `StatusCode == 0`, an
             empty body and the [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') (its message says what
             happened and how to fix it). HTTP error statuses (404, 500) are normal results with that status code. Only
             caller cancellation ([System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')) and a body object that cannot be serialized to JSON
             ([CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')) throw. If you prefer exceptions, use
             [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') or [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'), or call
             `if (result.Exception != null) throw result.Exception;`.

The transport behaves like the curl tool: redirects are followed only after
             [WithFollowRedirects\(int\)](CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).md 'CurlDotNet\.Lib\.LibCurl\.WithFollowRedirects\(int\)'), there is no time limit unless [WithTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithTimeout\(System\.TimeSpan\)') is set, responses are
             not decompressed unless asked, and [WithProxy\(string, string, string\)](CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithProxy\(string, string, string\)') / [WithInsecureSsl\(\)](CurlDotNet.Lib.LibCurl.WithInsecureSsl().md 'CurlDotNet\.Lib\.LibCurl\.WithInsecureSsl\(\)') apply to every request.

Thread safety: configure the instance first; after that, any number of requests may run concurrently
             on it. Do not call the `With...` methods or [Configure\(Action&lt;CurlOptions&gt;\)](CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_).md 'CurlDotNet\.Lib\.LibCurl\.Configure\(System\.Action\<CurlDotNet\.Core\.CurlOptions\>\)') while requests are running.

Example: `using var api = new LibCurl().WithHeader("Accept", "application/json").WithBearerToken(token).WithTimeout(TimeSpan.FromSeconds(30));`
             then `var user = await api.GetAsync("https://api.example.com/user");` and
             `var posts = await api.GetAsync("https://api.example.com/posts");` both send the same headers and limit.

How this maps to libcurl's `curl_easy_setopt` options:
             https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/libcurl-mapping.md

### See Also
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')
- [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')
- [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception')

| Constructors | |
| :--- | :--- |
| [LibCurl\(\)](CurlDotNet.Lib.LibCurl.LibCurl().md 'CurlDotNet\.Lib\.LibCurl\.LibCurl\(\)') | Creates a client with no defaults: curl's behaviour \(no redirects followed, no time limit, TLS certificates verified, proxy from the `http_proxy`/`https_proxy` environment variables\)\. |

| Methods | |
| :--- | :--- |
| [Configure\(Action&lt;CurlOptions&gt;\)](CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_).md 'CurlDotNet\.Lib\.LibCurl\.Configure\(System\.Action\<CurlDotNet\.Core\.CurlOptions\>\)') | Edits this client's default [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') with a callback, as an alternative to the `With...` methods; only the defaults listed in the remarks are applied to requests\. |
| [DeleteAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.DeleteAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Sends a DELETE request \(`curl -X DELETE URL`\) to remove the resource at [url](CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url 'CurlDotNet\.Lib\.LibCurl\.DeleteAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.url'); most APIs answer 200, 202 or 204\. |
| [Dispose\(\)](CurlDotNet.Lib.LibCurl.Dispose().md 'CurlDotNet\.Lib\.LibCurl\.Dispose\(\)') | Releases this client's connections and handlers; call it \(or use `using`\) when the client is no longer needed, after its requests have finished\. |
| [GetAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.GetAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Sends a GET request \(`curl URL`\) with this client's defaults and returns the response; failures come back in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') instead of being thrown\. |
| [HeadAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.HeadAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Sends a HEAD request: only the status and headers come back \(in [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')\), so you can check existence, size or `Last-Modified` without downloading the body\. |
| [PatchAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PatchAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Sends a PATCH request \(`curl -X PATCH`\) with the fields to change; the body follows the same rules as [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') \(string as\-is, object as JSON\)\. |
| [PerformAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PerformAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)') | Runs a request you built yourself as [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') \(any method, body, form, output file or curl option\), with this client's defaults filled in; libcurl's `curl_easy_perform`\. |
| [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Sends a POST request with a body: a string is sent as\-is \(form\-encoded content type\), any other object is serialized to JSON and sent with `Content-Type: application/json`\. |
| [PutAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PutAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') | Sends a PUT request \(`curl -X PUT`\) to create or replace the resource at [url](CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url 'CurlDotNet\.Lib\.LibCurl\.PutAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.url'); the body follows the same rules as [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') \(string as\-is, object as JSON\)\. |
| [WithBasicAuth\(string, string\)](CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithBasicAuth\(string, string\)') | Sends HTTP Basic credentials \(curl's `-u user:password`\) with every request that does not set its own [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')\. |
| [WithBearerToken\(string\)](CurlDotNet.Lib.LibCurl.WithBearerToken(string).md 'CurlDotNet\.Lib\.LibCurl\.WithBearerToken\(string\)') | Sends `Authorization: Bearer TOKEN` \(OAuth 2 / JWT\) with every request from this client, like `curl -H 'Authorization: Bearer TOKEN'` or `--oauth2-bearer TOKEN`\. |
| [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithConnectTimeout\(System\.TimeSpan\)') | Limits only the connection phase \(DNS, TCP connect, TLS handshake\) of every request, like curl's `--connect-timeout`, so a dead server fails fast while slow downloads may continue\. |
| [WithFollowRedirects\(int\)](CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).md 'CurlDotNet\.Lib\.LibCurl\.WithFollowRedirects\(int\)') | Follows `Location:` redirects \(301, 302, 303, 307, 308\) for every request, like curl's `-L`, up to [maxRedirects](CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).md#CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).maxRedirects 'CurlDotNet\.Lib\.LibCurl\.WithFollowRedirects\(int\)\.maxRedirects') hops; without it you get the 3xx response itself\. |
| [WithHeader\(string, string\)](CurlDotNet.Lib.LibCurl.WithHeader(string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithHeader\(string, string\)') | Adds a header that every request from this client sends \(curl's `-H 'Name: value'`\), unless the request sets a header with the same name itself\. |
| [WithInsecureSsl\(\)](CurlDotNet.Lib.LibCurl.WithInsecureSsl().md 'CurlDotNet\.Lib\.LibCurl\.WithInsecureSsl\(\)') | Skips TLS certificate verification for every request \(curl's `-k` / `--insecure`\), accepting self\-signed and expired certificates\. Development and testing only\. |
| [WithOutputFile\(string\)](CurlDotNet.Lib.LibCurl.WithOutputFile(string).md 'CurlDotNet\.Lib\.LibCurl\.WithOutputFile\(string\)') | Writes the body of every request to [filePath](CurlDotNet.Lib.LibCurl.WithOutputFile(string).md#CurlDotNet.Lib.LibCurl.WithOutputFile(string).filePath 'CurlDotNet\.Lib\.LibCurl\.WithOutputFile\(string\)\.filePath') \(curl's `-o`\), overwriting it each time, unless a request sets its own [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithProxy\(string, string, string\)') | Sends every request through a proxy \(curl's `-x URL`, with `--proxy-user user:password` when credentials are given\), for corporate proxies, Fiddler or SOCKS tunnels\. |
| [WithTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithTimeout\(System\.TimeSpan\)') | Limits every request's total time \(connect, send, wait, download\) to [timeout](CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).md#CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).timeout 'CurlDotNet\.Lib\.LibCurl\.WithTimeout\(System\.TimeSpan\)\.timeout'), like curl's `-m` / `--max-time`; without it there is no limit\. |
| [WithUserAgent\(string\)](CurlDotNet.Lib.LibCurl.WithUserAgent(string).md 'CurlDotNet\.Lib\.LibCurl\.WithUserAgent\(string\)') | Sets the `User-Agent` header for every request \(curl's `-A`\); without it CurlDotNet sends `curl/8.x` like the curl tool\. |
| [WithVerbose\(\)](CurlDotNet.Lib.LibCurl.WithVerbose().md 'CurlDotNet\.Lib\.LibCurl\.WithVerbose\(\)') | Records curl's `-v` verbose log \(request and response headers, connection notes\) at the start of every result's [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), for debugging\. |
