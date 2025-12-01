#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## ErgonomicExtensions Class

Ergonomic extension methods for CurlResult to improve developer experience\.
These extensions are in the CurlDotNet namespace for easy discovery\.

```csharp
public static class ErgonomicExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ErgonomicExtensions

| Methods | |
| :--- | :--- |
| [EnsureSuccessStatusCode\(this CurlResult\)](CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).md 'CurlDotNet\.ErgonomicExtensions\.EnsureSuccessStatusCode\(this CurlDotNet\.Core\.CurlResult\)') | Throw an exception if the request was not successful\. Similar to HttpResponseMessage\.EnsureSuccessStatusCode\(\)\. |
| [GetHeader\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.GetHeader\(this CurlDotNet\.Core\.CurlResult, string\)') | Get a specific header value from the response\. |
| [HasContentType\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.HasContentType\(this CurlDotNet\.Core\.CurlResult, string\)') | Check if the response has a specific content type\. |
| [ParseJson&lt;T&gt;\(this CurlResult, JsonSerializerOptions\)](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)') | Parse the response body as JSON and deserialize to a strongly\-typed object\. |
| [SaveToFile\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.SaveToFile\(this CurlDotNet\.Core\.CurlResult, string\)') | Save the response body to a file\. |
| [ToSimple\(this CurlResult\)](CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult).md 'CurlDotNet\.ErgonomicExtensions\.ToSimple\(this CurlDotNet\.Core\.CurlResult\)') | Convert the CurlResult to a simplified success/error tuple\. |
| [TryParseJson&lt;T&gt;\(this CurlResult, T, JsonSerializerOptions\)](CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).md 'CurlDotNet\.ErgonomicExtensions\.TryParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, T, System\.Text\.Json\.JsonSerializerOptions\)') | Try to parse the response body as JSON\. Returns false if parsing fails\. |
