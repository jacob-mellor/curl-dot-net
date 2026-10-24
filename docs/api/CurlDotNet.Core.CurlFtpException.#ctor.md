#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFtpException](CurlDotNet.Core.CurlFtpException.md 'CurlDotNet\.Core\.CurlFtpException')

## CurlFtpException Constructors

| Overloads | |
| :--- | :--- |
| [CurlFtpException\(string, int\)](CurlDotNet.Core.CurlFtpException.#ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int) 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int\)') | Creates the exception for an FTP error reply; the message should say what failed and how to fix it\. |
| [CurlFtpException\(string, int, string\)](CurlDotNet.Core.CurlFtpException.#ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int,string) 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int, string\)') | Creates the exception for an FTP error reply and records the curl command that was running, so [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') shows what to retry\. |

<a name='ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int)'></a>

## CurlFtpException\(string, int\) Constructor

Creates the exception for an FTP error reply; the message should say what failed and how to fix it\.

```csharp
public CurlFtpException(string message, int ftpStatusCode);
```
#### Parameters

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened, then \(optionally, on new lines\) how to fix it and a
            `See https://...` link\.

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int).ftpStatusCode'></a>

`ftpStatusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The FTP reply code from the server, such as 530 or 550\.

### Remarks

The exit code ([CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')) is always 9. When
            [message](CurlDotNet.Core.CurlFtpException.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int).message 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int\)\.message') has no fix line or link, the generic advice and documentation page for
            exit code 9 are added, so [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') is never empty.

Example: `throw new CurlFtpException("The FTP server answered 550 for ftp://example.com/x.csv\nCheck the path.", 550);`

### See Also
- [CurlFtpException\(string, int, string\)](CurlDotNet.Core.CurlFtpException.#ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int,string) 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int, string\)')

<a name='ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int,string)'></a>

## CurlFtpException\(string, int, string\) Constructor

Creates the exception for an FTP error reply and records the curl command that was running,
so [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') shows what to retry\.

```csharp
public CurlFtpException(string message, int ftpStatusCode, string? command);
```
#### Parameters

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened, then how to fix it and a `See https://...` link, on separate lines\.

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int,string).ftpStatusCode'></a>

`ftpStatusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The FTP reply code from the server, such as 530 or 550\.

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

Same as [CurlFtpException\(string, int\)](CurlDotNet.Core.CurlFtpException.#ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int) 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int\)') plus the command. The command is kept as
            given (it may contain `-u user:password`), so log [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message') rather than
            [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') when credentials must not reach the logs.

Example: `throw new CurlFtpException(message, 530, "curl -u bob:x ftp://example.com/");`

### See Also
- [CurlFtpException\(string, int\)](CurlDotNet.Core.CurlFtpException.#ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int) 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int\)')