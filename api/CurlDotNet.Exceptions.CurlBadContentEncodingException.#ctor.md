#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException')

## CurlBadContentEncodingException Constructors

| Overloads | |
| :--- | :--- |
| [CurlBadContentEncodingException\(string, string\)](CurlDotNet.Exceptions.CurlBadContentEncodingException.#ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string) 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.CurlBadContentEncodingException\(string, string\)') | Creates the error \(exit code 61\) for a body whose Content\-Encoding could not be decoded, with advice to drop `--compressed`\. |
| [CurlBadContentEncodingException\(string, string, string\)](CurlDotNet.Exceptions.CurlBadContentEncodingException.#ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string,string) 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.CurlBadContentEncodingException\(string, string, string\)') | Creates the error explaining why the server's Transfer\-Encoding or Content\-Encoding was refused, for framing that no client option can accept\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string)'></a>

## CurlBadContentEncodingException\(string, string\) Constructor

Creates the error \(exit code 61\) for a body whose Content\-Encoding could not be decoded, with advice to drop
`--compressed`\.

```csharp
public CurlBadContentEncodingException(string encoding, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string).encoding'></a>

`encoding` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The content encoding that was not recognized or could not be decoded\.

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Bad content encoding: ENCODING`.

### See Also
- [CurlBadContentEncodingException\(string, string, string\)](CurlDotNet.Exceptions.CurlBadContentEncodingException.#ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string,string) 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.CurlBadContentEncodingException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string,string)'></a>

## CurlBadContentEncodingException\(string, string, string\) Constructor

Creates the error explaining why the server's Transfer\-Encoding or Content\-Encoding was refused, for framing
that no client option can accept\.

```csharp
public CurlBadContentEncodingException(string encoding, string detail, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string,string).encoding'></a>

`encoding` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The encoding header value the server sent\.

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What is wrong with it, in words\.

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

The suggestion says this is a server bug (or an attack on HTTP framing).

### See Also
- [CurlBadContentEncodingException\(string, string\)](CurlDotNet.Exceptions.CurlBadContentEncodingException.#ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string) 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.CurlBadContentEncodingException\(string, string\)')