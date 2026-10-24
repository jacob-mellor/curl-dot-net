#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlGotNothingException](CurlDotNet.Exceptions.CurlGotNothingException.md 'CurlDotNet\.Exceptions\.CurlGotNothingException')

## CurlGotNothingException Constructors

| Overloads | |
| :--- | :--- |
| [CurlGotNothingException\(string\)](CurlDotNet.Exceptions.CurlGotNothingException.#ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string) 'CurlDotNet\.Exceptions\.CurlGotNothingException\.CurlGotNothingException\(string\)') | Creates the empty\-reply error \(exit code 52\) with curl's wording `Empty reply from server` and advice on ports, TLS and server logs\. |
| [CurlGotNothingException\(string, string, string\)](CurlDotNet.Exceptions.CurlGotNothingException.#ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string) 'CurlDotNet\.Exceptions\.CurlGotNothingException\.CurlGotNothingException\(string, string, string\)') | Creates the empty\-reply error naming the server that closed the connection without replying, plus any extra context such as the request that was sent\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string)'></a>

## CurlGotNothingException\(string\) Constructor

Creates the empty\-reply error \(exit code 52\) with curl's wording `Empty reply from server` and advice on
ports, TLS and server logs\.

```csharp
public CurlGotNothingException(string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Used when the host is not known; the transport normally uses the overload that names it.

### See Also
- [CurlGotNothingException\(string, string, string\)](CurlDotNet.Exceptions.CurlGotNothingException.#ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string) 'CurlDotNet\.Exceptions\.CurlGotNothingException\.CurlGotNothingException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string)'></a>

## CurlGotNothingException\(string, string, string\) Constructor

Creates the empty\-reply error naming the server that closed the connection without replying, plus any extra
context such as the request that was sent\.

```csharp
public CurlGotNothingException(string host, string? command, string detail);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The server \(or proxy\) that accepted the request and closed the connection\.

<a name='CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

<a name='CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Extra context, such as the request that was sent; may be empty\.

### Remarks

Thrown by the transport; [detail](CurlDotNet.Exceptions.CurlGotNothingException.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string).detail 'CurlDotNet\.Exceptions\.CurlGotNothingException\.CurlGotNothingException\(string, string, string\)\.detail') may be empty.

### See Also
- [CurlGotNothingException\(string\)](CurlDotNet.Exceptions.CurlGotNothingException.#ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string) 'CurlDotNet\.Exceptions\.CurlGotNothingException\.CurlGotNothingException\(string\)')