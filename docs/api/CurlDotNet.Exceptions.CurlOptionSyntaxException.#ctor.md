#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')

## CurlOptionSyntaxException Constructors

| Overloads | |
| :--- | :--- |
| [CurlOptionSyntaxException\(string, string\)](CurlDotNet.Exceptions.CurlOptionSyntaxException.#ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string) 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string\)') | Creates a syntax error for [option](CurlDotNet.Exceptions.CurlOptionSyntaxException.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).option 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string\)\.option') without an exit code, with advice to check the value's format in curl's manual\. |
| [CurlOptionSyntaxException\(string, string, string\)](CurlDotNet.Exceptions.CurlOptionSyntaxException.#ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string,string) 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string, string\)') | Creates the error for an option value curl refuses to parse \(exit code 49\), such as `--resolve` or `--connect-to` entries with a port that is not a number\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string)'></a>

## CurlOptionSyntaxException\(string, string\) Constructor

Creates a syntax error for [option](CurlDotNet.Exceptions.CurlOptionSyntaxException.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).option 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string\)\.option') without an exit code, with advice to check the value's
format in curl's manual\.

```csharp
public CurlOptionSyntaxException(string option, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).option'></a>

`option` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The option whose value has the wrong syntax, such as `--resolve`\.

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Option syntax error: OPTION`.

### See Also
- [CurlOptionSyntaxException\(string, string, string\)](CurlDotNet.Exceptions.CurlOptionSyntaxException.#ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string,string) 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string,string)'></a>

## CurlOptionSyntaxException\(string, string, string\) Constructor

Creates the error for an option value curl refuses to parse \(exit code 49\), such as `--resolve` or
`--connect-to` entries with a port that is not a number\.

```csharp
public CurlOptionSyntaxException(string option, string message, string command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string,string).option'></a>

`option` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The option whose value is malformed \(for example `--resolve`\)\.

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What is wrong with the value and how to write it correctly\.

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Usage: `curl --resolve example.com:80:127.0.0.1 http://example.com/` is valid; swapping the
            host and the port (`--resolve 80:example.com:127.0.0.1`) throws this exception with
            [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') 49, exactly where the curl tool exits with 49.

### See Also
- [CurlOptionSyntaxException\(string, string\)](CurlDotNet.Exceptions.CurlOptionSyntaxException.#ctor.md#CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string) 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException\.CurlOptionSyntaxException\(string, string\)')