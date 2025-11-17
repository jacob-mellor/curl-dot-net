#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')

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