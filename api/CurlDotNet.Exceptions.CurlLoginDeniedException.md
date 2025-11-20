#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlLoginDeniedException Class

CURLE\_LOGIN\_DENIED \(67\) \- Login denied

```csharp
public class CurlLoginDeniedException : CurlDotNet.Exceptions.CurlAuthenticationException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException') &#129106; CurlLoginDeniedException
### Constructors

<a name='CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string)'></a>

## CurlLoginDeniedException\(string, string\) Constructor

Initializes a new instance of the [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException') class\.

```csharp
public CurlLoginDeniedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.