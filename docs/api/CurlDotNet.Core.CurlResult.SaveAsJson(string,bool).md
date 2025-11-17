#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.SaveAsJson\(string, bool\) Method


<b>Save as formatted JSON file (pretty-printed).</b>

Makes JSON human-readable with indentation:

```csharp
// Save with nice formatting
result.SaveAsJson("data.json");           // Pretty-printed
result.SaveAsJson("data.json", false);    // Minified

// Before: {"name":"John","age":30}
// After:  {
//           "name": "John",
//           "age": 30
//         }
```

```csharp
public CurlDotNet.Core.CurlResult SaveAsJson(string filePath, bool indented=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save the JSON file

<a name='CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).indented'></a>

`indented` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

true for pretty formatting \(default\), false for minified

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result \(for chaining\)