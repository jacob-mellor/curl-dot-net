#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ExecuteAsync Method

| Overloads | |
| :--- | :--- |
| [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)') | Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object instead of a command string, so values from variables are sent exactly as given with no shell quoting to get wrong\. |
| [ExecuteAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)') | Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object, like [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)'), and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled\. |
| [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') | Runs a curl command \(the same text you would type in a terminal\) and returns the response; this is the main method of CurlDotNet\. |
| [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Runs a curl command with timeouts, retries, extra headers, a progress callback or a cancellation token set in code through a [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object instead of in the command text\. |
| [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') | Runs a curl command that was copied from a specific shell \(bash, cmd\.exe, PowerShell or Invoke\-WebRequest\), reading its quotes, escapes, line continuations and variables the way that shell would\. |
| [ExecuteAsync\(string, CurlFlavor, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor, System\.Threading\.CancellationToken\)') | Runs a curl command copied from a specific shell, like [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)'), and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled\. |
| [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)') | Runs a curl command like [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled, for user "Cancel" buttons, request aborts in ASP\.NET Core and your own deadlines\. |

<a name='CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions)'></a>

## Curl\.ExecuteAsync\(CurlOptions\) Method

Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object instead of a command string, so values from
variables are sent exactly as given with no shell quoting to get wrong\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

The request to send; [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') must be set to an absolute URL such
             as `https://api.example.com/v1/items`\. The object may be modified while the request runs \(URL
             normalisation, defaults\), so do not share one instance between concurrent calls\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[options](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions).options 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)\.options') is null\. Fix: pass a `new CurlOptions { Url = ... }`\.
             See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/run\-curl\-command\-in\-csharp/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
[Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') is empty \(exit code 2, "no URL specified"\), or the transfer failed;
             the subtypes are listed on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. See
             https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
var options = new CurlOptions
{
    Url = "https://api.example.com/users",
    Method = "POST",
    Data = "{\"name\":\"O'Brien\"}",
    Headers = { ["Content-Type"] = "application/json" },
    FollowLocation = true
};

var result = await Curl.ExecuteAsync(options);
```

### Remarks

Nothing is parsed: [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data') containing quotes, spaces or backslashes is sent
             byte for byte. Set at least [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url'); every other property mirrors a curl option
             ([Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method') is `-X`, [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers') is `-H`,
             [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation') is `-L`, [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime') is `-m`...).
             The quick helpers [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)'), [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)') and [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)') are
             built on this method. The `Default*` properties of this class still apply when the options leave them
             unset.

To start from a command and adjust it, parse it with [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') and use
             `ParsedOptions`.

Example: `var result = await Curl.ExecuteAsync(new CurlOptions { Url = "https://api.example.com/users", Method = "POST", Data = "{\"name\":\"O'Brien\"}", Headers = { ["Content-Type"] = "application/json" } });`

Mapping of every option to libcurl: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/libcurl-mapping.md

### See Also
- [Blocking version](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.Execute\(CurlDotNet\.Core\.CurlOptions\)')
- [With cancellation](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)')
- [libcurl\-style equivalent](CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PerformAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)')
- [GetOptions\(\)](CurlDotNet.Core.CurlRequestBuilder.GetOptions().md 'CurlDotNet\.Core\.CurlRequestBuilder\.GetOptions\(\)')

<a name='CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken)'></a>

## Curl\.ExecuteAsync\(CurlOptions, CancellationToken\) Method

Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object, like [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)'),
and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Core.CurlOptions options, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

The request to send; [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') must be set\. Do not share one instance
            between concurrent calls\.

<a name='CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancels the request when signalled; `CancellationToken.None` for no
            cancellation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[options](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).options 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)\.options') is null\.

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The token was cancelled before the transfer finished\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
No URL \(exit code 2, like curl's "no URL specified"\) or the transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Remarks

Cancelling throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException'); a [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime') limit
            throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28) instead.

Example: `using var cts = new CancellationTokenSource(5000);` then
            `var result = await Curl.ExecuteAsync(new CurlOptions { Url = "https://api.example.com/health" }, cts.Token);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')
- [Execute\(CurlOptions\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.Execute\(CurlDotNet\.Core\.CurlOptions\)')
- [PerformAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PerformAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)')

<a name='CurlDotNet.Curl.ExecuteAsync(string)'></a>

## Curl\.ExecuteAsync\(string\) Method

Runs a curl command \(the same text you would type in a terminal\) and returns the response; this is the main
method of CurlDotNet\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, for example `curl -s https://api.example.com/users?page=2`
             or just `https://api.example.com`\. Must contain a URL\. Quote values with spaces as the source shell
             would \(`-H 'Accept: application/json'`\)\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'): [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') \(the HTTP status; for `file://` 200, or
             404 when the file does not exist; 0 for a transfer skipped by `--skip-existing`\), [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') \(standard output text\), [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')
             \(response headers, case\-insensitive\), [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') \(for non\-text responses\),
             [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles'), [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings') and [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string).command 'CurlDotNet\.Curl\.ExecuteAsync\(string\)\.command') is null, empty or only whitespace\. Fix: pass a command
             that contains a URL\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/run\-curl\-command\-in\-csharp/README\.md

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
A `--resolve` or `--connect-to` value curl cannot parse
             \([CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException'), exit code 49\), a command with several URL groups but no URL, or shell
             syntax the chosen [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') cannot read\. The message names the option\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
Thrown as the base type with exit code 2 for an option missing its value
             \(`-H` at the end\) or a value that is not a number \(`--max-filesize abc`\), and with exit code 3 for a
             command without any URL\. The message names the option and shows a valid value\. \(Invalid numbers for
             `-m`, `--connect-timeout`, `--max-redirs` and `--retry` are currently ignored rather than
             reported\.\) See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

[CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')  
A URL that cannot be parsed \(exit code 3\)\. Fix: check the
             scheme and host, percent\-encode spaces\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

[CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')  
A scheme CurlDotNet does not implement, such as
             `gopher://` \(exit code 1\)\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/supported\-protocols\.md

[CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')  
DNS could not resolve the host \(exit code 6\)\. Fix: check the
             host name; `--resolve host:443:1.2.3.4` skips DNS\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/dns\-errors\.md

[CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')  
Nothing accepted the connection on that host and port, or the
             proxy was unreachable \(exit code 7\)\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/dns\-errors\.md

[CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')  
`-m`/`--max-time`, `--connect-timeout` or
             [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds') expired \(exit code 28; thrown as [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException')\)\.
             Fix: raise the limit\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/timeout\-errors\.md

[CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')  
The TLS handshake failed \(exit code 35\) or the certificate is not trusted
             \(exit code 60, [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')\)\. Fix: trust the CA with `--cacert`; use
             `-k` for testing only\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/ssl\-errors\.md

[CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')  
Only with `-f`/`--fail` or `--fail-with-body`:
             the server answered 400 or above \(exit code 22\)\. [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') has the status\.
             See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/http\-errors\.md

[CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')  
With `-L`, more redirects than `--max-redirs` allows
             \(exit code 47\)\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/redirect\-errors\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
An output file \(`-o`, `-O`, `-D`, `-c`\) could
             not be written, for example because its folder does not exist \(exit code 23\)\. \(`--trace` creates its
             folder itself; an `--etag-save` folder that does not exist skips the transfer with a warning on standard error\.\) Fix: create the folder or add
             `--create-dirs`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

[CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')  
A file named with `@` \(`-d @body.json`, `-F file=@a.png`,
             `-T`\) could not be read \(exit code 26\); a `-d @file` that does not exist at all is
             [CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException') \(exit code 37\)\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
Any other curl failure; [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is curl's exit
             code and the message says what happened, how to fix it, and links the page for that code\. Full table:
             https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
// Copy any curl command from documentation and paste it here:
var response = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
Console.WriteLine($"Status: {response.StatusCode}");  // 200
var user = response.ParseJson<GitHubUser>();

// The exact command from Stripe's documentation:
var charge = await Curl.ExecuteAsync(@"
    curl https://api.stripe.com/v1/charges \
      -u sk_test_4eC39HqLyjWDarjtT1zdp7dc: \
      -d amount=2000 \
      -d currency=usd \
      -d source=tok_mastercard");

// Common options
await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://api.example.com");
await Curl.ExecuteAsync("curl -o download.pdf https://example.com/file.pdf");
await Curl.ExecuteAsync("curl -L --max-time 30 https://short.link/abc");
```

### Remarks

<b>The command.</b> Any curl command line, with or without the leading `curl` / `curl.exe`:
             `curl -X POST -H 'Content-Type: application/json' -d '{"a":1}' https://api.example.com`. Multi-line
             commands with `\`, `^` or backtick continuations work. It is read with [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
             (auto-detected shell by default); pass a [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') to
             [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') when you know where it came from. Several URLs, URL globs
             (`https://x/[1-3].json`) and `--next` run one after another like curl's transfer loop; the result
             describes the last transfer and [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') holds the output of all of them.

<b>What you get back.</b> HTTP error statuses are not exceptions: a 404 returns a result with
             [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') 404 (add `-f` to throw instead). [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') holds
             what curl would print on standard output (the body, plus headers with `-i`, plus `-w` text);
             binary responses are in [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'); files written with `-o`/`-O` are listed
             in [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles').

<b>Unknown options</b> are ignored, as a lenient curl would; call [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') first
             when the command comes from users and you want typos reported.

<b>Defaults</b> that apply when the command does not say otherwise: [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')
             (`-m`), [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds') (`--connect-timeout`),
             [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects') (`-L`), [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure') (`-k`) and
             [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent') (`-A`).

Thread-safe; many calls may run at once and share pooled connections.

Example: `var result = await Curl.ExecuteAsync("curl -u alice:secret https://api.example.com/me");`
             then `if (result.IsSuccess) Console.WriteLine(result.Body); else Console.WriteLine($"HTTP {result.StatusCode}");`.

Example with error handling: `try { await Curl.ExecuteAsync("curl -f https://api.example.com/x"); }
             catch (CurlHttpReturnedErrorException ex) { /* ex.StatusCode is 404 */ } catch (CurlException ex) { Console.WriteLine(ex.Message); /* what, fix, link */ }`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md
             - errors: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')
- [With cancellation](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')
- [Command from a specific shell](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')
- [With timeouts, retries and progress set in code](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')
- [Typed options, no command string](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')
- [Several commands in parallel](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)')
- [Check a command without sending it](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)')
- [Fluent builder equivalent](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [libcurl\-style equivalent](CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PerformAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)')
- [Same method under the DotNetCurl name](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)')
- [curl manual](https://curl.se/docs/manpage.html 'https://curl\.se/docs/manpage\.html')

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings)'></a>

## Curl\.ExecuteAsync\(string, CurlSettings\) Method

Runs a curl command with timeouts, retries, extra headers, a progress callback or a cancellation token set
in code through a [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object instead of in the command text\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

The settings to apply on top of the command, built with `new CurlSettings().With...()`
             or [FromDefaults\(\)](CurlDotNet.Core.CurlSettings.FromDefaults().md 'CurlDotNet\.Core\.CurlSettings\.FromDefaults\(\)')\. Null runs the command unchanged \(no cancellation\)\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings).command 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)\.command') is null, empty or whitespace\.

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
[CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken') was cancelled\. See
             https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/12\-cancellation\-tokens\.md

[CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')  
The [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress') callback threw \(exit
             code 42\); the callback's exception is the [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\.

[CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')  
The settings' \(or the command's\) time limit expired \(exit code 28\)\. See
             https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/timeout\-errors\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed after all retries, as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Example

```csharp
var settings = new CurlSettings()
    .WithTimeout(30)
    .WithRetries(count: 3, delayMs: 2000)
    .WithHeader("X-Request-Id", Guid.NewGuid().ToString())
    .WithProgress((percent, received, total) =>
        Console.WriteLine(total > 0 ? $"{percent:F1}%" : $"{received} bytes"));

var result = await Curl.ExecuteAsync("curl -O https://example.com/large-file.zip", settings);
```

### Remarks

<b>Settings that this method applies</b> (each overrides the same option in the command):
             [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds') (`-m`), [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds')
             (`--connect-timeout`), [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects') (`-L`),
             [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure') (`-k`), [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers') (one `-H` each;
             replaces a header of the same name), [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress') (called for every chunk of the
             response body with the percentage or -1 when the size is unknown, the bytes received so far, and the total
             size or -1), [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount') with [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs') (retries any
             failure, like `--retry N --retry-delay S --retry-all-errors`), [Flavor](CurlDotNet.Core.CurlSettings.Flavor.md 'CurlDotNet\.Core\.CurlSettings\.Flavor') (how the
             command text is read; default [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')) and [CancellationToken](CurlDotNet.Core.CurlSettings.CancellationToken.md 'CurlDotNet\.Core\.CurlSettings\.CancellationToken').

The other [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') properties (`Proxy`, `UserAgent`, `Cookies`,
             `OnRedirect`, `BufferSize`, `AutomaticDecompression`) are not used by this method; put
             `-x`, `-A`, `-b`/`-c` or `--compressed` in the command instead.

A progress callback that throws aborts the transfer with [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')
             (exit code 42). Cancelling the settings' token throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException').

Example: `var settings = new CurlSettings().WithTimeout(10).WithRetries(3, 500).WithProgress((pct, got, total) => Console.Write($"\r{pct:F0}%"));`
             then `var result = await Curl.ExecuteAsync("curl -o big.zip https://example.com/big.zip", settings);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)')
- [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')
- [Builder equivalent](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(CurlDotNet.Core.CurlSettings,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(CurlDotNet\.Core\.CurlSettings, System\.Threading\.CancellationToken\)')
- [CurlAsync\(string, CurlSettings\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, CurlDotNet\.Core\.CurlSettings\)')

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor)'></a>

## Curl\.ExecuteAsync\(string, CurlFlavor\) Method

Runs a curl command that was copied from a specific shell \(bash, cmd\.exe, PowerShell or
Invoke\-WebRequest\), reading its quotes, escapes, line continuations and variables the way that shell would\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, CurlDotNet.CurlFlavor flavor);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The command exactly as it was pasted, with or without the leading `curl`,
            `curl.exe` or \(for [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest')\) `Invoke-WebRequest`\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor).flavor'></a>

`flavor` [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')

Where the command came from: [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') for bash/zsh \(macOS, Linux, Git Bash, WSL\);
[WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd') for `curl.exe` in the Windows Command Prompt;
[PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') for `curl.exe` in PowerShell;
[InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest') for PowerShell's `Invoke-WebRequest` / `Invoke-RestMethod`
syntax \(also the `curl`, `iwr` and `irm` aliases\); [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') to detect it\.
Overrides [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') for this call\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') of the request, as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. To see
            which flavor and which parsed values a command produces before sending it, use
            [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)') and read [Flavor](CurlDotNet.Core.CurlOptions.Flavor.md 'CurlDotNet\.Core\.CurlOptions\.Flavor') on its `ParsedOptions`\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
The command is null, empty or only whitespace\. Fix: pass the pasted text\.

[System\.ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception 'System\.ArgumentOutOfRangeException')  
The flavor is not a defined [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The text breaks the flavor's rules \(for example an
            unterminated quote\), or uses an `Invoke-WebRequest` parameter that cannot be translated, such as
            `-Credential`\. The message says how to rewrite it\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-flavors\-bash\-cmd\-powershell/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The request itself fails, exactly as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

The same characters mean different bytes in different shells. With
            `-d '{\"a\":1}'`, [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') sends `{"a":1}` (the Windows C runtime turns
            `\"` into a quote) while [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') sends `{\"a\":1}` with the backslashes.
            With `"%TOKEN%"`, [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd') substitutes the environment variable and the
            other flavors do not.

Example: `var result = await Curl.ExecuteAsync("curl.exe -H 'Content-Type: application/json' -d '{\"a\":1}' https://api.example.com", CurlFlavor.PowerShell);`

Example: `var result = await Curl.ExecuteAsync("Invoke-RestMethod -Uri https://api.example.com -Method Post -Body '{\"a\":1}' -ContentType 'application/json'", CurlFlavor.InvokeWebRequest);`

Use [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)') to see what [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') would choose, and
            [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)') to check a command without sending it.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [Blocking version](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.CurlFlavor\)')
- [With cancellation](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor, System\.Threading\.CancellationToken\)')
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')
- [WithFlavor\(CurlFlavor\)](CurlDotNet.Core.CurlSettings.WithFlavor(CurlDotNet.CurlFlavor).md 'CurlDotNet\.Core\.CurlSettings\.WithFlavor\(CurlDotNet\.CurlFlavor\)')
- [curl flavors guide](https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md 'https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-flavors\-bash\-cmd\-powershell/README\.md')

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken)'></a>

## Curl\.ExecuteAsync\(string, CurlFlavor, CancellationToken\) Method

Runs a curl command copied from a specific shell, like [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)'), and
stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, CurlDotNet.CurlFlavor flavor, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The command exactly as it was pasted, for example
            `curl.exe -H "Accept: application/json" https://api.example.com`\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken).flavor'></a>

`flavor` [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')

The shell it was written for: [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix'), [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd'),
            [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell'), [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest') or [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto')
            \(see [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')\)\. Overrides [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') for this call\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancels the request when signalled; `CancellationToken.None` for no
            cancellation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The token was cancelled before the transfer finished\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/12\-cancellation\-tokens\.md

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken).command 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor, System\.Threading\.CancellationToken\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The text breaks the flavor's rules or uses an
            untranslatable `Invoke-WebRequest` parameter; the message says how to rewrite it\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-flavors\-bash\-cmd\-powershell/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed, as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Remarks

Example: `using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));` then
            `var result = await Curl.ExecuteAsync(pasted, CurlFlavor.WindowsCmd, cts.Token);`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')
- [Execute\(string, CurlFlavor\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.CurlFlavor\)')
- [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')

<a name='CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken)'></a>

## Curl\.ExecuteAsync\(string, CancellationToken\) Method

Runs a curl command like [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)\.cancellationToken')
is cancelled, for user "Cancel" buttons, request aborts in ASP\.NET Core and your own deadlines\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, exactly as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'),
             for example `curl -L https://example.com/file.zip -o file.zip`\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancels the request when signalled: from a
             `CancellationTokenSource` \(optionally with a timeout, `new CancellationTokenSource(TimeSpan.FromSeconds(30))`\),
             an ASP\.NET Core action parameter, or `CancellationToken.None` for no cancellation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The same [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') as [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The token was cancelled before the transfer finished\. Fix: nothing
             to fix if you cancelled on purpose; otherwise use a later deadline\. See
             https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/12\-cancellation\-tokens\.md

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).command 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)\.command') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; the subtypes and exit codes are listed on
             [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
private CancellationTokenSource _downloadCts;

async Task StartDownload()
{
    _downloadCts = new CancellationTokenSource();
    try
    {
        await Curl.ExecuteAsync("curl -o large-file.zip https://example.com/huge-file.zip", _downloadCts.Token);
        MessageBox.Show("Download complete!");
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("Download cancelled by user");
    }
}

void CancelButton_Click() => _downloadCts?.Cancel();
```

### Remarks

Cancelling the token stops the transfer at once (during connect, while waiting for headers or while
             reading the body) and throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') - the standard .NET contract, so
             `catch (OperationCanceledException)` works. A limit in the command itself (`-m 30`) or
             [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds') is different: it throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit
             code 28). Files already written by `-o` before cancellation may be incomplete.

Everything else (the command format, the result, the other exceptions) is the same as
             [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)').

Example: `using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));` then
             `var result = await Curl.ExecuteAsync("curl -o big.zip https://example.com/big.zip", cts.Token);` - after
             30 seconds it throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException').

Example in ASP.NET Core: `public async Task<IActionResult> Proxy(CancellationToken ct) => Ok((await Curl.ExecuteAsync("curl https://upstream.example.com/data", ct)).Body);`
             stops the upstream call when the browser disconnects.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [Blocking version](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)')
- [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
- [ExecuteAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)')
- [ExecuteAsync\(CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(System\.Threading\.CancellationToken\)')
- [CurlAsync\(string, CancellationToken\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)')