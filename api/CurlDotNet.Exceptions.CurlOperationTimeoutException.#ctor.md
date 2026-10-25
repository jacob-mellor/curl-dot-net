#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException')

## CurlOperationTimeoutException Constructors

| Overloads | |
| :--- | :--- |
| [CurlOperationTimeoutException\(double, string\)](CurlDotNet.Exceptions.CurlOperationTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string) 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string\)') | Creates the timeout error with curl's wording `Operation timed out after N milliseconds` for the limit [timeoutSeconds](CurlDotNet.Exceptions.CurlOperationTimeoutException.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).timeoutSeconds 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string\)\.timeoutSeconds')\. |
| [CurlOperationTimeoutException\(double, string, string\)](CurlDotNet.Exceptions.CurlOperationTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string) 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string, string\)') | Creates the timeout error with a message describing which limit expired and what had been received, such as curl's `Connection timed out after 2000 milliseconds`\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string)'></a>

## CurlOperationTimeoutException\(double, string\) Constructor

Creates the timeout error with curl's wording `Operation timed out after N milliseconds` for the limit
[timeoutSeconds](CurlDotNet.Exceptions.CurlOperationTimeoutException.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).timeoutSeconds 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string\)\.timeoutSeconds')\.

```csharp
public CurlOperationTimeoutException(double timeoutSeconds, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).timeoutSeconds'></a>

`timeoutSeconds` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The timeout value in seconds that was exceeded \(decimals allowed\)\.

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

The suggestion names the limit and tells the reader to raise `-m` or `--connect-timeout`.

### See Also
- [CurlOperationTimeoutException\(double, string, string\)](CurlDotNet.Exceptions.CurlOperationTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string) 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string)'></a>

## CurlOperationTimeoutException\(double, string, string\) Constructor

Creates the timeout error with a message describing which limit expired and what had been received, such as
curl's `Connection timed out after 2000 milliseconds`\.

```csharp
public CurlOperationTimeoutException(double timeoutSeconds, string message, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string).timeoutSeconds'></a>

`timeoutSeconds` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The limit that expired \(`--connect-timeout` or `-m/--max-time`\), 0 when none was set\.

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened and, optionally, how to fix it \(newline\-separated\)\.

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlOperationTimeoutException.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string).message 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string, string\)\.message') has no fix, the suggestion names the limit.

### See Also
- [CurlOperationTimeoutException\(double, string\)](CurlDotNet.Exceptions.CurlOperationTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string) 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string\)')