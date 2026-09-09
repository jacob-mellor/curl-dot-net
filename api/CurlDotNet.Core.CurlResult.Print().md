#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Print\(\) Method


<b>Print status code and body to console.</b>

More detailed debug output:

```csharp
result.Print();
// Output:
// Status: 200
// {"name":"John","age":30}
```

```csharp
public CurlDotNet.Core.CurlResult Print();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')