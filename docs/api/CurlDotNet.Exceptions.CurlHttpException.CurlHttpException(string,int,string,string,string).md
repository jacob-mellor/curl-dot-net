#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\(string, int, string, string, string\) Constructor

Initializes a new instance of the CurlHttpException class\.

```csharp
public CurlHttpException(string message, int statusCode, string statusText=null, string responseBody=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).statusText'></a>

`statusText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The HTTP status text\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).responseBody'></a>

`responseBody` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body content\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the error\.