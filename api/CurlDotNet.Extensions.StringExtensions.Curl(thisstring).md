#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.Curl\(this string\) Method

Executes curl synchronously \(blocking\)\.

```csharp
public static CurlDotNet.Core.CurlResult Curl(this string command);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.Curl(thisstring).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command string

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The curl result

### Example
var result = "curl https://api\.github\.com"\.Curl\(\);