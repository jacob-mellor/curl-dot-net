#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlResult Class


<b>🎯 The response from your curl command - everything you need is here!</b>

After running any curl command, you get this object back. It has the status code,
             response body, headers, and helpful methods to work with the data.

<b>The API is designed to be intuitive - just type what you want to do:</b>
- Want the body? → `result.Body`
- Want JSON? → `result.ParseJson<T>()` or `result.AsJson<T>()`
- Want to save? → `result.SaveToFile("path")`
- Want headers? → `result.Headers["Content-Type"]`
- Check success? → `result.IsSuccess` or `result.EnsureSuccess()`

<b>Quick Example:</b>

```csharp
var result = await Curl.Execute("curl https://api.github.com/users/octocat");

if (result.IsSuccess)  // Was it 200-299?
{
    var user = result.ParseJson<User>();  // Parse JSON to your type
    result.SaveToFile("user.json");       // Save for later
}
```

```csharp
public class CurlResult
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlResult

### Remarks

<b>Design Philosophy:</b> Every method name tells you exactly what it does.
             No surprises. If you guess a method name, it probably exists and does what you expect.

<b>Fluent API:</b> Most methods return 'this' so you can chain operations:

```csharp
result
    .EnsureSuccess()           // Throw if not 200-299
    .SaveToFile("backup.json") // Save a copy
    .ParseJson<Data>()        // Parse and return data
