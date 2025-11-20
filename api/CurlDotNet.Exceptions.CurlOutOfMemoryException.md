#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlOutOfMemoryException Class

CURLE\_OUT\_OF\_MEMORY \(27\) \- Out of memory

```csharp
public class CurlOutOfMemoryException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlOutOfMemoryException
### Constructors

<a name='CurlDotNet.Exceptions.CurlOutOfMemoryException.CurlOutOfMemoryException(string)'></a>

## CurlOutOfMemoryException\(string\) Constructor

Initializes a new instance of the [CurlOutOfMemoryException](CurlDotNet.Exceptions.CurlOutOfMemoryException.md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException') class\.

```csharp
public CurlOutOfMemoryException(string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOutOfMemoryException.CurlOutOfMemoryException(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.