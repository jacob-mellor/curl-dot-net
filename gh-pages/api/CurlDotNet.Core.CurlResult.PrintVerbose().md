#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.PrintVerbose\(\) Method


<b>Print everything - status, headers, and body (like curl -v).</b>

Full debug output:

```csharp
result.PrintVerbose();
// Output:
// Status: 200
// Headers:
//   Content-Type: application/json
//   Content-Length: 123
// Body:
// {"name":"John"}
```

```csharp
public CurlDotNet.Core.CurlResult PrintVerbose();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')