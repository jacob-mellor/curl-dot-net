#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## ErgonomicExtensions Class

Extra [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') helpers in the root `CurlDotNet` namespace: JSON parsing with your own
[System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions'), a non\-throwing `TryParseJson`, `EnsureSuccessStatusCode` and a
\(success, body, error\) tuple\.

```csharp
public static class ErgonomicExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ErgonomicExtensions

### Remarks

These are extension methods, so they appear on any [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') once `using CurlDotNet;` is in
            scope. Where [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') has an instance method of the same name and parameters (`ParseJson<T>()`,
            `SaveToFile(path)`, `GetHeader(name)`), C# calls the instance method; the extension version runs only when you
            pass the extra argument (`ParseJson<T>(options)`) or call it statically
            (`ErgonomicExtensions.SaveToFile(result, path)`).

The JSON helpers here always use System.Text.Json, on every target framework (including .NET Standard 2.0, where
            [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') uses Newtonsoft.Json).

Example: `var user = result.ParseJson<User>(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md

### See Also
- [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')
- [CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient')

| Methods | |
| :--- | :--- |
| [EnsureSuccessStatusCode\(this CurlResult\)](CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).md 'CurlDotNet\.ErgonomicExtensions\.EnsureSuccessStatusCode\(this CurlDotNet\.Core\.CurlResult\)') | Throws [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') unless the status is 200\-299, otherwise returns the result \- named like `HttpResponseMessage.EnsureSuccessStatusCode()` for code moved from HttpClient\. |
| [GetHeader\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.GetHeader\(this CurlDotNet\.Core\.CurlResult, string\)') | Returns a response header value by name, ignoring case, or null \- the same as [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)')\. |
| [HasContentType\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.HasContentType\(this CurlDotNet\.Core\.CurlResult, string\)') | True when the response's `Content-Type` header contains [contentType](CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).md#CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).contentType 'CurlDotNet\.ErgonomicExtensions\.HasContentType\(this CurlDotNet\.Core\.CurlResult, string\)\.contentType'), ignoring case \- so `HasContentType("json")` matches `application/json; charset=utf-8`\. |
| [ParseJson&lt;T&gt;\(this CurlResult, JsonSerializerOptions\)](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)') | Deserializes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') into a [T](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.T') with System\.Text\.Json and the [options](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).options 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.options') you choose, refusing non\-2xx responses and empty bodies\. |
| [SaveToFile\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.SaveToFile\(this CurlDotNet\.Core\.CurlResult, string\)') | Writes the response \(bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'), else the text of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\) to [filePath](CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).md#CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).filePath 'CurlDotNet\.ErgonomicExtensions\.SaveToFile\(this CurlDotNet\.Core\.CurlResult, string\)\.filePath') after checking it is a 2xx response with content, and returns how much was written\. |
| [ToSimple\(this CurlResult\)](CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult).md 'CurlDotNet\.ErgonomicExtensions\.ToSimple\(this CurlDotNet\.Core\.CurlResult\)') | Collapses the result into `(Success, Body, Error)` for code that just wants "did it work, what came back, and why not"\. |
| [TryParseJson&lt;T&gt;\(this CurlResult, T, JsonSerializerOptions\)](CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).md 'CurlDotNet\.ErgonomicExtensions\.TryParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, T, System\.Text\.Json\.JsonSerializerOptions\)') | Tries to deserialize [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') into a [T](CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).md#CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.TryParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, T, System\.Text\.Json\.JsonSerializerOptions\)\.T'); returns false instead of throwing when the status is not 2xx, the body is empty, or the JSON does not fit\. |
