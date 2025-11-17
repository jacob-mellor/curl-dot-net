#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

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
### Properties

<a name='CurlDotNet.Core.CurlSettings.AutomaticDecompression'></a>

## CurlSettings\.AutomaticDecompression Property

Whether to automatically decompress response\.

```csharp
public bool AutomaticDecompression { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlSettings.BufferSize'></a>

## CurlSettings\.BufferSize Property

Buffer size for download operations\.

```csharp
public int BufferSize { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlSettings.CancellationToken'></a>

## CurlSettings\.CancellationToken Property

Cancellation token for the operation\.

```csharp
public System.Threading.CancellationToken CancellationToken { get; set; }
```

#### Property Value
[System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

<a name='CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds'></a>

## CurlSettings\.ConnectTimeoutSeconds Property

Connection timeout in seconds\.

```csharp
public System.Nullable<int> ConnectTimeoutSeconds { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlSettings.Cookies'></a>

## CurlSettings\.Cookies Property

Cookie container for maintaining session\.

```csharp
public System.Net.CookieContainer Cookies { get; set; }
```

#### Property Value
[System\.Net\.CookieContainer](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer 'System\.Net\.CookieContainer')

<a name='CurlDotNet.Core.CurlSettings.FollowRedirects'></a>

## CurlSettings\.FollowRedirects Property

Whether to follow redirects\.

```csharp
public System.Nullable<bool> FollowRedirects { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlSettings.Headers'></a>

## CurlSettings\.Headers Property

Additional headers to add to the request\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlSettings.Insecure'></a>

## CurlSettings\.Insecure Property

Whether to ignore SSL certificate errors\.

```csharp
public System.Nullable<bool> Insecure { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlSettings.MaxTimeSeconds'></a>

## CurlSettings\.MaxTimeSeconds Property

Maximum time in seconds for the entire operation\.

```csharp
public System.Nullable<int> MaxTimeSeconds { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlSettings.OnProgress'></a>

## CurlSettings\.OnProgress Property

Progress callback \(percent, totalBytes, currentBytes\)\.

```csharp
public System.Action<double,long,long> OnProgress { get; set; }
```

#### Property Value
[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')

<a name='CurlDotNet.Core.CurlSettings.OnRedirect'></a>

## CurlSettings\.OnRedirect Property

Callback for each redirect\.

```csharp
public System.Action<string> OnRedirect { get; set; }
```

#### Property Value
[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

<a name='CurlDotNet.Core.CurlSettings.Proxy'></a>

## CurlSettings\.Proxy Property

Proxy settings\.

```csharp
public System.Net.IWebProxy Proxy { get; set; }
```

#### Property Value
[System\.Net\.IWebProxy](https://learn.microsoft.com/en-us/dotnet/api/system.net.iwebproxy 'System\.Net\.IWebProxy')

<a name='CurlDotNet.Core.CurlSettings.RetryCount'></a>

## CurlSettings\.RetryCount Property

Number of retry attempts on failure\.

```csharp
public int RetryCount { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlSettings.RetryDelayMs'></a>

## CurlSettings\.RetryDelayMs Property

Delay between retries in milliseconds\.

```csharp
public int RetryDelayMs { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlSettings.UserAgent'></a>

## CurlSettings\.UserAgent Property

Custom user agent string\.

```csharp
public string UserAgent { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
### Methods

<a name='CurlDotNet.Core.CurlSettings.FromDefaults()'></a>

## CurlSettings\.FromDefaults\(\) Method

Create default settings from global Curl settings\.

```csharp
public static CurlDotNet.Core.CurlSettings FromDefaults();
```

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool)'></a>

## CurlSettings\.WithAutoDecompression\(bool\) Method

Set automatic decompression\.

```csharp
public CurlDotNet.Core.CurlSettings WithAutoDecompression(bool enable=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).enable'></a>

`enable` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithBufferSize(int)'></a>

## CurlSettings\.WithBufferSize\(int\) Method

Set buffer size for downloads\.

```csharp
public CurlDotNet.Core.CurlSettings WithBufferSize(int size);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithBufferSize(int).size'></a>

`size` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken)'></a>

## CurlSettings\.WithCancellation\(CancellationToken\) Method

Set cancellation token\.

```csharp
public CurlDotNet.Core.CurlSettings WithCancellation(System.Threading.CancellationToken token);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithCancellation(System.Threading.CancellationToken).token'></a>

`token` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithConnectTimeout(int)'></a>

## CurlSettings\.WithConnectTimeout\(int\) Method

Set connection timeout\.

```csharp
public CurlDotNet.Core.CurlSettings WithConnectTimeout(int seconds);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).seconds'></a>

`seconds` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer)'></a>

## CurlSettings\.WithCookies\(CookieContainer\) Method

Use cookie container for session management\.

```csharp
public CurlDotNet.Core.CurlSettings WithCookies(System.Net.CookieContainer container=null);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).container'></a>

`container` [System\.Net\.CookieContainer](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer 'System\.Net\.CookieContainer')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool)'></a>

## CurlSettings\.WithFollowRedirects\(bool\) Method

Enable or disable following redirects\.

```csharp
public CurlDotNet.Core.CurlSettings WithFollowRedirects(bool follow=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).follow'></a>

`follow` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithHeader(string,string)'></a>

## CurlSettings\.WithHeader\(string, string\) Method

Add a header\.

```csharp
public CurlDotNet.Core.CurlSettings WithHeader(string key, string value);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithHeader(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlSettings.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithHeaders(System.Collections.Generic.Dictionary_string,string_)'></a>

## CurlSettings\.WithHeaders\(Dictionary\<string,string\>\) Method

Add multiple headers\.

```csharp
public CurlDotNet.Core.CurlSettings WithHeaders(System.Collections.Generic.Dictionary<string,string> headers);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithHeaders(System.Collections.Generic.Dictionary_string,string_).headers'></a>

`headers` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithInsecure(bool)'></a>

## CurlSettings\.WithInsecure\(bool\) Method

Enable or disable SSL certificate validation\.

```csharp
public CurlDotNet.Core.CurlSettings WithInsecure(bool insecure=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithInsecure(bool).insecure'></a>

`insecure` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_)'></a>

## CurlSettings\.WithProgress\(Action\<double,long,long\>\) Method

Set progress callback\.

```csharp
public CurlDotNet.Core.CurlSettings WithProgress(System.Action<double,long,long> callback);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).callback'></a>

`callback` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string)'></a>

## CurlSettings\.WithProxy\(string\) Method

Set proxy\.

```csharp
public CurlDotNet.Core.CurlSettings WithProxy(string proxyUrl);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string)'></a>

## CurlSettings\.WithProxy\(string, string, string\) Method

Set proxy with credentials\.

```csharp
public CurlDotNet.Core.CurlSettings WithProxy(string proxyUrl, string username, string password);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlSettings.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithRedirectHandler(System.Action_string_)'></a>

## CurlSettings\.WithRedirectHandler\(Action\<string\>\) Method

Set redirect callback\.

```csharp
public CurlDotNet.Core.CurlSettings WithRedirectHandler(System.Action<string> callback);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithRedirectHandler(System.Action_string_).callback'></a>

`callback` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithRetries(int,int)'></a>

## CurlSettings\.WithRetries\(int, int\) Method

Set retry behavior\.

```csharp
public CurlDotNet.Core.CurlSettings WithRetries(int count, int delayMs=1000);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithRetries(int,int).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlSettings.WithRetries(int,int).delayMs'></a>

`delayMs` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithTimeout(int)'></a>

## CurlSettings\.WithTimeout\(int\) Method

Set maximum time for operation\.

```csharp
public CurlDotNet.Core.CurlSettings WithTimeout(int seconds);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithTimeout(int).seconds'></a>

`seconds` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Core.CurlSettings.WithUserAgent(string)'></a>

## CurlSettings\.WithUserAgent\(string\) Method

Set custom user agent\.

```csharp
public CurlDotNet.Core.CurlSettings WithUserAgent(string userAgent);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithUserAgent(string).userAgent'></a>

`userAgent` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')