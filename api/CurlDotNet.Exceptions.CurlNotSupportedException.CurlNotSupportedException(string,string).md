#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlNotSupportedException](CurlDotNet.Exceptions.CurlNotSupportedException.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException')

## CurlNotSupportedException\(string, string\) Constructor

Creates the "not supported" error \(exit code 1\) for [feature](CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).md#CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).feature 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.CurlNotSupportedException\(string, string\)\.feature'), with advice to remove it or use
the native curl tool\.

```csharp
public CurlNotSupportedException(string feature, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).feature'></a>

`feature` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the unsupported feature or option, such as `--libcurl`\.

<a name='CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

### Remarks

Message: `Feature not supported: FEATURE`, then the fix and a link to the compatibility list.
            Example: `throw new CurlNotSupportedException("--libcurl", command);`

### See Also
- [CurlNotBuiltInException\(string, string\)](CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.CurlNotBuiltInException\(string, string\)')