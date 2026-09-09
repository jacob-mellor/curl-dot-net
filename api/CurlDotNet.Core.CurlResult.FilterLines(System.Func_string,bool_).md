#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.FilterLines\(Func\<string,bool\>\) Method


<b>Extract lines that match a condition.</b>

Filter text responses:

```csharp
// Keep only error lines
result.FilterLines(line => line.Contains("ERROR"));

// Remove empty lines
result.FilterLines(line => !string.IsNullOrWhiteSpace(line));

// Keep lines starting with data
result.FilterLines(line => line.StartsWith("data:"));
```

```csharp
public CurlDotNet.Core.CurlResult FilterLines(System.Func<string,bool> predicate);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')