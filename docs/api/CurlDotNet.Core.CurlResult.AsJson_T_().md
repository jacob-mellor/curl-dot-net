#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.AsJson\<T\>\(\) Method

Deserializes the JSON in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') into a [T](CurlDotNet.Core.CurlResult.AsJson_T_().md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T'); identical to
[ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)'), for those who prefer the name\.

```csharp
public T AsJson<T>();
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.AsJson_T_().T'></a>

`T`

The type to create; see [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')\.

#### Returns
[T](CurlDotNet.Core.CurlResult.AsJson_T_().md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T')  
A new [T](CurlDotNet.Core.CurlResult.AsJson_T_().md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T') filled from [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

#### Exceptions

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
The body is empty \(a 204, a HEAD request, output sent to a file with
            `-o`, or binary data\), the body is not valid JSON \(often an HTML error page or login redirect\), or the JSON
            does not fit [T](CurlDotNet.Core.CurlResult.AsJson_T_().md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T') \(an array into a single object, text into a number\)\. The message names the
            URL, status, Content\-Type and the start of the body; the parser's exception is the InnerException\. Fix: check
            [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') first and print [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/parse\-json\-from\-curl\-in\-csharp/README\.md

### Remarks

Same serializer, case-insensitive matching and errors as [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)').

Example: `var order = result.AsJson<Order>();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md

### See Also
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')