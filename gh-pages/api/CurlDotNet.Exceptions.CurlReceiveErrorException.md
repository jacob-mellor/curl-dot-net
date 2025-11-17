#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlReceiveErrorException Class

CURLE\_RECV\_ERROR \(56\) \- Receive error

```csharp
public class CurlReceiveErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlReceiveErrorException
### Constructors

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string)'></a>

## CurlReceiveErrorException\(string, string\) Constructor

Initializes a new instance of CurlReceiveErrorException\.

```csharp
public CurlReceiveErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.