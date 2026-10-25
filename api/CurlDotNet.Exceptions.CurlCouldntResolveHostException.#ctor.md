#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')

## CurlCouldntResolveHostException Constructors

| Overloads | |
| :--- | :--- |
| [CurlCouldntResolveHostException\(string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveHostException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.CurlCouldntResolveHostException\(string, string\)') | Creates the error with curl's wording `Could not resolve host: NAME` and advice on spelling, DNS and `--resolve`\. |
| [CurlCouldntResolveHostException\(string, string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveHostException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.CurlCouldntResolveHostException\(string, string, string\)') | Creates the error with the resolver's explanation of why the name failed, such as "No such host is known"\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string)'></a>

## CurlCouldntResolveHostException\(string, string\) Constructor

Creates the error with curl's wording `Could not resolve host: NAME` and advice on spelling, DNS and
`--resolve`\.

```csharp
public CurlCouldntResolveHostException(string hostname, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string).hostname'></a>

`hostname` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The hostname that could not be resolved\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred, or null\.

### Remarks

Example: `throw new CurlCouldntResolveHostException("api.exmaple.com", command);`

### See Also
- [CurlCouldntResolveHostException\(string, string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveHostException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.CurlCouldntResolveHostException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string)'></a>

## CurlCouldntResolveHostException\(string, string, string\) Constructor

Creates the error with the resolver's explanation of why the name failed, such as "No such host is known"\.

```csharp
public CurlCouldntResolveHostException(string hostname, string detail, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string).hostname'></a>

`hostname` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host name from the URL\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the resolver reported, in its own words\.

<a name='CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

Thrown by the transport; the resolver's text is shown in parentheses after the host name.

### See Also
- [CurlCouldntResolveHostException\(string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveHostException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.CurlCouldntResolveHostException\(string, string\)')