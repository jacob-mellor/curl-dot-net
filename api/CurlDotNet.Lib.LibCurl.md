#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](index.md#CurlDotNet.Lib 'CurlDotNet\.Lib')

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
### Constructors

<a name='CurlDotNet.Lib.LibCurl.LibCurl()'></a>

## LibCurl\(\) Constructor

Creates a new LibCurl instance\.

```csharp
public LibCurl();
```

### Example

```csharp
using (var curl = new LibCurl())
{
    var result = await curl.GetAsync("https://api.example.com/data");
}
```
### Methods

<a name='CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_)'></a>

## LibCurl\.Configure\(Action\<CurlOptions\>\) Method

Configure default options using an action\.

```csharp
public CurlDotNet.Lib.LibCurl Configure(System.Action<CurlDotNet.Core.CurlOptions> configure);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Configuration action

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

### Example

```csharp
using (var curl = new LibCurl())
{
    curl.Configure(opts => {
        opts.FollowRedirects = true;
        opts.MaxTime = 60;
        opts.Insecure = false;
    });
}
```

<a name='CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken)'></a>

## LibCurl\.DeleteAsync\(string, Action\<CurlOptions\>, CancellationToken\) Method

Perform a DELETE request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DeleteAsync(string url, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to DELETE

<a name='CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional configuration action for this request

<a name='CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request

<a name='CurlDotNet.Lib.LibCurl.Dispose()'></a>

## LibCurl\.Dispose\(\) Method

Dispose resources\.

```csharp
public void Dispose();
```

Implements [Dispose\(\)](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable.dispose 'System\.IDisposable\.Dispose')

<a name='CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken)'></a>

## LibCurl\.GetAsync\(string, Action\<CurlOptions\>, CancellationToken\) Method

Perform a GET request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string url, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to GET

<a name='CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional configuration action for this request

<a name='CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request

### Example

```csharp
using (var curl = new LibCurl())
{
    // Simple GET
    var result = await curl.GetAsync("https://api.example.com/users");

    // GET with per-request configuration
    var result = await curl.GetAsync("https://api.example.com/users", 
        opts => opts.FollowRedirects = true);
}
```

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken)'></a>

## LibCurl\.HeadAsync\(string, Action\<CurlOptions\>, CancellationToken\) Method

Perform a HEAD request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> HeadAsync(string url, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to HEAD

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional configuration action for this request

<a name='CurlDotNet.Lib.LibCurl.HeadAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request

<a name='CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken)'></a>

## LibCurl\.PatchAsync\(string, object, Action\<CurlOptions\>, CancellationToken\) Method

Perform a PATCH request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PatchAsync(string url, object data=null, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to PATCH

<a name='CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Data to send \(object will be JSON serialized, string sent as\-is\)

<a name='CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional configuration action for this request

<a name='CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request

<a name='CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken)'></a>

## LibCurl\.PerformAsync\(CurlOptions, CancellationToken\) Method

Perform a custom request with full control\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PerformAsync(CurlDotNet.Core.CurlOptions options, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

Fully configured CurlOptions

<a name='CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request

<a name='CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken)'></a>

## LibCurl\.PostAsync\(string, object, Action\<CurlOptions\>, CancellationToken\) Method

Perform a POST request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostAsync(string url, object data=null, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to

<a name='CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Data to send \(object will be JSON serialized, string sent as\-is\)

<a name='CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional configuration action for this request

<a name='CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request

### Example

```csharp
using (var curl = new LibCurl())
{
    // POST with object (auto-serialized to JSON)
    var result = await curl.PostAsync("https://api.example.com/users",
        new { name = "John", email = "john@example.com" });

    // POST with string data
    var result = await curl.PostAsync("https://api.example.com/data",
        "key1=value1&key2=value2",
        opts => opts.Headers["Content-Type"] = "application/x-www-form-urlencoded");
}
```

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken)'></a>

## LibCurl\.PutAsync\(string, object, Action\<CurlOptions\>, CancellationToken\) Method

Perform a PUT request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PutAsync(string url, object data=null, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to PUT to

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Data to send \(object will be JSON serialized, string sent as\-is\)

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional configuration action for this request

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancellation token

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the request

<a name='CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string)'></a>

## LibCurl\.WithBasicAuth\(string, string\) Method

Set basic authentication for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithBasicAuth(string username, string password);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Username

<a name='CurlDotNet.Lib.LibCurl.WithBasicAuth(string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Password

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithBearerToken(string)'></a>

## LibCurl\.WithBearerToken\(string\) Method

Set bearer token authentication for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithBearerToken(string token);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithBearerToken(string).token'></a>

`token` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Bearer token

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithConnectTimeout(System.TimeSpan)'></a>

## LibCurl\.WithConnectTimeout\(TimeSpan\) Method

Set connection timeout for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithConnectTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithConnectTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

Connection timeout duration

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithFollowRedirects(int)'></a>

## LibCurl\.WithFollowRedirects\(int\) Method

Enable following redirects for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithFollowRedirects(int maxRedirects=50);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).maxRedirects'></a>

`maxRedirects` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Maximum number of redirects \(default: 50\)

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithHeader(string,string)'></a>

## LibCurl\.WithHeader\(string, string\) Method

Set a default header for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithHeader(string key, string value);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithHeader(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header name

<a name='CurlDotNet.Lib.LibCurl.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header value

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

### Example

```csharp
using (var curl = new LibCurl())
{
    curl.WithHeader("Accept", "application/json")
        .WithHeader("X-API-Key", "your-key");
    
    // All subsequent requests will include these headers
    var result = await curl.GetAsync("https://api.example.com/data");
}
```

<a name='CurlDotNet.Lib.LibCurl.WithInsecureSsl()'></a>

## LibCurl\.WithInsecureSsl\(\) Method

Enable ignoring SSL certificate errors \(for testing only\)\.

```csharp
public CurlDotNet.Lib.LibCurl WithInsecureSsl();
```

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithOutputFile(string)'></a>

## LibCurl\.WithOutputFile\(string\) Method

Set default output file for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithOutputFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithOutputFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Output file path

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string)'></a>

## LibCurl\.WithProxy\(string, string, string\) Method

Set proxy for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithProxy(string proxyUrl, string username=null, string password=null);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy URL \(e\.g\., "http://proxy\.example\.com:8080"\)

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Optional proxy username

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Optional proxy password

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan)'></a>

## LibCurl\.WithTimeout\(TimeSpan\) Method

Set timeout for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

Timeout duration

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithUserAgent(string)'></a>

## LibCurl\.WithUserAgent\(string\) Method

Set user agent for all requests\.

```csharp
public CurlDotNet.Lib.LibCurl WithUserAgent(string userAgent);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithUserAgent(string).userAgent'></a>

`userAgent` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

User agent string

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

<a name='CurlDotNet.Lib.LibCurl.WithVerbose()'></a>

## LibCurl\.WithVerbose\(\) Method

Enable verbose output for debugging\.

```csharp
public CurlDotNet.Lib.LibCurl WithVerbose();
```

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining