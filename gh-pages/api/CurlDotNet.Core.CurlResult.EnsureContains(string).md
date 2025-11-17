#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.EnsureContains\(string\) Method


<b>Throw if response body doesn't contain expected text.</b>

Validate response content:

```csharp
// Make sure we got the right response
result.EnsureContains("success");

// Check for error messages
if (result.Body.Contains("error"))
{
    result.EnsureContains("recoverable");  // Make sure it's recoverable
}
```

```csharp
public CurlDotNet.Core.CurlResult EnsureContains(string expectedText);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.EnsureContains(string).expectedText'></a>

`expectedText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')