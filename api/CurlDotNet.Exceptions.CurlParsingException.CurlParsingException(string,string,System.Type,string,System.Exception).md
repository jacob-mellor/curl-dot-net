#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')

## CurlParsingException\(string, string, Type, string, Exception\) Constructor

Creates a parsing error naming the response's Content\-Type and the type you asked for, with the parser's own
exception kept as the inner exception\.

```csharp
public CurlParsingException(string message, string contentType, System.Type expectedType, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What could not be parsed, naming the URL; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).contentType'></a>

`contentType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response's Content\-Type header value, or null\.

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).expectedType'></a>

`expectedType` [System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')

The type the body was supposed to be converted to, or null\.

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The JSON parser's exception, kept as InnerException; may be null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).md#CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).message 'CurlDotNet\.Exceptions\.CurlParsingException\.CurlParsingException\(string, string, System\.Type, string, System\.Exception\)\.message') has no fix, the suggestion tells the reader to check the status code and the
            body. Example: `throw new CurlParsingException("Body of GET /users is not JSON.", result.GetHeader("Content-Type"), typeof(User[]), result.Command, jsonException);`

### See Also
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')