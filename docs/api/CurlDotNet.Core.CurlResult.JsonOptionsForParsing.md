#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.JsonOptionsForParsing Field

JSON APIs are overwhelmingly camelCase while C\# properties are PascalCase, so property
matching is case\-insensitive by default \(the same as Newtonsoft on netstandard2\.0\)\.

```csharp
private static readonly JsonSerializerOptions JsonOptionsForParsing;
```

#### Field Value
[System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')