#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlExecutionException Class

Thrown when execution fails for general reasons

```csharp
public class CurlExecutionException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlExecutionException
### Constructors

<a name='CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception)'></a>

## CurlExecutionException\(string, string, Exception\) Constructor

Initializes a new instance of the [CurlExecutionException](CurlDotNet.Exceptions.CurlExecutionException.md 'CurlDotNet\.Exceptions\.CurlExecutionException') class

```csharp
public CurlExecutionException(string message, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the execution failure

<a name='CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed

<a name='CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying exception that caused the failure