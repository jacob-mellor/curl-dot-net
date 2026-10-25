#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## CurlApiClient Class

A tiny REST client bound to one base URL: `GetAsync("users/1")`, `PostJsonAsync("users", obj)`, PUT, PATCH and
DELETE, each with the same time limit \- for scripts that call one API many times\.

```csharp
public class CurlApiClient
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlApiClient

### Remarks

Each call builds a [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') for `baseUrl + "/" + path` and runs it with a
            `--max-time` equal to the client's timeout. Responses with any HTTP status are returned (check
            [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess')); transport failures throw [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subclasses, such as
            [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException') or [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException'). The client holds no
            connection or state and is safe to share between threads.

Bodies are serialized with System.Text.Json (Newtonsoft.Json on .NET Standard 2.0), keeping C# property names;
            `Content-Type: application/json` is set for you. There is no built-in authentication: for a token, build requests
            with [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)') instead.

Example: `var api = new CurlApiClient("https://jsonplaceholder.typicode.com", TimeSpan.FromSeconds(10));`
            then `var post = (await api.GetAsync("posts/1")).EnsureSuccess().ParseJson<Post>();` and
            `await api.PostJsonAsync("posts", new { title = "hi", body = "...", userId = 1 });`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)')

| Constructors | |
| :--- | :--- |
| [CurlApiClient\(string, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)') | Creates a client for [baseUrl](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md#CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).baseUrl 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)\.baseUrl') whose requests all use [defaultTimeout](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md#CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).defaultTimeout 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)\.defaultTimeout') \(30 seconds when null\) as their total time limit\. |

| Methods | |
| :--- | :--- |
| [DeleteAsync\(string\)](CurlDotNet.CurlApiClient.DeleteAsync(string).md 'CurlDotNet\.CurlApiClient\.DeleteAsync\(string\)') | Sends `DELETE baseUrl/path` and returns the response \(often 204 No Content\) \- like `curl -X DELETE baseUrl/path`\. |
| [GetAsync\(string\)](CurlDotNet.CurlApiClient.GetAsync(string).md 'CurlDotNet\.CurlApiClient\.GetAsync\(string\)') | Sends `GET baseUrl/path` and returns the response, whatever its status \- like `curl -m T baseUrl/path`\. |
| [PatchJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PatchJsonAsync\(string, object\)') | Sends `PATCH baseUrl/path` with [data](CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).md#CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).data 'CurlDotNet\.CurlApiClient\.PatchJsonAsync\(string, object\)\.data') serialized as JSON \(change some fields\) and returns the response \- like `curl -X PATCH --json '{...}' baseUrl/path`\. |
| [PostJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PostJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PostJsonAsync\(string, object\)') | Sends `POST baseUrl/path` with [data](CurlDotNet.CurlApiClient.PostJsonAsync(string,object).md#CurlDotNet.CurlApiClient.PostJsonAsync(string,object).data 'CurlDotNet\.CurlApiClient\.PostJsonAsync\(string, object\)\.data') serialized as JSON and returns the response \- like `curl --json '{...}' baseUrl/path`\. |
| [PutJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PutJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PutJsonAsync\(string, object\)') | Sends `PUT baseUrl/path` with [data](CurlDotNet.CurlApiClient.PutJsonAsync(string,object).md#CurlDotNet.CurlApiClient.PutJsonAsync(string,object).data 'CurlDotNet\.CurlApiClient\.PutJsonAsync\(string, object\)\.data') serialized as JSON \(replace a resource\) and returns the response \- like `curl -X PUT --json '{...}' baseUrl/path`\. |
