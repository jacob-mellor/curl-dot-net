#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException')

## CurlSslEngineNotFoundException\(string, string\) Constructor

Creates the error \(exit code 53\) for an `--engine` that does not exist, with advice to remove the option\.

```csharp
public CurlSslEngineNotFoundException(string engine, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string).engine'></a>

`engine` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The SSL engine that was not found, as given with `--engine`\.

<a name='CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `SSL engine not found: NAME`.

### See Also
- [CurlSslEngineSetFailedException](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException')