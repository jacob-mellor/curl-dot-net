#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')

## CurlMalformedUrlException Constructors

| Overloads | |
| :--- | :--- |
| [CurlMalformedUrlException\(string, string\)](CurlDotNet.Exceptions.CurlMalformedUrlException.#ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string) 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.CurlMalformedUrlException\(string, string\)') | Creates the error for a URL from the command that cannot be parsed, with advice on scheme, encoding and quoting\. |
| [CurlMalformedUrlException\(string, string, string\)](CurlDotNet.Exceptions.CurlMalformedUrlException.#ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string) 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.CurlMalformedUrlException\(string, string, string\)') | Creates the error with an explanation of what is wrong with the URL, for URLs that did not come from the command line \(such as a redirect's `Location` header\)\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string)'></a>

## CurlMalformedUrlException\(string, string\) Constructor

Creates the error for a URL from the command that cannot be parsed, with advice on scheme, encoding and
quoting\.

```csharp
public CurlMalformedUrlException(string url, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The malformed URL that caused the error, as given\.

<a name='CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred, or null\.

### Remarks

Message: `Malformed URL: URL`, the fix, and the URL-errors page.
            Example: `throw new CurlMalformedUrlException("http://exa mple.com/", command);`

### See Also
- [CurlMalformedUrlException\(string, string, string\)](CurlDotNet.Exceptions.CurlMalformedUrlException.#ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string) 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.CurlMalformedUrlException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string)'></a>

## CurlMalformedUrlException\(string, string, string\) Constructor

Creates the error with an explanation of what is wrong with the URL, for URLs that did not come from the
command line \(such as a redirect's `Location` header\)\.

```csharp
public CurlMalformedUrlException(string url, string detail, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The malformed URL, as received\.

<a name='CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What is wrong with it and, optionally, how to fix it\.

<a name='CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

The [detail](CurlDotNet.Exceptions.CurlMalformedUrlException.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string).detail 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.CurlMalformedUrlException\(string, string, string\)\.detail') explains the defect, for example "the port 99999 is out of range".

### See Also
- [CurlMalformedUrlException\(string, string\)](CurlDotNet.Exceptions.CurlMalformedUrlException.#ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string) 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.CurlMalformedUrlException\(string, string\)')