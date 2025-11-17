#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

## CurlOptions Class

Represents parsed curl command options\.

```csharp
public class CurlOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlOptions

### Remarks

This class contains all options extracted from a curl command string.

AI-Usage: This is the intermediate representation between curl syntax and HTTP operations.
### Properties

<a name='CurlDotNet.Core.CurlOptions.BinaryData'></a>

## CurlOptions\.BinaryData Property

Binary data for upload\.

```csharp
public byte[]? BinaryData { get; set; }
```

#### Property Value
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='CurlDotNet.Core.CurlOptions.CaCert'></a>

## CurlOptions\.CaCert Property

Alias for CaCertFile for test compatibility\.

```csharp
public string CaCert { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.CaCertFile'></a>

## CurlOptions\.CaCertFile Property

CA certificate file \(\-\-cacert\)\.

```csharp
public string? CaCertFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.CertFile'></a>

## CurlOptions\.CertFile Property

Certificate file for client authentication \(\-\-cert\)\.

```csharp
public string? CertFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.Compressed'></a>

## CurlOptions\.Compressed Property

Whether to use compressed encoding \(\-\-compressed\)\.

```csharp
public bool Compressed { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.ConnectTimeout'></a>

## CurlOptions\.ConnectTimeout Property

Connection timeout in seconds \(\-\-connect\-timeout\)\.

```csharp
public System.Nullable<int> ConnectTimeout { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlOptions.Cookie'></a>

## CurlOptions\.Cookie Property

Cookie string \(\-b or \-\-cookie\)\.

```csharp
public string? Cookie { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.CookieJar'></a>

## CurlOptions\.CookieJar Property

Cookie jar file path \(\-c or \-\-cookie\-jar\)\.

```csharp
public string? CookieJar { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.CreateDirs'></a>

## CurlOptions\.CreateDirs Property

Whether to create missing directories for output \(\-\-create\-dirs\)\.

```csharp
public bool CreateDirs { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Credentials'></a>

## CurlOptions\.Credentials Property

Basic authentication \(\-u or \-\-user\)\.

```csharp
public System.Net.NetworkCredential? Credentials { get; set; }
```

#### Property Value
[System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')

<a name='CurlDotNet.Core.CurlOptions.CustomMethod'></a>

## CurlOptions\.CustomMethod Property

Request method override \(\-X flag value\)\.

```csharp
public string? CustomMethod { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.Data'></a>

## CurlOptions\.Data Property

Request body data\.

```csharp
public string? Data { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.DataUrlEncode'></a>

## CurlOptions\.DataUrlEncode Property

Whether to URL encode the data \(\-\-data\-urlencode flag\)\.

```csharp
public bool DataUrlEncode { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.DisableEprt'></a>

## CurlOptions\.DisableEprt Property

Whether to disable EPRT for FTP \(\-\-disable\-eprt\)\.

```csharp
public bool DisableEprt { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.DisableEpsv'></a>

## CurlOptions\.DisableEpsv Property

Whether to disable EPSV for FTP \(\-\-disable\-epsv\)\.

```csharp
public bool DisableEpsv { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.DnsServers'></a>

## CurlOptions\.DnsServers Property

DNS servers to use \(\-\-dns\-servers\)\.

```csharp
public string? DnsServers { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.FailOnError'></a>

## CurlOptions\.FailOnError Property

Fail silently on HTTP errors \(\-f flag\)\.

```csharp
public bool FailOnError { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Files'></a>

## CurlOptions\.Files Property

Files to upload \(for \-F flag with @file\)\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Files { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlOptions.FollowLocation'></a>

## CurlOptions\.FollowLocation Property

Whether to follow redirects \(\-L flag\)\.

```csharp
public bool FollowLocation { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.FollowRedirects'></a>

## CurlOptions\.FollowRedirects Property

Alias for FollowLocation for test compatibility\.

```csharp
public bool FollowRedirects { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.FormData'></a>

## CurlOptions\.FormData Property

Form data for multipart/form\-data\.

```csharp
public System.Collections.Generic.Dictionary<string,string> FormData { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlOptions.FtpPassive'></a>

## CurlOptions\.FtpPassive Property

Whether to use passive mode for FTP \(\-\-ftp\-pasv\)\.

```csharp
public bool FtpPassive { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.FtpSsl'></a>

## CurlOptions\.FtpSsl Property

Whether to use SSL/TLS for FTP \(\-\-ftp\-ssl\)\.

```csharp
public bool FtpSsl { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Headers'></a>

## CurlOptions\.Headers Property

Request headers\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlOptions.HeadOnly'></a>

## CurlOptions\.HeadOnly Property

Whether to show only headers \(\-I or \-\-head flag\)\.

```csharp
public bool HeadOnly { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.HttpVersion'></a>

## CurlOptions\.HttpVersion Property

HTTP version to use \(\-\-http1\.0, \-\-http1\.1, \-\-http2\)\.

```csharp
public string? HttpVersion { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.IncludeHeaders'></a>

## CurlOptions\.IncludeHeaders Property

Whether to include headers in output \(\-i flag\)\.

```csharp
public bool IncludeHeaders { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Insecure'></a>

## CurlOptions\.Insecure Property

Whether to ignore SSL errors \(\-k flag\)\.

```csharp
public bool Insecure { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Interface'></a>

## CurlOptions\.Interface Property

Interface to use for outgoing connections \(\-\-interface\)\.

```csharp
public string? Interface { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.KeepAliveTime'></a>

## CurlOptions\.KeepAliveTime Property

Whether to use TCP keepalive \(\-\-keepalive\-time\)\.

```csharp
public System.Nullable<int> KeepAliveTime { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlOptions.KeyFile'></a>

## CurlOptions\.KeyFile Property

Certificate key file \(\-\-key\)\.

```csharp
public string? KeyFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.LocationTrusted'></a>

## CurlOptions\.LocationTrusted Property

Location trusted for automatic authentication \(\-\-location\-trusted\)\.

```csharp
public bool LocationTrusted { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.MaxRedirects'></a>

## CurlOptions\.MaxRedirects Property

Maximum number of redirects to follow \(\-\-max\-redirs\)\.

```csharp
public int MaxRedirects { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlOptions.MaxTime'></a>

## CurlOptions\.MaxTime Property

Maximum time in seconds \(\-\-max\-time\)\.

```csharp
public System.Nullable<int> MaxTime { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlOptions.Method'></a>

## CurlOptions\.Method Property

HTTP method \(GET, POST, PUT, DELETE, etc\.\)\.

```csharp
public string? Method { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.OriginalCommand'></a>

## CurlOptions\.OriginalCommand Property

The original command string\.

```csharp
public string? OriginalCommand { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.OutputFile'></a>

## CurlOptions\.OutputFile Property

Output file path \(\-o flag\)\.

```csharp
public string? OutputFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.ProgressBar'></a>

## CurlOptions\.ProgressBar Property

Whether to show progress bar \(\-\-progress\-bar\)\.

```csharp
public bool ProgressBar { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.ProgressHandler'></a>

## CurlOptions\.ProgressHandler Property

Progress callback handler\.

```csharp
public System.Action<double,long,long>? ProgressHandler { get; set; }
```

#### Property Value
[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')

<a name='CurlDotNet.Core.CurlOptions.Proxy'></a>

## CurlOptions\.Proxy Property

Proxy URL \(\-x or \-\-proxy\)\.

```csharp
public string Proxy { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.ProxyCredentials'></a>

## CurlOptions\.ProxyCredentials Property

Proxy authentication \(\-\-proxy\-user\)\.

```csharp
public System.Net.NetworkCredential? ProxyCredentials { get; set; }
```

#### Property Value
[System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')

<a name='CurlDotNet.Core.CurlOptions.Quote'></a>

## CurlOptions\.Quote Property

FTP/SFTP commands to execute after transfer \(\-\-quote\)\.

```csharp
public System.Collections.Generic.List<string> Quote { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='CurlDotNet.Core.CurlOptions.Range'></a>

## CurlOptions\.Range Property

Range of bytes to request \(\-r or \-\-range\)\.

```csharp
public string? Range { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.Referer'></a>

## CurlOptions\.Referer Property

Referer header \(\-e or \-\-referer\)\.

```csharp
public string? Referer { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.Resolve'></a>

## CurlOptions\.Resolve Property

Resolve host to IP \(\-\-resolve\)\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Resolve { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlOptions.ResumeFrom'></a>

## CurlOptions\.ResumeFrom Property

Resume from byte offset \(\-C or \-\-continue\-at\)\.

```csharp
public System.Nullable<long> ResumeFrom { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Core.CurlOptions.Retry'></a>

## CurlOptions\.Retry Property

Retry count \(\-\-retry\)\.

```csharp
public int Retry { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlOptions.RetryDelay'></a>

## CurlOptions\.RetryDelay Property

Retry delay in seconds \(\-\-retry\-delay\)\.

```csharp
public int RetryDelay { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlOptions.RetryMaxTime'></a>

## CurlOptions\.RetryMaxTime Property

Maximum retry time \(\-\-retry\-max\-time\)\.

```csharp
public int RetryMaxTime { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlOptions.ShowError'></a>

## CurlOptions\.ShowError Property

Show error even in silent mode \(\-S flag\)\.

```csharp
public bool ShowError { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Silent'></a>

## CurlOptions\.Silent Property

Silent mode \(\-s flag\)\.

```csharp
public bool Silent { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Socks5Proxy'></a>

## CurlOptions\.Socks5Proxy Property

SOCKS proxy \(\-\-socks5\)\.

```csharp
public string? Socks5Proxy { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.SpeedLimit'></a>

## CurlOptions\.SpeedLimit Property

Speed limit in bytes per second \(\-\-limit\-rate\)\.

```csharp
public long SpeedLimit { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='CurlDotNet.Core.CurlOptions.SpeedTime'></a>

## CurlOptions\.SpeedTime Property

Speed time period for limit \(\-\-speed\-time\)\.

```csharp
public int SpeedTime { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlOptions.Url'></a>

## CurlOptions\.Url Property

The target URL\.

```csharp
public string Url { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.UserAgent'></a>

## CurlOptions\.UserAgent Property

User agent string \(\-A or \-\-user\-agent\)\.

```csharp
public string? UserAgent { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlOptions.UserAuth'></a>

## CurlOptions\.UserAuth Property

Alias for Credentials for test compatibility\.

```csharp
public System.Net.NetworkCredential UserAuth { get; set; }
```

#### Property Value
[System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')

<a name='CurlDotNet.Core.CurlOptions.UseRemoteFileName'></a>

## CurlOptions\.UseRemoteFileName Property

Whether to use remote filename for output \(\-O flag\)\.

```csharp
public bool UseRemoteFileName { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.Verbose'></a>

## CurlOptions\.Verbose Property

Verbose output \(\-v flag\)\.

```csharp
public bool Verbose { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlOptions.WriteOut'></a>

## CurlOptions\.WriteOut Property

Write\-out format string \(\-w or \-\-write\-out\)\.

```csharp
public string? WriteOut { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
### Methods

<a name='CurlDotNet.Core.CurlOptions.Clone()'></a>

## CurlOptions\.Clone\(\) Method

Clone this options object\.

```csharp
public CurlDotNet.Core.CurlOptions Clone();
```

#### Returns
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')