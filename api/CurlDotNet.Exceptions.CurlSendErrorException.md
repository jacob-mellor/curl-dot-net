#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlSendErrorException Class

CURLE\_SEND\_ERROR \(55\) \- Send error

```csharp
public class CurlSendErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlSendErrorException
### Constructors

<a name='CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string)'></a>

## CurlSendErrorException\(string, string\) Constructor

Initializes a new instance of CurlSendErrorException\.

```csharp
public CurlSendErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.