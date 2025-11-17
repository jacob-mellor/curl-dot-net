#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlSslEngineSetFailedException Class

CURLE\_SSL\_ENGINE\_SETFAILED \(54\) \- Failed setting SSL engine

```csharp
public class CurlSslEngineSetFailedException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslEngineSetFailedException
### Constructors

<a name='CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string)'></a>

## CurlSslEngineSetFailedException\(string, string\) Constructor

Initializes a new instance of the [CurlSslEngineSetFailedException](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException') class\.

```csharp
public CurlSslEngineSetFailedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.