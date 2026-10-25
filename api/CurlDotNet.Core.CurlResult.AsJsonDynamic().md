#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.AsJsonDynamic\(\) Method

Parses [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') as JSON and returns it as a `dynamic` object, so you can read
`json.rate.remaining` or `json.items[0].title` without writing a class first\.

```csharp
public dynamic AsJsonDynamic();
```

#### Returns
[dynamic](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/using-type-dynamic 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/programming\-guide/types/using\-type\-dynamic')  
The root of the JSON document as described in the remarks: a dynamic object for
             `{...}`, a `List<object>` for `[...]`, or a plain value \(string, number, bool\)
             for a document that is just a value\. Never a [System\.Text\.Json\.JsonDocument](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument 'System\.Text\.Json\.JsonDocument')\.

#### Exceptions

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
The body is empty \(for example a 204 response or a HEAD
             request\) or is not valid JSON \(for example an HTML error page\)\. The message names the URL, the
             Content\-Type and the first characters of the body; the parser's own exception is the
             [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. Fix: check [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') and print
             [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/parse\-json\-from\-curl\-in\-csharp/README\.md

### Example

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");
dynamic json = result.AsJsonDynamic();
int remaining = json.rate.remaining;
Console.WriteLine($"{remaining} of {json.rate.limit} calls left");

var repos = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat/repos");
foreach (var repo in repos.AsJsonDynamic())   // a JSON array
    Console.WriteLine(repo.full_name);
```

### Remarks

What you get back, on .NET 8/9/10 and .NET Framework 4.7.2/4.8 (System.Text.Json builds):
             a JSON object becomes a dynamic object that supports `json.name`, `json["name"]`,
             `foreach (var property in json)` (key/value pairs) and a cast to
             `Dictionary<string, object>`; a JSON array becomes a `List<object>` (use
             `json.items[0]`, `json.items.Count`, `foreach`); numbers become `int` when they fit,
             otherwise `long`, otherwise `double`; strings become `string`, `true`/`false`
             become `bool` and `null` becomes `null`.

A property that does not exist reads as `null` instead of throwing. Property names are
             matched exactly first; when that fails a single case-insensitive match is used, so `json.Name`
             also finds `"name"`. `json.ToString()` on an object returns its original JSON text.

On the .NET Standard 2.0 build (Unity, Xamarin, older runtimes) the result is Newtonsoft's
             `JObject`/`JArray`, which supports the same `json.name`, `json["name"]` and
             `foreach` syntax; numbers are `JValue`s that convert implicitly to `int`, `long` or
             `double`.

Example, reading GitHub's rate limit without a class: `var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");`
             then `dynamic json = result.AsJsonDynamic();` and `int remaining = json.rate.remaining;`
             and `Console.WriteLine($"{remaining} of {json.rate.limit} calls left");`.

Example, iterating an array: `foreach (var repo in result.AsJsonDynamic()) Console.WriteLine(repo.full_name);`
             for a response such as `[{"full_name":"a/b"}, ...]`.

There is no compile-time checking with `dynamic`: a typo in a property name gives
             `null`, not a compiler error. For code you keep, prefer [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') with a typed
             class. Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md

### See Also
- [Parse JSON into a typed class \(recommended for code you keep\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [Same as ParseJson](CurlDotNet.Core.CurlResult.AsJson_T_().md 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)')
- [The JSON text being parsed](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')