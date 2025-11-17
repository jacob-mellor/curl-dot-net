#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlHttpPostErrorException Class

CURLE\_HTTP\_POST\_ERROR \(34\) \- HTTP POST error

```csharp
public class CurlHttpPostErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlHttpPostErrorException
### Constructors

<a name='CurlDotNet.Exceptions.CurlHttpPostErrorException.CurlHttpPostErrorException(string,string)'></a>

## CurlHttpPostErrorException\(string, string\) Constructor

Initializes a new instance of CurlHttpPostErrorException\.

```csharp
public CurlHttpPostErrorException(string message, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpPostErrorException.CurlHttpPostErrorException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlHttpPostErrorException.CurlHttpPostErrorException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.