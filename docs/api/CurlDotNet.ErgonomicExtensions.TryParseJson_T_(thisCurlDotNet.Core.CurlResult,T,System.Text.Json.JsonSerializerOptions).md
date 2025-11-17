#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.TryParseJson\<T\>\(this CurlResult, T, JsonSerializerOptions\) Method

Try to parse the response body as JSON\. Returns false if parsing fails\.

```csharp
public static bool TryParseJson<T>(this CurlDotNet.Core.CurlResult result, out T value, System.Text.Json.JsonSerializerOptions? options=null)
    where T : class;
```
#### Type parameters

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).T'></a>

`T`

The type to deserialize to
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult to parse

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).value'></a>

`value` [T](CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.TryParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, T, System\.Text\.Json\.JsonSerializerOptions\)\.T')

The deserialized object if successful

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).options'></a>

`options` [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')

Optional JSON serializer options

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if parsing succeeded, false otherwise