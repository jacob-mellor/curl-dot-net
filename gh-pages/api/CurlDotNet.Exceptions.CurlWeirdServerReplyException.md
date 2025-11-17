#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlWeirdServerReplyException Class

CURLE\_WEIRD\_SERVER\_REPLY \(8\) \- Weird server reply

```csharp
public class CurlWeirdServerReplyException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlWeirdServerReplyException
### Constructors

<a name='CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string)'></a>

## CurlWeirdServerReplyException\(string, string\) Constructor

Initializes a new instance of the [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException') class\.

```csharp
public CurlWeirdServerReplyException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.