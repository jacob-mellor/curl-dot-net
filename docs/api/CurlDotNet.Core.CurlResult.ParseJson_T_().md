#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.ParseJson\<T\>\(\) Method

Deserializes the JSON in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') into a [T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T') \- your class, a
`List<T>`, a `Dictionary<string, object>` \- matching property names case\-insensitively\.

```csharp
public T ParseJson<T>();
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.ParseJson_T_().T'></a>

`T`

A class or struct with a public parameterless constructor and settable properties, a
             collection such as [System\.Collections\.Generic\.List&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1'), or [System\.Collections\.Generic\.Dictionary&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')\. Interfaces and abstract types
             cannot be created\.

#### Returns
[T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T')  
A new [T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T') filled from [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'); `null`/`default` when the body is
             the JSON literal `null`\.

#### Exceptions

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
The body is empty \(a 204, a HEAD request, output sent to a file with
             `-o`, or binary data\), the body is not valid JSON \(often an HTML error page or login redirect\), or the JSON
             does not fit [T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T') \(an array into a single object, text into a number\)\. The message names the
             URL, status, Content\-Type and the start of the body; the parser's exception is the InnerException\. Fix: check
             [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') first and print [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.
             See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/parse\-json\-from\-curl\-in\-csharp/README\.md

### Example

```csharp
public class User { public string Name { get; set; } public string Email { get; set; } public int Id { get; set; } }

var response = await Curl.ExecuteAsync("curl https://api.example.com/users/42");
var user = response.EnsureSuccess().ParseJson<User>();
Console.WriteLine($"Hello {user.Name}!");

var users = (await Curl.ExecuteAsync("curl https://api.example.com/users")).ParseJson<List<User>>();
```

### Remarks

On .NET 8/9/10 and .NET Framework 4.7.2/4.8 this uses System.Text.Json with
             `PropertyNameCaseInsensitive = true` (so `"userName"` fills `UserName`); on the .NET Standard 2.0
             build it uses Newtonsoft.Json, which is case-insensitive by default. JSON properties without a matching C#
             property are ignored, and C# properties without a JSON value keep their default. JSON `null` returns
             `null`/`default`.

The status code is not checked: a 404 with a JSON error body parses happily into your type with empty
             properties. Call [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)') first when only 2xx bodies are valid.

Example: `var user = (await Curl.ExecuteAsync("curl https://api.github.com/users/octocat")).EnsureSuccess().ParseJson<GitHubUser>();`
             then `Console.WriteLine(user.Login);`. For arrays: `result.ParseJson<List<Repo>>()`. To explore
             JSON without a class, use [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)'); to choose your own serializer options, use the
             [ParseJson&lt;T&gt;\(this CurlResult, JsonSerializerOptions\)](CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).md 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)') extension.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md

### See Also
- [AsJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.AsJson_T_().md 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)')
- [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')
- [SaveAsJson\(string, bool\)](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)')