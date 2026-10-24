#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.ParseJson\<T\>\(this CurlResult, JsonSerializerOptions\) Method

Deserializes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') into a [T](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.T') with System\.Text\.Json and the
[options](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).options 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.options') you choose, refusing non\-2xx responses and empty bodies\.

```csharp
public static T ParseJson<T>(this CurlDotNet.Core.CurlResult result, System.Text.Json.JsonSerializerOptions? options=null);
```
#### Type parameters

<a name='CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T'></a>

`T`

A class, struct or collection that System\.Text\.Json can create, such as `User` or `List<User>`\.
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The response to parse; must not be null\.

<a name='CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).options'></a>

`options` [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')

System\.Text\.Json options \(naming policy, converters, case sensitivity\), or null for
            case\-insensitive defaults\.

#### Returns
[T](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.T')  
The deserialized [T](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.T'); never null\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[result](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).result 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.result') is null\.

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
The status is not 2xx, the body is empty, the body is not valid JSON for
            [T](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.T'), or the JSON is the literal `null`\. The message names the URL, status and the start of
            the body; for invalid JSON the [System\.Text\.Json\.JsonException](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonexception 'System\.Text\.Json\.JsonException') is the InnerException\. Fix: check
            [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') and print [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/parse\-json\-from\-curl\-in\-csharp/README\.md

### Remarks

Called as `result.ParseJson<T>(options)`. Without an argument, `result.ParseJson<T>()` binds
            to [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') instead, which does not check the status. When [options](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).options 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.options')
            is null, `PropertyNameCaseInsensitive = true` is used.

Example: `var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };`
            then `var order = result.ParseJson<Order>(opts);`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md

### See Also
- [TryParseJson&lt;T&gt;\(this CurlResult, T, JsonSerializerOptions\)](CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).md 'CurlDotNet\.ErgonomicExtensions\.TryParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, T, System\.Text\.Json\.JsonSerializerOptions\)')
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')