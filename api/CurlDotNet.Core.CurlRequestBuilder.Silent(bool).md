#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Silent\(bool\) Method

Turns on curl's silent mode \(`-s` / `--silent`\): no progress meter and no warnings written to
standard error\. The result and any exception are unchanged\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Silent(bool silent=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Silent(bool).silent'></a>

`silent` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True \(the default\) for silent mode\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Useful in console tools where CurlDotNet would otherwise write curl-style warnings to the console
            (for example "Failed creating file for saving etags"). Errors are still thrown as exceptions.

Example: `CurlRequestBuilder.Get(url).Silent()` is `curl -s URL`.

### See Also
- [Verbose\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Verbose(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Verbose\(bool\)')
- [Silent](CurlDotNet.Core.CurlOptions.Silent.md 'CurlDotNet\.Core\.CurlOptions\.Silent')