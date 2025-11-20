#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlOptionSyntaxException Class

CURLE\_SETOPT\_OPTION\_SYNTAX \(49\) \- Option syntax error

```csharp
public class CurlOptionSyntaxException : CurlDotNet.Exceptions.CurlInvalidCommandException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException') &#129106; CurlOptionSyntaxException
### Constructors

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string)'></a>

## CurlOptionSyntaxException\(string, string\) Constructor

Initializes a new instance of the [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException') class\.

```csharp
public CurlOptionSyntaxException(string option, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).option'></a>

`option` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The option that caused the syntax error\.

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.