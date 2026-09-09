#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlRequestBuilder Class


<b>🎨 Fluent Builder API - Build curl requests programmatically!</b>

For developers who prefer a fluent API over curl command strings.
             This builder lets you construct HTTP requests using method chaining,
             perfect for IntelliSense and compile-time checking.

<b>When to use Builder vs Curl String:</b>
- <b>Use Builder</b> - When building requests dynamically, need IntelliSense, or prefer type safety
- <b>Use Curl String</b> - When you have curl commands from docs/examples (paste and go!)

<b>Quick Example:</b>

```csharp
// Build a request fluently
var result = await CurlRequestBuilder
    .Get("https://api.example.com/users")
    .WithHeader("Accept", "application/json")
    .WithHeader("Authorization", "Bearer token123")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .ExecuteAsync();

// Same as: curl -H 'Accept: application/json' -H 'Authorization: Bearer token123' --max-time 30 https://api.example.com/users
```

```csharp
public class CurlRequestBuilder
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlRequestBuilder

### Remarks

The builder provides a type-safe, IntelliSense-friendly way to build HTTP requests.
             All builder methods return the builder itself for method chaining.

You can convert a builder to a curl command string using [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)').

| Methods | |
| :--- | :--- |
| [Compressed\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Compressed(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Compressed\(bool\)') | Enable compression \(gzip, deflate, etc\.\)\. |
| [Delete\(string\)](CurlDotNet.Core.CurlRequestBuilder.Delete(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Delete\(string\)') | Create a DELETE request builder\. |
| [Execute\(\)](CurlDotNet.Core.CurlRequestBuilder.Execute().md 'CurlDotNet\.Core\.CurlRequestBuilder\.Execute\(\)') | Execute the request synchronously\. |
| [ExecuteAsync\(CurlSettings, CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(CurlDotNet\.Core\.CurlSettings, System\.Threading\.CancellationToken\)') | Execute the request with custom settings\. |
| [ExecuteAsync\(CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(System\.Threading\.CancellationToken\)') | Execute the request asynchronously\. |
| [FailOnError\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FailOnError(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FailOnError\(bool\)') | Fail on HTTP errors \(like curl \-f\)\. |
| [FollowRedirects\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FollowRedirects\(bool\)') | Enable following redirects \(301, 302, etc\.\)\. |
| [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)') | Create a GET request builder\. |
| [GetOptions\(\)](CurlDotNet.Core.CurlRequestBuilder.GetOptions().md 'CurlDotNet\.Core\.CurlRequestBuilder\.GetOptions\(\)') | Get the underlying options object \(for advanced scenarios\)\. |
| [Head\(string\)](CurlDotNet.Core.CurlRequestBuilder.Head(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Head\(string\)') | Create a HEAD request builder\. |
| [IncludeHeaders\(bool\)](CurlDotNet.Core.CurlRequestBuilder.IncludeHeaders(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.IncludeHeaders\(bool\)') | Include headers in response output \(like curl \-i\)\. |
| [Insecure\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Insecure(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Insecure\(bool\)') | Ignore SSL certificate errors \(not recommended for production\!\)\. |
| [Patch\(string\)](CurlDotNet.Core.CurlRequestBuilder.Patch(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Patch\(string\)') | Create a PATCH request builder\. |
| [Post\(string\)](CurlDotNet.Core.CurlRequestBuilder.Post(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Post\(string\)') | Create a POST request builder\. |
| [Put\(string\)](CurlDotNet.Core.CurlRequestBuilder.Put(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Put\(string\)') | Create a PUT request builder\. |
| [Request\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.Request(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Request\(string, string\)') | Create a custom method request builder\. |
| [SaveToFile\(string\)](CurlDotNet.Core.CurlRequestBuilder.SaveToFile(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.SaveToFile\(string\)') | Save response to file \(like curl \-o\)\. |
| [SaveWithRemoteName\(\)](CurlDotNet.Core.CurlRequestBuilder.SaveWithRemoteName().md 'CurlDotNet\.Core\.CurlRequestBuilder\.SaveWithRemoteName\(\)') | Use remote filename for output \(like curl \-O\)\. |
| [Silent\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Silent(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Silent\(bool\)') | Enable silent mode \(like curl \-s\)\. |
| [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') | Convert this builder to a curl command string\. Useful for debugging or logging what will be executed\. |
| [Verbose\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Verbose(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Verbose\(bool\)') | Enable verbose output \(like curl \-v\)\. |
| [WithAuth\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithAuth(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithAuth\(string\)') | Set custom authentication header\. |
| [WithBasicAuth\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBasicAuth\(string, string\)') | Set basic authentication \(username:password\)\. |
| [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)') | Set bearer token authentication\. |
| [WithBinaryData\(byte\[\]\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryData\(byte\[\]\)') | Add binary data for upload\. |
| [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithConnectTimeout\(System\.TimeSpan\)') | Set connection timeout\. |
| [WithCookie\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookie(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookie\(string\)') | Set cookie string\. |
| [WithCookieJar\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookieJar(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookieJar\(string\)') | Set cookie jar file path\. |
| [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)') | Add POST/PUT data as string\. |
| [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)') | Upload a file\. |
| [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') | Add form data \(application/x\-www\-form\-urlencoded\)\. |
| [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)') | Add a header to the request\. |
| [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') | Add multiple headers at once\. |
| [WithHttpVersion\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithHttpVersion(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHttpVersion\(string\)') | Set HTTP version \(1\.0, 1\.1, or 2\.0\)\. |
| [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)') | Add POST/PUT data as JSON \(automatically serializes and sets Content\-Type\)\. |
| [WithMaxRedirects\(int\)](CurlDotNet.Core.CurlRequestBuilder.WithMaxRedirects(int).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMaxRedirects\(int\)') | Set maximum number of redirects to follow\. |
| [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)') | Add multipart form data with file uploads\. |
| [WithProxy\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string\)') | Set proxy URL\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)') | Set proxy with authentication\. |
| [WithRange\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithRange(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithRange\(string\)') | Set range for partial downloads \(like curl \-r\)\. |
| [WithReferer\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithReferer(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithReferer\(string\)') | Set the Referer header\. |
| [WithTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTimeout\(System\.TimeSpan\)') | Set timeout for the entire operation\. |
| [WithUserAgent\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUserAgent\(string\)') | Set the User\-Agent header\. |
