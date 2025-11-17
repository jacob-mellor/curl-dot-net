#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlHttpReturnedErrorException Class

CURLE\_HTTP\_RETURNED\_ERROR \(22\) \- HTTP returned error

```csharp
public class CurlHttpReturnedErrorException : CurlDotNet.Exceptions.CurlHttpException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') &#129106; CurlHttpReturnedErrorException
### Constructors

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string)'></a>

## CurlHttpReturnedErrorException\(int, string, string, string\) Constructor

Initializes a new instance of CurlHttpReturnedErrorException\.

```csharp
public CurlHttpReturnedErrorException(int statusCode, string statusText, string body, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).statusText'></a>

`statusText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The HTTP status text\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).body'></a>

`body` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.