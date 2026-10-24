#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')

## CurlRetryException\(string, string, int, Exception\) Constructor

Creates the "retries exhausted" error with the number of attempts and the last attempt's exception, which
becomes the inner exception and is summarised in the first line\.

```csharp
public CurlRetryException(string message, string command, int retryCount, System.Exception lastException);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed, such as `Failed after 3 retries`; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).retryCount'></a>

`retryCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of retry attempts that were made\.

<a name='CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).lastException'></a>

`lastException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The exception from the final attempt, kept as InnerException; may be null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).md#CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).message 'CurlDotNet\.Exceptions\.CurlRetryException\.CurlRetryException\(string, string, int, System\.Exception\)\.message') has no fix, the suggestion tells the reader to fix the last error's cause
            before adding retries. Example: `throw new CurlRetryException($"Failed after {max} retries", command, max, last);`

### See Also
- [LastAttemptException](CurlDotNet.Exceptions.CurlRetryException.LastAttemptException.md 'CurlDotNet\.Exceptions\.CurlRetryException\.LastAttemptException')