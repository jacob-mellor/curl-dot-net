#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.FilterLines\(Func\<string,bool\>\) Method

Keeps only the lines of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') for which [predicate](CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).md#CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).predicate 'CurlDotNet\.Core\.CurlResult\.FilterLines\(System\.Func\<string,bool\>\)\.predicate') returns true \(this changes
[Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') in place\) and returns this result\.

```csharp
public CurlDotNet.Core.CurlResult FilterLines(System.Func<string,bool> predicate);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Called once per line; return true to keep it\. Must not be null\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result \(with the filtered [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\), for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[predicate](CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).md#CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).predicate 'CurlDotNet\.Core\.CurlResult\.FilterLines\(System\.Func\<string,bool\>\)\.predicate') is null; pass a function such as `l => l.Length > 0`\.

### Remarks

Lines are split on `\n` and joined back with `\n`; a `\r` before the newline stays part of the
            line. Does nothing when [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') is null. [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') is untouched.

Example: `result.FilterLines(line => line.StartsWith("data:"))` keeps the data lines of a server-sent-events response.

### See Also
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')