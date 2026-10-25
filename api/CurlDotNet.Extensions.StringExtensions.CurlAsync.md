#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.CurlAsync Method

| Overloads | |
| :--- | :--- |
| [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)') | Runs this string as a curl command line \(with or without the leading `curl`\) and returns the result \- the same as [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. |
| [CurlAsync\(this string, CancellationToken\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)') | Runs this string as a curl command line and stops the transfer when [cancellationToken](CurlDotNet.Extensions.StringExtensions.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled \- the same as [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\. |

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring)'></a>

## StringExtensions\.CurlAsync\(this string\) Method

Runs this string as a curl command line \(with or without the leading `curl`\) and returns the result \- the
same as [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(this string command);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

A curl command line such as `curl -L https://example.com`, as you would type it in a shell\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') \(status, body, headers, files written\)\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The command is invalid \([CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')\) or the transfer
            failed \(DNS, connect, timeout, TLS, `-f` HTTP error\); every subclass message says what happened and how to fix it\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Any curl option works: `-X`, `-H`, `-d`, `-u`, `-L`, `-o`... The string is read with
            [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') shell rules (bash, cmd or PowerShell quoting, detected automatically).
            HTTP error statuses are returned in the result; use `-f` to turn them into exceptions.

Example: `var result = await "curl -s -H 'Accept: application/json' https://api.github.com/users/octocat".CurlAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

### See Also
- [CurlAsync\(this string, CancellationToken\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)')
- [Curl\(this string\)](CurlDotNet.Extensions.StringExtensions.Curl(thisstring).md 'CurlDotNet\.Extensions\.StringExtensions\.Curl\(this string\)')
- [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken)'></a>

## StringExtensions\.CurlAsync\(this string, CancellationToken\) Method

Runs this string as a curl command line and stops the transfer when [cancellationToken](CurlDotNet.Extensions.StringExtensions.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)\.cancellationToken') is
cancelled \- the same as [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(this string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

A curl command line such as `curl -X POST -d a=1 https://example.com`\.

<a name='CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Token that aborts the transfer, for example `HttpContext.RequestAborted`\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task with the [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
[cancellationToken](CurlDotNet.Extensions.StringExtensions.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string, System\.Threading\.CancellationToken\)\.cancellationToken') was cancelled before the transfer finished\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The command is invalid or the transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Cancellation throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException'). For a curl-style time limit (exit code 28)
            put `-m SECONDS` in the command instead.

Example: `using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); var r = await "curl https://slow.example.com".CurlAsync(cts.Token);`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)')
- [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')