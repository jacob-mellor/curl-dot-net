#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')

## CurlUnsupportedProtocolException Constructors

| Overloads | |
| :--- | :--- |
| [CurlUnsupportedProtocolException\(string, string\)](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.#ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string) 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string\)') | Creates the error for a URL whose scheme is not supported, with curl's wording `Protocol "gopher" not supported` and advice to fix the scheme\. |
| [CurlUnsupportedProtocolException\(string, string, bool\)](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.#ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,bool) 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string, bool\)') | Creates the error for a redirect \(`Location:` header\) that points at a scheme curl refuses to follow, such as `file://` or `gopher://`\. |
| [CurlUnsupportedProtocolException\(string, string, string\)](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.#ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,string) 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string, string\)') | Creates the error for a server reply whose status line uses a protocol version curl does not speak \(HTTP/0\.9 without `--http0.9`, `HTTP/1.2`, or a malformed status code\)\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string)'></a>

## CurlUnsupportedProtocolException\(string, string\) Constructor

Creates the error for a URL whose scheme is not supported, with curl's wording
`Protocol "gopher" not supported` and advice to fix the scheme\.

```csharp
public CurlUnsupportedProtocolException(string protocol, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string).protocol'></a>

`protocol` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The scheme from the URL that is not supported, such as `gopher`\.

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred, or null\.

### Remarks

Thrown before connecting. Example: `curl gopher://example.com/` or a typo like `htps://`.
            The fix lists the schemes CurlDotNet supports.

### See Also
- [CurlUnsupportedProtocolException\(string, string, bool\)](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.#ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,bool) 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string, bool\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,bool)'></a>

## CurlUnsupportedProtocolException\(string, string, bool\) Constructor

Creates the error for a redirect \(`Location:` header\) that points at a scheme curl refuses to follow,
such as `file://` or `gopher://`\.

```csharp
public CurlUnsupportedProtocolException(string protocol, string? command, bool redirect);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,bool).protocol'></a>

`protocol` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The scheme of the redirect target, such as `gopher`\.

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,bool).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,bool).redirect'></a>

`redirect` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

Must be true; distinguishes this overload from the others\.

### Remarks

With `-L`, curl only follows redirects to `http://` and `https://`; a server sending
            `Location: gopher://...` or `file:///etc/passwd` ends the transfer with exit code 1 instead of
            silently switching protocols. The fix says to fetch the target explicitly if you trust it.

### See Also
- [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,string)'></a>

## CurlUnsupportedProtocolException\(string, string, string\) Constructor

Creates the error for a server reply whose status line uses a protocol version curl does not speak
\(HTTP/0\.9 without `--http0.9`, `HTTP/1.2`, or a malformed status code\)\.

```csharp
public CurlUnsupportedProtocolException(string protocol, string detail, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,string).protocol'></a>

`protocol` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the server announced, such as `HTTP/1.2` or `HTTP/0.9`\.

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Explanation \(and optionally a fix\), already worded for the reader\.

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

The [detail](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,string).detail 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string, string\)\.detail') text comes from the response validator and already explains the problem;
            the fix points at `--http0.9` and at checking the port.

### See Also
- [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')