```

| Properties | |
| :--- | :--- |
| [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') |   <b>Binary data for files like images, PDFs, downloads.</b>  When you download non-text files, the bytes are here:\`\.\.\.\` |
| [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') |   <b>The response body as a string - this is your data!</b>  Contains whatever the server sent back: JSON, HTML, XML, plain text, etc.  <b>Common patterns:</b>\`\.\.\.\`  <b>Note:</b> For binary data (images, PDFs), use [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') instead.  <b>Note:</b> Can be null for 204 No Content or binary responses. |
| [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') |   <b>The original curl command that was executed.</b>  Useful for debugging or retrying:\`\.\.\.\` |
| [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception') |   <b>Any exception if the request failed completely.</b>  Only set for network failures, not HTTP errors:\`\.\.\.\` |
| [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers') |   <b>All HTTP headers from the response - contains metadata about the response.</b>  Headers tell you things like content type, cache rules, rate limits, etc.              Access them like a dictionary (case-insensitive keys).  <b>Get a specific header:</b>\`\.\.\.\`  <b>Check rate limits (common in APIs):</b>\`\.\.\.\`  <b>Common headers:</b>\.\.\. |
| [IsBinary](CurlDotNet.Core.CurlResult.IsBinary.md 'CurlDotNet\.Core\.CurlResult\.IsBinary') |   <b>Is this binary data? (images, PDFs, etc.)</b>  Quick check before accessing BinaryData:\`\.\.\.\` |
| [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess') |   <b>Quick success check - true if status is 200-299.</b>  The easiest way to check if your request worked:\`\.\.\.\`  What's considered success: 200 OK, 201 Created, 204 No Content, etc.  What's NOT success: 404 Not Found, 500 Server Error, etc. |
| [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles') |   <b>Files that were saved (if using -o flag).</b>  Track what files were created:\`\.\.\.\` |
| [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') |   <b>The HTTP status code - tells you what happened.</b>  Common codes you'll see:\`\.\.\.\`  <b>Example - Handle different statuses:</b>\`\.\.\.\` |
| [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings') |   <b>Detailed timing information (like curl -w).</b>  See how long each phase took:\`\.\.\.\` |

| Methods | |
| :--- | :--- |
| [AppendToFile\(string\)](CurlDotNet.Core.CurlResult.AppendToFile(string).md 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)') |   <b>Append response to an existing file.</b>  Add to a file without overwriting:\`\.\.\.\` |
| [AsJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.AsJson_T_().md 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)') |   <b>Parse JSON response (alternative name for <see cref="M:CurlDotNet.Core.CurlResult.ParseJson``1"/>).</b>  Some people prefer `AsJson`, some prefer `ParseJson`. Both methods are identical and produce the same result.\`\.\.\.\` |
| [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)') |   <b>Parse JSON as dynamic object (when you don't have a class).</b>  Useful for quick exploration or simple JSON structures. This method returns a dynamic object that allows               you to access JSON properties without defining a C# class. However, there's no compile-time checking, so prefer               [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') with typed classes when possible.  <b>Example:</b>\`\.\.\.\` |
| [EnsureContains\(string\)](CurlDotNet.Core.CurlResult.EnsureContains(string).md 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)') |   <b>Throw if response body doesn't contain expected text.</b>  Validate response content:\`\.\.\.\` |
| [EnsureStatus\(int\)](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)') |   <b>Throw if status doesn't match what you expect.</b>  Validate specific status codes:\`\.\.\.\` |
| [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)') |   <b>Throw an exception if the request wasn't successful (not 200-299).</b>  Use this when you expect success and want to fail fast. This matches curl's `-f` (fail) flag behavior. |
| [FilterLines\(Func&lt;string,bool&gt;\)](CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).md 'CurlDotNet\.Core\.CurlResult\.FilterLines\(System\.Func\<string,bool\>\)') |   <b>Extract lines that match a condition.</b>  Filter text responses:\`\.\.\.\` |
| [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)') |   <b>Get a specific header value (case-insensitive).</b>  Easy header access with null safety. This matches curl's header behavior exactly. |
| [HasHeader\(string\)](CurlDotNet.Core.CurlResult.HasHeader(string).md 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)') |   <b>Check if a header exists.</b>  Test for header presence before accessing. This is case-insensitive, matching curl's behavior. |
| [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') |   <b>Parse the JSON response into your C# class.</b>  The most common operation - turning JSON into objects. This method uses [System\.Text\.Json\.JsonSerializer](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer 'System\.Text\.Json\.JsonSerializer')               in .NET 6+ and [Newtonsoft\.Json\.JsonConvert](https://learn.microsoft.com/en-us/dotnet/api/newtonsoft.json.jsonconvert 'Newtonsoft\.Json\.JsonConvert') in .NET Standard 2.0 for maximum compatibility.  <b>Example:</b>\`\.\.\.\`  <b>Tip:</b> Use https://json2csharp.com to generate C# classes from JSON! |
| [Print\(\)](CurlDotNet.Core.CurlResult.Print().md 'CurlDotNet\.Core\.CurlResult\.Print\(\)') |   <b>Print status code and body to console.</b>  More detailed debug output:\`\.\.\.\` |
| [PrintBody\(\)](CurlDotNet.Core.CurlResult.PrintBody().md 'CurlDotNet\.Core\.CurlResult\.PrintBody\(\)') |   <b>Print the response body to console.</b>  Quick debugging output:\`\.\.\.\` |
| [PrintVerbose\(\)](CurlDotNet.Core.CurlResult.PrintVerbose().md 'CurlDotNet\.Core\.CurlResult\.PrintVerbose\(\)') |   <b>Print everything - status, headers, and body (like curl -v).</b>  Full debug output:\`\.\.\.\` |
| [Retry\(\)](CurlDotNet.Core.CurlResult.Retry().md 'CurlDotNet\.Core\.CurlResult\.Retry\(\)') |   <b>Retry the same curl command again.</b>  Simple retry for transient failures:\`\.\.\.\` |
| [RetryWith\(Action&lt;CurlSettings&gt;\)](CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).md 'CurlDotNet\.Core\.CurlResult\.RetryWith\(System\.Action\<CurlDotNet\.Core\.CurlSettings\>\)') |   <b>Retry with modifications to the original command.</b>  Retry with different settings:\`\.\.\.\` |
| [SaveAsCsv\(string\)](CurlDotNet.Core.CurlResult.SaveAsCsv(string).md 'CurlDotNet\.Core\.CurlResult\.SaveAsCsv\(string\)') |   <b>Save JSON response as CSV file (for JSON arrays).</b>  Converts JSON arrays to CSV for Excel:\`\.\.\.\`  <b>Note:</b> Only works with JSON arrays of objects. |
| [SaveAsJson\(string, bool\)](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)') |   <b>Save as formatted JSON file (pretty-printed).</b>  Makes JSON human-readable with indentation:\`\.\.\.\` |
| [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)') |   <b>Save the response to a file - works for both text and binary!</b>  Smart saving - automatically handles text vs binary:\`\.\.\.\`  <b>Path examples:</b>\`\.\.\.\` |
| [SaveToFileAsync\(string\)](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)') |   <b>Save the response to a file asynchronously.</b>  Same as SaveToFile but doesn't block:\`\.\.\.\` |
| [ToStream\(\)](CurlDotNet.Core.CurlResult.ToStream().md 'CurlDotNet\.Core\.CurlResult\.ToStream\(\)') |   <b>Convert the response to a Stream for reading.</b>  Useful for streaming or processing data:\`\.\.\.\` |
| [Transform&lt;T&gt;\(Func&lt;CurlResult,T&gt;\)](CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).md 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)') |   <b>Transform the result using your own function.</b>  Extract or convert data however you need:\`\.\.\.\` |
