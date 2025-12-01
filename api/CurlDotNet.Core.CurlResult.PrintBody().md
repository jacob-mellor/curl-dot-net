#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.PrintBody\(\) Method


<b>Print the response body to console.</b>

Quick debugging output:

```csharp
result.PrintBody();  // Just prints the body

// Chain with other operations
result
    .PrintBody()           // Debug output
    .SaveToFile("out.txt") // Also save it
    .ParseJson<Data>();   // Then parse
```

```csharp
public CurlDotNet.Core.CurlResult PrintBody();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result \(for chaining\)