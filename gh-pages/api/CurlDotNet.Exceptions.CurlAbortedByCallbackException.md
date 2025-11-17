#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlAbortedByCallbackException Class

CURLE\_ABORTED\_BY\_CALLBACK \(42\) \- Aborted by callback

```csharp
public class CurlAbortedByCallbackException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlAbortedByCallbackException
### Constructors

<a name='CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string)'></a>

## CurlAbortedByCallbackException\(string\) Constructor

Initializes a new instance of the [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException') class\.

```csharp
public CurlAbortedByCallbackException(string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.