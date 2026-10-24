#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')

## CurlTooManyRedirectsException\(int, string\) Constructor

Initializes a new instance of the [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') class\.

```csharp
public CurlTooManyRedirectsException(int count, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of redirects that were attempted before failing\.

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.