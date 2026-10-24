#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException')

## CurlNotBuiltInException\(string, string\) Constructor

Creates the "feature not built in" error \(exit code 4\) for [feature](CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).md#CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).feature 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.CurlNotBuiltInException\(string, string\)\.feature'), with advice to remove it
or use the native curl tool\.

```csharp
public CurlNotBuiltInException(string feature, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).feature'></a>

`feature` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The feature that is not available in this build, such as an option name\.

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred, or null\.

### Remarks

Message: `Feature not available: FEATURE`. Example: `throw new CurlNotBuiltInException("--http3 on .NET Standard 2.0", command);`

### See Also
- [CurlNotSupportedException\(string, string\)](CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).md 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.CurlNotSupportedException\(string, string\)')