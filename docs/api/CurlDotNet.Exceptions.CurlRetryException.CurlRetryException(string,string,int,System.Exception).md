#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')

## CurlRetryException\(string, string, int, Exception\) Constructor

Initializes a new instance of the [CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException') class

```csharp
public CurlRetryException(string message, string command, int retryCount, System.Exception lastException);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the retry failure

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).retryCount'></a>

`retryCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of retry attempts that were made

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).lastException'></a>

`lastException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The exception from the final retry attempt