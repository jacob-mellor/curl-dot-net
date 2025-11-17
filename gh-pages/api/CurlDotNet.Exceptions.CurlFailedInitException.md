#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlFailedInitException Class

CURLE\_FAILED\_INIT \(2\) \- Failed to initialize

```csharp
public class CurlFailedInitException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFailedInitException
### Constructors

<a name='CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string)'></a>

## CurlFailedInitException\(string, string\) Constructor

Initializes a new instance of the [CurlFailedInitException](CurlDotNet.Exceptions.CurlFailedInitException.md 'CurlDotNet\.Exceptions\.CurlFailedInitException') class\.

```csharp
public CurlFailedInitException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the initialization failure\.

<a name='CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.