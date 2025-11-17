#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlGotNothingException Class

CURLE\_GOT\_NOTHING \(52\) \- Got nothing \(empty reply\)

```csharp
public class CurlGotNothingException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlGotNothingException
### Constructors

<a name='CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string)'></a>

## CurlGotNothingException\(string\) Constructor

Initializes a new instance of the [CurlGotNothingException](CurlDotNet.Exceptions.CurlGotNothingException.md 'CurlDotNet\.Exceptions\.CurlGotNothingException') class\.

```csharp
public CurlGotNothingException(string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.