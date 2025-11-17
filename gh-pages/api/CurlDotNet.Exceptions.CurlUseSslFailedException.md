#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlUseSslFailedException Class

CURLE\_USE\_SSL\_FAILED \(64\) \- Required SSL level failed

```csharp
public class CurlUseSslFailedException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlUseSslFailedException
### Constructors

<a name='CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string)'></a>

## CurlUseSslFailedException\(string, string\) Constructor

Initializes a new instance of the [CurlUseSslFailedException](CurlDotNet.Exceptions.CurlUseSslFailedException.md 'CurlDotNet\.Exceptions\.CurlUseSslFailedException') class\.

```csharp
public CurlUseSslFailedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.