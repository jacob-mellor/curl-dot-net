#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlExecutionException](CurlDotNet.Exceptions.CurlExecutionException.md 'CurlDotNet\.Exceptions\.CurlExecutionException')

## CurlExecutionException\(string, string, Exception\) Constructor

Creates a general execution failure that wraps the original exception and summarises it in the first line
of the message\.

```csharp
public CurlExecutionException(string message, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

<a name='CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying exception that caused the failure, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).md#CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).message 'CurlDotNet\.Exceptions\.CurlExecutionException\.CurlExecutionException\(string, string, System\.Exception\)\.message') has no fix, the suggestion tells the reader to look at the inner exception
            and to rerun the command with `-v`. Example: `throw new CurlExecutionException("Nightly sync failed.", command, ex);`

### See Also
- [CurlException\(string, int, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string, System\.Exception\)')