#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException')

## CurlOperationTimeoutException\(double, string\) Constructor

Initializes a new instance of the [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') class\.

```csharp
public CurlOperationTimeoutException(double timeoutSeconds, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).timeoutSeconds'></a>

`timeoutSeconds` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The timeout value in seconds that was exceeded\.

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.