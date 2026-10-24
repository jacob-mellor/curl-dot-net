#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslEngineSetFailedException](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException')

## CurlSslEngineSetFailedException\(string, string\) Constructor

Creates the error \(exit code 54\) for an engine that could not be made the default, prefixing
`Failed to set SSL engine:` to the reason\.

```csharp
public CurlSslEngineSetFailedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Why the engine could not be set; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string).md#CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string).message 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException\.CurlSslEngineSetFailedException\(string, string\)\.message') has no fix, the suggestion says to remove `--engine`.

### See Also
- [CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException')