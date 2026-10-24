#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFailedInitException](CurlDotNet.Exceptions.CurlFailedInitException.md 'CurlDotNet\.Exceptions\.CurlFailedInitException')

## CurlFailedInitException\(string, string\) Constructor

Creates an initialisation failure \(exit code 2\) with a message naming what could not be prepared before the
transfer started\.

```csharp
public CurlFailedInitException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What could not be initialised; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string).md#CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string).message 'CurlDotNet\.Exceptions\.CurlFailedInitException\.CurlFailedInitException\(string, string\)\.message') has no fix, the suggestion tells the reader to check the options that need
            local resources. Example: `throw new CurlFailedInitException("Client certificate store could not be opened.", command);`

### See Also
- [CurlBadFunctionArgumentException](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException')