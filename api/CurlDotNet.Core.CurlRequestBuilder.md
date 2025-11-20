#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

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

You can convert a builder to a curl command string using [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.md#CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand() 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)').
### Methods

<a name='CurlDotNet.Core.CurlRequestBuilder.Compressed(bool)'></a>

## CurlRequestBuilder\.Compressed\(bool\) Method

Enable compression \(gzip, deflate, etc\.\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Compressed(bool compressed=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Compressed(bool).compressed'></a>

`compressed` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Delete(string)'></a>

## CurlRequestBuilder\.Delete\(string\) Method

Create a DELETE request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Delete(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Delete(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Execute()'></a>

## CurlRequestBuilder\.Execute\(\) Method

Execute the request synchronously\.

```csharp
public CurlDotNet.Core.CurlResult Execute();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken)'></a>

## CurlRequestBuilder\.ExecuteAsync\(CurlSettings, CancellationToken\) Method

Execute the request with custom settings\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Core.CurlSettings settings, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken)'></a>

## CurlRequestBuilder\.ExecuteAsync\(CancellationToken\) Method

Execute the request asynchronously\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlRequestBuilder.FailOnError(bool)'></a>

## CurlRequestBuilder\.FailOnError\(bool\) Method

Fail on HTTP errors \(like curl \-f\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder FailOnError(bool fail=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.FailOnError(bool).fail'></a>

`fail` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool)'></a>

## CurlRequestBuilder\.FollowRedirects\(bool\) Method

Enable following redirects \(301, 302, etc\.\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder FollowRedirects(bool follow=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).follow'></a>

`follow` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Get(string)'></a>

## CurlRequestBuilder\.Get\(string\) Method

Create a GET request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Get(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to GET

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// Simple GET request
var result = await CurlRequestBuilder
    .Get("https://api.github.com/users/octocat")
    .ExecuteAsync();

// GET with headers
var result = await CurlRequestBuilder
    .Get("https://api.example.com/data")
    .WithHeader("Accept", "application/json")
    .WithHeader("X-API-Key", "your-key")
    .ExecuteAsync();

// GET with authentication
var result = await CurlRequestBuilder
    .Get("https://api.example.com/protected")
    .WithBearerToken("your-token")
    .FollowRedirects()
    .ExecuteAsync();
```

<a name='CurlDotNet.Core.CurlRequestBuilder.GetOptions()'></a>

## CurlRequestBuilder\.GetOptions\(\) Method

Get the underlying options object \(for advanced scenarios\)\.

```csharp
public CurlDotNet.Core.CurlOptions GetOptions();
```

#### Returns
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

<a name='CurlDotNet.Core.CurlRequestBuilder.Head(string)'></a>

## CurlRequestBuilder\.Head\(string\) Method

Create a HEAD request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Head(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Head(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.IncludeHeaders(bool)'></a>

## CurlRequestBuilder\.IncludeHeaders\(bool\) Method

Include headers in response output \(like curl \-i\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder IncludeHeaders(bool include=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.IncludeHeaders(bool).include'></a>

`include` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Insecure(bool)'></a>

## CurlRequestBuilder\.Insecure\(bool\) Method

Ignore SSL certificate errors \(not recommended for production\!\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Insecure(bool insecure=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Insecure(bool).insecure'></a>

`insecure` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Patch(string)'></a>

## CurlRequestBuilder\.Patch\(string\) Method

Create a PATCH request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Patch(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Patch(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Post(string)'></a>

## CurlRequestBuilder\.Post\(string\) Method

Create a POST request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Post(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Post(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// POST with JSON
var result = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(new { name = "John", email = "john@example.com" })
    .ExecuteAsync();

// POST with form data
var result = await CurlRequestBuilder
    .Post("https://api.example.com/login")
    .WithFormData(new Dictionary<string, string> {
        { "username", "user123" },
        { "password", "pass456" }
    })
    .ExecuteAsync();

// POST with raw string data
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithData("key1=value1&key2=value2")
    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
    .ExecuteAsync();
```

<a name='CurlDotNet.Core.CurlRequestBuilder.Put(string)'></a>

## CurlRequestBuilder\.Put\(string\) Method

Create a PUT request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Put(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Put(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Request(string,string)'></a>

## CurlRequestBuilder\.Request\(string, string\) Method

Create a custom method request builder\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Request(string method, string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Request(string,string).method'></a>

`method` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.Request(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.SaveToFile(string)'></a>

## CurlRequestBuilder\.SaveToFile\(string\) Method

Save output to a file\. Alias for WithOutput\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder SaveToFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.SaveToFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to save the file

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.SaveWithRemoteName()'></a>

## CurlRequestBuilder\.SaveWithRemoteName\(\) Method

Use remote filename for output \(like curl \-O\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder SaveWithRemoteName();
```

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.Silent(bool)'></a>

## CurlRequestBuilder\.Silent\(bool\) Method

Enable silent mode \(like curl \-s\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Silent(bool silent=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Silent(bool).silent'></a>

`silent` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand()'></a>

## CurlRequestBuilder\.ToCurlCommand\(\) Method

Convert this builder to a curl command string\.
Useful for debugging or logging what will be executed\.

```csharp
public string ToCurlCommand();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.Verbose(bool)'></a>

## CurlRequestBuilder\.Verbose\(bool\) Method

Enable verbose output \(like curl \-v\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Verbose(bool verbose=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Verbose(bool).verbose'></a>

`verbose` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithAuth(string)'></a>

## CurlRequestBuilder\.WithAuth\(string\) Method

Set custom authentication header\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithAuth(string authHeader);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithAuth(string).authHeader'></a>

`authHeader` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string)'></a>

## CurlRequestBuilder\.WithBasicAuth\(string, string\) Method

Set basic authentication \(username:password\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBasicAuth(string username, string password);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string)'></a>

## CurlRequestBuilder\.WithBearerToken\(string\) Method

Set bearer token authentication\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBearerToken(string token);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).token'></a>

`token` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[])'></a>

## CurlRequestBuilder\.WithBinaryData\(byte\[\]\) Method

Add binary data for upload\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBinaryData(byte[] data);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).data'></a>

`data` [System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan)'></a>

## CurlRequestBuilder\.WithConnectTimeout\(TimeSpan\) Method

Set connection timeout\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithConnectTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithCookie(string)'></a>

## CurlRequestBuilder\.WithCookie\(string\) Method

Set cookie string\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithCookie(string cookie);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithCookie(string).cookie'></a>

`cookie` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithCookieJar(string)'></a>

## CurlRequestBuilder\.WithCookieJar\(string\) Method

Set cookie jar file path\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithCookieJar(string cookieJarPath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithCookieJar(string).cookieJarPath'></a>

`cookieJarPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithData(string)'></a>

## CurlRequestBuilder\.WithData\(string\) Method

Add POST/PUT data as string\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithData(string data);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithData(string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string)'></a>

## CurlRequestBuilder\.WithFile\(string\) Method

Upload a file \(multipart/form\-data\)\. Alias for WithUploadFile\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to the file

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string)'></a>

## CurlRequestBuilder\.WithFile\(string, string\) Method

Upload a file\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFile(string fieldName, string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).fieldName'></a>

`fieldName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_)'></a>

## CurlRequestBuilder\.WithFormData\(Dictionary\<string,string\>\) Method

Add form data \(application/x\-www\-form\-urlencoded\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFormData(System.Collections.Generic.Dictionary<string,string> formData);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).formData'></a>

`formData` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string)'></a>

## CurlRequestBuilder\.WithHeader\(string, string\) Method

Add a header to the request\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithHeader(string name, string value);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header name \(e\.g\., "Content\-Type"\)

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header value \(e\.g\., "application/json"\)

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// Add Content-Type header
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { key = "value" })
    .ExecuteAsync();

// Add custom API key header
var result = await CurlRequestBuilder
    .Get("https://api.example.com/protected")
    .WithHeader("X-API-Key", "your-api-key-here")
    .WithHeader("X-Client-Version", "1.0.0")
    .ExecuteAsync();

// Add Accept header for API versioning
var result = await CurlRequestBuilder
    .Get("https://api.github.com/user")
    .WithHeader("Accept", "application/vnd.github.v3+json")
    .WithBearerToken("your-token")
    .ExecuteAsync();
```

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeaders(System.Collections.Generic.Dictionary_string,string_)'></a>

## CurlRequestBuilder\.WithHeaders\(Dictionary\<string,string\>\) Method

Add multiple headers at once\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithHeaders(System.Collections.Generic.Dictionary<string,string> headers);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeaders(System.Collections.Generic.Dictionary_string,string_).headers'></a>

`headers` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHttpVersion(string)'></a>

## CurlRequestBuilder\.WithHttpVersion\(string\) Method

Set HTTP version \(1\.0, 1\.1, or 2\.0\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithHttpVersion(string version);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHttpVersion(string).version'></a>

`version` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithJson(object)'></a>

## CurlRequestBuilder\.WithJson\(object\) Method

Add POST/PUT data as JSON \(automatically serializes and sets Content\-Type\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithJson(object data);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithJson(object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to serialize as JSON\. Can be any class, anonymous object, or built\-in type\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining

### Example

```csharp
// POST with anonymous object
var result = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(new { name = "John", email = "john@example.com", age = 30 })
    .ExecuteAsync();

// POST with typed class
public class User {
    public string Name { get; set; }
    public string Email { get; set; }
}
var user = new User { Name = "John", Email = "john@example.com" };
var result = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(user)
    .ExecuteAsync();

// PUT with JSON update
var result = await CurlRequestBuilder
    .Put("https://api.example.com/users/123")
    .WithJson(new { name = "Jane", email = "jane@example.com" })
    .WithBearerToken("your-token")
    .ExecuteAsync();
```

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMaxRedirects(int)'></a>

## CurlRequestBuilder\.WithMaxRedirects\(int\) Method

Set maximum number of redirects to follow\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithMaxRedirects(int maxRedirects);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMaxRedirects(int).maxRedirects'></a>

`maxRedirects` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_)'></a>

## CurlRequestBuilder\.WithMultipartForm\(Dictionary\<string,string\>, Dictionary\<string,string\>\) Method

Add multipart form data with file uploads\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithMultipartForm(System.Collections.Generic.Dictionary<string,string> fields, System.Collections.Generic.Dictionary<string,string> files=null);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).fields'></a>

`fields` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).files'></a>

`files` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithOutput(string)'></a>

## CurlRequestBuilder\.WithOutput\(string\) Method

Save output to a file\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithOutput(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to save the file

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string)'></a>

## CurlRequestBuilder\.WithProxy\(string\) Method

Set proxy URL\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithProxy(string proxyUrl);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string)'></a>

## CurlRequestBuilder\.WithProxy\(string, string, string\) Method

Set proxy with authentication\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithProxy(string proxyUrl, string username, string password);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithRange(string)'></a>

## CurlRequestBuilder\.WithRange\(string\) Method

Set range for partial downloads \(like curl \-r\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithRange(string range);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithRange(string).range'></a>

`range` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithReferer(string)'></a>

## CurlRequestBuilder\.WithReferer\(string\) Method

Set the Referer header\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithReferer(string referer);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithReferer(string).referer'></a>

`referer` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan)'></a>

## CurlRequestBuilder\.WithTimeout\(TimeSpan\) Method

Set timeout for the entire operation\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string)'></a>

## CurlRequestBuilder\.WithUploadFile\(string, string\) Method

Upload a file \(multipart/form\-data\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithUploadFile(string parameterName, string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).parameterName'></a>

`parameterName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The form field name

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to the file

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string)'></a>

## CurlRequestBuilder\.WithUserAgent\(string\) Method

Set the User\-Agent header\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithUserAgent(string userAgent);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string).userAgent'></a>

`userAgent` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')