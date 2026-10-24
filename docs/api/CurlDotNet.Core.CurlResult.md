#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlResult Class

What a curl command returned: the HTTP status, the body text \(or bytes\), the response headers, the files
written, the timings \- plus helpers to parse JSON, save, validate and retry\.

```csharp
public class CurlResult
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlResult

### Example

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

if (result.IsSuccess)  // 200-299
{
    var user = result.ParseJson<User>();  // Parse JSON to your type
    result.SaveToFile("user.json");          // Save for later
}
```

### Remarks

Every `Curl.ExecuteAsync`/`Curl.Execute` call, [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') and
             [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl') request returns one. Transport failures (DNS, connection, timeout, TLS, `-f`
             with an HTTP error) are thrown as [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subclasses instead, so a CurlResult in your hand means
             the server answered - possibly with 404 or 500, which you check with [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess'),
             [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') or [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)'). (The non-throwing [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl') API
             returns a result with [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') 0 and the failure in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') instead.)

Where the data is: text responses are in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') (exactly what the curl tool would print on standard
             output, so with `-i` it starts with the headers and with `-w` it ends with the write-out text); binary
             responses (images, PDFs, zip, Office files) are in [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'); responses written with `-o`/`-O`
             are on disk and listed in [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles'); headers of the final response are in [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers').

Most helpers return the same instance, so calls chain:
             `var user = (await Curl.ExecuteAsync("curl https://api.example.com/users/42")).EnsureSuccess().SaveToFile("user.json").ParseJson<User>();`

A CurlResult is a plain object with settable properties; it is not thread-safe while being modified
             ([FilterLines\(Func&lt;string,bool&gt;\)](CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).md 'CurlDotNet\.Core\.CurlResult\.FilterLines\(System\.Func\<string,bool\>\)') changes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')).

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/05-understanding-results.md

### See Also
- [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
- [CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')
- [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

| Properties | |
| :--- | :--- |
| [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') | The response body as raw bytes when it is binary \(image, PDF, zip, Office file, or anything that is not text\); null for text responses, whose content is in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\. |
| [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') | The response as text \- exactly what the curl tool would print to standard output \(the body, plus the headers with `-i`, the `-v` log, and the `-w` output\)\. |
| [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') | The curl command that produced this result, as you passed it \(or as [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') rendered it\); used by [Retry\(\)](CurlDotNet.Core.CurlResult.Retry().md 'CurlDotNet\.Core\.CurlResult\.Retry\(\)') and in error messages\. |
| [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') | The failure behind a result that has no HTTP response, set only by the non\-throwing [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl') API; null for results from `Curl.ExecuteAsync`, which throws instead\. |
| [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers') | The headers of the final response, by name, case\-insensitive \(`Headers["content-type"]` works\); repeated headers such as `Set-Cookie` are joined with `", "`\. |
| [IsBinary](CurlDotNet.Core.CurlResult.IsBinary.md 'CurlDotNet\.Core\.CurlResult\.IsBinary') | True when the response body is in [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') \(non\-empty\) rather than in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\. |
| [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess') | True when [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') is 200\-299 \(OK, Created, No Content\.\.\.\); false for 1xx, 3xx, 4xx, 5xx and 0\. |
| [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles') | Paths of the files this transfer wrote: the `-o`/`-O` output \(with `--output-dir` and `--no-clobber` renaming applied\), plus any files saved later with [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)') and friends\. |
| [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') | The HTTP status code of the final response \(after any redirects followed with `-L`\), such as 200, 201, 304, 404 or 500 \- curl's `%{http_code}`\. |
| [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings') | How long each phase of the transfer took \(DNS, connect, TLS, first byte, total\) in milliseconds, like curl's `-w '%{time_total}'` family\. |

| Methods | |
| :--- | :--- |
| [AppendToFile\(string\)](CurlDotNet.Core.CurlResult.AppendToFile(string).md 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)') | Appends the response \(bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'), or the text of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\) to the end of [filePath](CurlDotNet.Core.CurlResult.AppendToFile(string).md#CurlDotNet.Core.CurlResult.AppendToFile(string).filePath 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)\.filePath'), creating the file if needed, and returns this result\. |
| [AsJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.AsJson_T_().md 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)') | Deserializes the JSON in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') into a [T](CurlDotNet.Core.CurlResult.AsJson_T_().md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T'); identical to [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)'), for those who prefer the name\. |
| [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)') | Parses [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') as JSON and returns it as a `dynamic` object, so you can read `json.rate.remaining` or `json.items[0].title` without writing a class first\. |
| [EnsureContains\(string\)](CurlDotNet.Core.CurlResult.EnsureContains(string).md 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)') | Throws [System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException') unless [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') contains [expectedText](CurlDotNet.Core.CurlResult.EnsureContains(string).md#CurlDotNet.Core.CurlResult.EnsureContains(string).expectedText 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)\.expectedText') \(exact, case\-sensitive\), otherwise returns this result \- a quick content check for smoke tests and health checks\. |
| [EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)') | Throws [CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException') unless [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') equals [expectedStatus](CurlDotNet.Core.CurlResult.EnsureStatus(int).md#CurlDotNet.Core.CurlResult.EnsureStatus(int).expectedStatus 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)\.expectedStatus') exactly, otherwise returns this result \- for APIs where only one code is right \(201 Created, 204 No Content\)\. |
| [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)') | Throws [CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException') unless [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') is 200\-299, otherwise returns this result \- the CurlDotNet version of `HttpResponseMessage.EnsureSuccessStatusCode()`\. |
| [FilterLines\(Func&lt;string,bool&gt;\)](CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).md 'CurlDotNet\.Core\.CurlResult\.FilterLines\(System\.Func\<string,bool\>\)') | Keeps only the lines of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') for which [predicate](CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).md#CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).predicate 'CurlDotNet\.Core\.CurlResult\.FilterLines\(System\.Func\<string,bool\>\)\.predicate') returns true \(this changes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') in place\) and returns this result\. |
| [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)') | Returns the value of one response header by name, ignoring case, or null when the response did not have it \- safer than indexing [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers'), which throws for a missing name\. |
| [HasHeader\(string\)](CurlDotNet.Core.CurlResult.HasHeader(string).md 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)') | True when the response contains the header [headerName](CurlDotNet.Core.CurlResult.HasHeader(string).md#CurlDotNet.Core.CurlResult.HasHeader(string).headerName 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)\.headerName') \(ignoring case\), so you can test before reading it\. |
| [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') | Deserializes the JSON in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') into a [T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T') \- your class, a `List<T>`, a `Dictionary<string, object>` \- matching property names case\-insensitively\. |
| [Print\(\)](CurlDotNet.Core.CurlResult.Print().md 'CurlDotNet\.Core\.CurlResult\.Print\(\)') | Writes `Status: {StatusCode}` and then [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to the console, and returns this result\. |
| [PrintBody\(\)](CurlDotNet.Core.CurlResult.PrintBody().md 'CurlDotNet\.Core\.CurlResult\.PrintBody\(\)') | Writes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to the console \(standard output\) followed by a newline, and returns this result \- handy in scripts and quick experiments\. |
| [PrintVerbose\(\)](CurlDotNet.Core.CurlResult.PrintVerbose().md 'CurlDotNet\.Core\.CurlResult\.PrintVerbose\(\)') | Writes the status, every response header, the body and the main timings \(DNS, connect, total\) to the console, and returns this result \- a quick look at everything that came back\. |
| [Retry\(\)](CurlDotNet.Core.CurlResult.Retry().md 'CurlDotNet\.Core\.CurlResult\.Retry\(\)') | Runs [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') again with [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') and returns the new result \- a one\-line manual retry after a 429, 503 or flaky network\. |
| [RetryWith\(Action&lt;CurlSettings&gt;\)](CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).md 'CurlDotNet\.Core\.CurlResult\.RetryWith\(System\.Action\<CurlDotNet\.Core\.CurlSettings\>\)') | Runs [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') again with [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') you configure \(longer timeout, extra header, retries, progress\) and returns the new result\. |
| [SaveAsCsv\(string\)](CurlDotNet.Core.CurlResult.SaveAsCsv(string).md 'CurlDotNet\.Core\.CurlResult\.SaveAsCsv\(string\)') | Converts a JSON array of objects in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to CSV \(one column per property of the first object\) and writes it to [filePath](CurlDotNet.Core.CurlResult.SaveAsCsv(string).md#CurlDotNet.Core.CurlResult.SaveAsCsv(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveAsCsv\(string\)\.filePath'); a body that is not such an array is written unchanged\. |
| [SaveAsJson\(string, bool\)](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)') | Writes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to [filePath](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md#CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).filePath 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)\.filePath') as re\-formatted JSON \(indented by default, or minified\), and returns this result; a body that is not JSON is written unchanged\. |
| [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)') | Writes the response to [filePath](CurlDotNet.Core.CurlResult.SaveToFile(string).md#CurlDotNet.Core.CurlResult.SaveToFile(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)\.filePath') \- the bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') for binary responses, otherwise the text of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') \- overwriting any existing file, and returns this result\. |
| [SaveToFileAsync\(string\)](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)') | Writes the response to [filePath](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md#CurlDotNet.Core.CurlResult.SaveToFileAsync(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)\.filePath') without blocking the calling thread \- bytes for binary responses, text for the rest \- and returns this result when the file is complete\. |
| [ToStream\(\)](CurlDotNet.Core.CurlResult.ToStream().md 'CurlDotNet\.Core\.CurlResult\.ToStream\(\)') | Returns the response as a new readable [System\.IO\.MemoryStream](https://learn.microsoft.com/en-us/dotnet/api/system.io.memorystream 'System\.IO\.MemoryStream'): the bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'), or [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') encoded as UTF\-8, or an empty stream\. |
| [Transform&lt;T&gt;\(Func&lt;CurlResult,T&gt;\)](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)') | Passes this result to [transformer](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.transformer') and returns whatever it returns \- a way to end a fluent chain with your own projection\. |
