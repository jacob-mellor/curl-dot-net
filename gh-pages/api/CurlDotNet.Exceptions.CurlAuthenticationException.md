#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlAuthenticationException Class

Thrown when authentication fails

```csharp
public class CurlAuthenticationException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlAuthenticationException

Derived  
&#8627; [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')
### Constructors

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string)'></a>

## CurlAuthenticationException\(string, string, string\) Constructor

Initializes a new instance of the CurlAuthenticationException class\.

```csharp
public CurlAuthenticationException(string message, string authMethod=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).authMethod'></a>

`authMethod` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The authentication method that failed\.

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the error\.
### Properties

<a name='CurlDotNet.Exceptions.CurlAuthenticationException.AuthMethod'></a>

## CurlAuthenticationException\.AuthMethod Property

Gets the authentication method that failed\.

```csharp
public string AuthMethod { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')