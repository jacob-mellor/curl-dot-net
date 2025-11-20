#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

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
### Properties

<a name='CurlDotNet.Core.CurlResult.BinaryData'></a>

## CurlResult\.BinaryData Property


<b>Binary data for files like images, PDFs, downloads.</b>

When you download non-text files, the bytes are here:

```csharp
// Download an image
var result = await Curl.Execute("curl https://example.com/logo.png");

if (result.IsBinary)
{
    File.WriteAllBytes("logo.png", result.BinaryData);
    Console.WriteLine($"Saved {result.BinaryData.Length} bytes");
}
```

```csharp
public byte[] BinaryData { get; set; }
```

#### Property Value
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='CurlDotNet.Core.CurlResult.Body'></a>

## CurlResult\.Body Property


<b>The response body as a string - this is your data!</b>

Contains whatever the server sent back: JSON, HTML, XML, plain text, etc.

<b>Common patterns:</b>

```csharp
// JSON API response (most common)
if (result.Body.StartsWith("{"))
{
    var data = result.ParseJson<MyClass>();
}

// HTML webpage
if (result.Body.Contains("<html"))
{
    result.SaveToFile("page.html");
}

// Plain text
Console.WriteLine(result.Body);
```

<b>Note:</b> For binary data (images, PDFs), use [BinaryData](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.BinaryData 'CurlDotNet\.Core\.CurlResult\.BinaryData') instead.

<b>Note:</b> Can be null for 204 No Content or binary responses.

```csharp
public string Body { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlResult.Command'></a>

## CurlResult\.Command Property


<b>The original curl command that was executed.</b>

Useful for debugging or retrying:

```csharp
Console.WriteLine($"Executed: {result.Command}");

// Retry the same command
var retry = await Curl.Execute(result.Command);
```

```csharp
public string Command { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlResult.Exception'></a>

## CurlResult\.Exception Property


<b>Any exception if the request failed completely.</b>

Only set for network failures, not HTTP errors:

```csharp
if (result.Exception != null)
{
    // Network/DNS/Timeout failure
    Console.WriteLine($"Failed: {result.Exception.Message}");
}
else if (!result.IsSuccess)
{
    // HTTP error (404, 500, etc)
    Console.WriteLine($"HTTP {result.StatusCode}");
}
```

```csharp
public System.Exception Exception { get; set; }
```

#### Property Value
[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

<a name='CurlDotNet.Core.CurlResult.Headers'></a>

## CurlResult\.Headers Property


<b>All HTTP headers from the response - contains metadata about the response.</b>

Headers tell you things like content type, cache rules, rate limits, etc.
             Access them like a dictionary (case-insensitive keys).

<b>Get a specific header:</b>

```csharp
// These all work (case-insensitive):
var type = result.Headers["Content-Type"];
var type = result.Headers["content-type"];
var type = result.Headers["CONTENT-TYPE"];

// Or use the helper:
var type = result.GetHeader("Content-Type");
```

<b>Check rate limits (common in APIs):</b>

```csharp
if (result.Headers.ContainsKey("X-RateLimit-Remaining"))
{
    var remaining = int.Parse(result.Headers["X-RateLimit-Remaining"]);
    if (remaining < 10)
        Console.WriteLine("⚠️ Only {0} API calls left!", remaining);
}
```

<b>Common headers:</b>
- <b>Content-Type</b> - Format of the data (application/json, text/html)
- <b>Content-Length</b> - Size in bytes
- <b>Location</b> - Where you got redirected to
- <b>Set-Cookie</b> - Cookies to store
- <b>Cache-Control</b> - How long to cache

```csharp
public System.Collections.Generic.Dictionary<string,string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlResult.IsBinary'></a>

## CurlResult\.IsBinary Property


<b>Is this binary data? (images, PDFs, etc.)</b>

Quick check before accessing BinaryData:

```csharp
if (result.IsBinary)
    File.WriteAllBytes("file.bin", result.BinaryData);
else
    File.WriteAllText("file.txt", result.Body);
```

```csharp
public bool IsBinary { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlResult.IsSuccess'></a>

## CurlResult\.IsSuccess Property


<b>Quick success check - true if status is 200-299.</b>

The easiest way to check if your request worked:

```csharp
if (result.IsSuccess)
{
    // It worked! Do something with result.Body
}
else
{
    // Something went wrong, check result.StatusCode
}
```

What's considered success: 200 OK, 201 Created, 204 No Content, etc.

What's NOT success: 404 Not Found, 500 Server Error, etc.

```csharp
public bool IsSuccess { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.CurlResult.OutputFiles'></a>

## CurlResult\.OutputFiles Property


<b>Files that were saved (if using -o flag).</b>

Track what files were created:

```csharp
foreach (var file in result.OutputFiles)
{
    Console.WriteLine($"Saved: {file}");
}
```

```csharp
public System.Collections.Generic.List<string> OutputFiles { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='CurlDotNet.Core.CurlResult.StatusCode'></a>

## CurlResult\.StatusCode Property


<b>The HTTP status code - tells you what happened.</b>

Common codes you'll see:

```csharp
200 = OK, it worked!
201 = Created something new
204 = Success, but no content returned
400 = Bad request (you sent something wrong)
401 = Unauthorized (need to login)
403 = Forbidden (not allowed)
404 = Not found
429 = Too many requests (slow down!)
500 = Server error (their fault, not yours)
503 = Service unavailable (try again later)
```

<b>Example - Handle different statuses:</b>

```csharp
switch (result.StatusCode)
{
    case 200: ProcessData(result.Body); break;
    case 404: Console.WriteLine("Not found"); break;
    case 401: RedirectToLogin(); break;
    case >= 500: Console.WriteLine("Server error, retry later"); break;
}
```

```csharp
public int StatusCode { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Core.CurlResult.Timings'></a>

## CurlResult\.Timings Property


<b>Detailed timing information (like curl -w).</b>

See how long each phase took:

```csharp
Console.WriteLine($"DNS lookup: {result.Timings.NameLookup}ms");
Console.WriteLine($"Connect: {result.Timings.Connect}ms");
Console.WriteLine($"Total: {result.Timings.Total}ms");
```

```csharp
public CurlDotNet.Core.CurlTimings Timings { get; set; }
```

#### Property Value
[CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')
### Methods

<a name='CurlDotNet.Core.CurlResult.AppendToFile(string)'></a>

## CurlResult\.AppendToFile\(string\) Method


<b>Append response to an existing file.</b>

Add to a file without overwriting:

```csharp
// Log all responses
result.AppendToFile("api-log.txt");

// Build up a file over time
foreach (var url in urls)
{
    var r = await Curl.Execute($"curl {url}");
    r.AppendToFile("combined.txt");
}
```

```csharp
public CurlDotNet.Core.CurlResult AppendToFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.AppendToFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Core.CurlResult.AsJson_T_()'></a>

## CurlResult\.AsJson\<T\>\(\) Method


<b>Parse JSON response (alternative name for <see cref="M:CurlDotNet.Core.CurlResult.ParseJson``1"/>).</b>

Some people prefer `AsJson`, some prefer `ParseJson`. Both methods are identical and produce the same result.

```csharp
var data = result.AsJson<MyData>();
// Exactly the same as: result.ParseJson<MyData>()
```

```csharp
public T AsJson<T>();
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.AsJson_T_().T'></a>

`T`

The type to deserialize to\. See [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_() 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') for details\.

#### Returns
[T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T')  
An instance of [T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AsJson_T_().T 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)\.T') with data from the JSON [Body](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Body 'CurlDotNet\.Core\.CurlResult\.Body')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
Thrown when [Body](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Body 'CurlDotNet\.Core\.CurlResult\.Body') is null or empty\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
Thrown when JSON deserialization fails\.

[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')  
Thrown when the JSON syntax is invalid\.

### Remarks

This is simply an alias for [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_() 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)'). Use whichever method name you prefer.

### See Also
- [Primary method for parsing JSON](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_() 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [Parse JSON as dynamic without a class](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AsJsonDynamic() 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')

<a name='CurlDotNet.Core.CurlResult.AsJsonDynamic()'></a>

## CurlResult\.AsJsonDynamic\(\) Method


<b>Parse JSON as dynamic object (when you don't have a class).</b>

Useful for quick exploration or simple JSON structures. This method returns a dynamic object that allows 
             you to access JSON properties without defining a C# class. However, there's no compile-time checking, so prefer 
             [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_() 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') with typed classes when possible.

<b>Example:</b>

```csharp
dynamic json = result.AsJsonDynamic();
Console.WriteLine(json.name);           // Access properties directly
Console.WriteLine(json.users[0].email); // Navigate arrays

// Iterate dynamic arrays
foreach (var item in json.items)
{
    Console.WriteLine(item.title);
}
```

```csharp
public dynamic AsJsonDynamic();
```

#### Returns
[dynamic](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/using-type-dynamic 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/programming\-guide/types/using\-type\-dynamic')  

A dynamic object representing the JSON. In .NET 6+, this is a [System\.Text\.Json\.JsonDocument](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument 'System\.Text\.Json\.JsonDocument').
             In .NET Standard 2.0, this is a `JObject` from Newtonsoft.Json.

Access properties like: `dynamicObj.propertyName` or `dynamicObj["propertyName"]`

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
Thrown when [Body](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Body 'CurlDotNet\.Core\.CurlResult\.Body') is null or empty\.

[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')  
Thrown when the JSON syntax is invalid\.

### Remarks

<b>⚠️ Warning:</b> No compile-time checking! If a property doesn't exist, you'll get a runtime exception.

For production code, prefer [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_() 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') with typed classes for better safety and IntelliSense support.

This method is useful for:
- Quick prototyping and exploration
- Working with highly dynamic JSON structures
- One-off scripts and tools

### See Also
- [Parse JSON into typed classes \(recommended\)](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_() 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [Alternative method name for typed parsing](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AsJson_T_() 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)')
- [The JSON string being parsed](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Body 'CurlDotNet\.Core\.CurlResult\.Body')

<a name='CurlDotNet.Core.CurlResult.EnsureContains(string)'></a>

## CurlResult\.EnsureContains\(string\) Method


<b>Throw if response body doesn't contain expected text.</b>

Validate response content:

```csharp
// Make sure we got the right response
result.EnsureContains("success");

// Check for error messages
if (result.Body.Contains("error"))
{
    result.EnsureContains("recoverable");  // Make sure it's recoverable
}
```

```csharp
public CurlDotNet.Core.CurlResult EnsureContains(string expectedText);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.EnsureContains(string).expectedText'></a>

`expectedText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Core.CurlResult.EnsureStatus(int)'></a>

## CurlResult\.EnsureStatus\(int\) Method


<b>Throw if status doesn't match what you expect.</b>

Validate specific status codes:

```csharp
// Expect 201 Created
result.EnsureStatus(201);

// Expect 204 No Content
result.EnsureStatus(204);
```

```csharp
public CurlDotNet.Core.CurlResult EnsureStatus(int expectedStatus);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.EnsureStatus(int).expectedStatus'></a>

`expectedStatus` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The status code you expect

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result if status matches \(for chaining\)

#### Exceptions

[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')  
Thrown if status doesn't match

<a name='CurlDotNet.Core.CurlResult.EnsureSuccess()'></a>

## CurlResult\.EnsureSuccess\(\) Method


<b>Throw an exception if the request wasn't successful (not 200-299).</b>

Use this when you expect success and want to fail fast. This matches curl's `-f` (fail) flag behavior.

```csharp
public CurlDotNet.Core.CurlResult EnsureSuccess();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result if successful \(for chaining\)

#### Exceptions

[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')  
Thrown if status is not 200\-299\. The exception contains [StatusCode](CurlDotNet.Core.CurlHttpException.md#CurlDotNet.Core.CurlHttpException.StatusCode 'CurlDotNet\.Core\.CurlHttpException\.StatusCode') and [ResponseBody](CurlDotNet.Core.CurlHttpException.md#CurlDotNet.Core.CurlHttpException.ResponseBody 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody')\.

### Example

```csharp
// Fail fast pattern
try
{
    var data = result
        .EnsureSuccess()      // Throws if not 200-299
        .ParseJson<Data>();  // Only runs if successful
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
    Console.WriteLine($"Response body: {ex.ResponseBody}");
}

// Common API pattern - get user data
var user = (await Curl.ExecuteAsync("curl https://api.example.com/user/123"))
    .EnsureSuccess()        // Throws on 404, 500, etc.
    .ParseJson<User>();    // Safe to parse, we know it's 200

// Chain multiple operations
var response = await Curl.ExecuteAsync("curl https://api.example.com/data");
var processed = response
    .EnsureSuccess()           // Ensure 200-299
    .SaveToFile("backup.json") // Save backup
    .ParseJson<DataModel>(); // Then parse

// Different handling for different status codes
try
{
    result.EnsureSuccess();
    ProcessData(result.Body);
}
catch (CurlHttpException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine("Resource not found");
}
catch (CurlHttpException ex) when (ex.StatusCode >= 500)
{
    Console.WriteLine("Server error - retry later");
}
```

### See Also
- [Ensure specific status code](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.EnsureStatus(int) 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)')
- [Ensure response contains text](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.EnsureContains(string) 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)')
- [Check success without throwing](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.IsSuccess 'CurlDotNet\.Core\.CurlResult\.IsSuccess')

<a name='CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_)'></a>

## CurlResult\.FilterLines\(Func\<string,bool\>\) Method


<b>Extract lines that match a condition.</b>

Filter text responses:

```csharp
// Keep only error lines
result.FilterLines(line => line.Contains("ERROR"));

// Remove empty lines
result.FilterLines(line => !string.IsNullOrWhiteSpace(line));

// Keep lines starting with data
result.FilterLines(line => line.StartsWith("data:"));
```

```csharp
public CurlDotNet.Core.CurlResult FilterLines(System.Func<string,bool> predicate);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.FilterLines(System.Func_string,bool_).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Core.CurlResult.GetHeader(string)'></a>

## CurlResult\.GetHeader\(string\) Method


<b>Get a specific header value (case-insensitive).</b>

Easy header access with null safety. This matches curl's header behavior exactly.

```csharp
public string GetHeader(string headerName);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.GetHeader(string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Name of the header \(case doesn't matter\)

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Header value or null if not found

### Example

```csharp
// Get content type
var contentType = result.GetHeader("Content-Type");
if (contentType?.Contains("json") == true)
{
    var data = result.ParseJson<MyData>();
}

// Check rate limits (common in APIs)
var remaining = result.GetHeader("X-RateLimit-Remaining");
if (remaining != null && int.Parse(remaining) < 10)
{
    Console.WriteLine("⚠️ Only {0} API calls left!", remaining);
}

// Check cache control
var cacheControl = result.GetHeader("Cache-Control");
if (cacheControl?.Contains("no-cache") == true)
{
    Console.WriteLine("Response should not be cached");
}

// Get redirect location
var location = result.GetHeader("Location");
if (location != null)
{
    Console.WriteLine($"Redirected to: {location}");
}
```

### See Also
- [Check if header exists](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.HasHeader(string) 'CurlDotNet\.Core\.CurlResult\.HasHeader\(string\)')
- [Access all headers as dictionary](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Headers 'CurlDotNet\.Core\.CurlResult\.Headers')

<a name='CurlDotNet.Core.CurlResult.HasHeader(string)'></a>

## CurlResult\.HasHeader\(string\) Method


<b>Check if a header exists.</b>

Test for header presence before accessing. This is case-insensitive, matching curl's behavior.

```csharp
public bool HasHeader(string headerName);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.HasHeader(string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Name of the header to check \(case\-insensitive\)

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true if the header exists, false otherwise

### Example

```csharp
// Check for cookies
if (result.HasHeader("Set-Cookie"))
{
    var cookie = result.GetHeader("Set-Cookie");
    Console.WriteLine($"Cookie received: {cookie}");
}

// Check for authentication requirements
if (result.HasHeader("WWW-Authenticate"))
{
    Console.WriteLine("Authentication required");
}

// Check for custom headers
if (result.HasHeader("X-Custom-Header"))
{
    var value = result.GetHeader("X-Custom-Header");
    ProcessCustomValue(value);
}

// Conditional logic based on headers
if (result.HasHeader("Content-Encoding") && 
    result.GetHeader("Content-Encoding").Contains("gzip"))
{
    Console.WriteLine("Response is gzip compressed");
}
```

### See Also
- [Get header value](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.GetHeader(string) 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)')
- [Access all headers](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Headers 'CurlDotNet\.Core\.CurlResult\.Headers')

<a name='CurlDotNet.Core.CurlResult.ParseJson_T_()'></a>

## CurlResult\.ParseJson\<T\>\(\) Method


<b>Parse the JSON response into your C# class.</b>

The most common operation - turning JSON into objects. This method uses [System\.Text\.Json\.JsonSerializer](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer 'System\.Text\.Json\.JsonSerializer') 
             in .NET 6+ and [Newtonsoft\.Json\.JsonConvert](https://learn.microsoft.com/en-us/dotnet/api/newtonsoft.json.jsonconvert 'Newtonsoft\.Json\.JsonConvert') in .NET Standard 2.0 for maximum compatibility.\<example\>
  \<code language="csharp"\>
             // Define your class matching the JSON structure
             public class User
             \{
                 public string Name \{ get; set; \}
                 public string Email \{ get; set; \}
                 public int Id \{ get; set; \}
             \}
            
             var response = await Curl\.ExecuteAsync\("curl https://api\.example\.com/users/42"\);
             var user = response\.ParseJson&lt;User&gt;\(\);
             Console\.WriteLine\($"Hello \{user\.Name\}\!"\);
            
             // Or parse arrays
             var listResponse = await Curl\.ExecuteAsync\("curl https://api\.example\.com/users"\);
             var users = listResponse\.ParseJson&lt;List&lt;User&gt;&gt;\(\);
             Console\.WriteLine\($"Found \{users\.Count\} users"\);
             \</code\>
\</example\>

<b>Tip:</b> Use https://json2csharp.com to generate C# classes from JSON!

```csharp
public T ParseJson<T>();
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.ParseJson_T_().T'></a>

`T`

The type to deserialize to\. Must match the JSON structure\. Can be a class, struct, or collection type like [System\.Collections\.Generic\.List&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1') or [System\.Collections\.Generic\.Dictionary&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')\.

#### Returns
[T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T')  
An instance of [T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T') with data from the JSON [Body](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Body 'CurlDotNet\.Core\.CurlResult\.Body')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
Thrown when [Body](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Body 'CurlDotNet\.Core\.CurlResult\.Body') is null or empty\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
Thrown when JSON deserialization fails or JSON doesn't match type [T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T')\.

[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')  
Thrown when the JSON syntax is invalid\. See [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException') for details\.

### Remarks

This method automatically detects whether to use System.Text.Json or Newtonsoft.Json based on the target framework.

For complex JSON structures, consider using [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AsJsonDynamic() 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)') for exploration, then creating a typed class.

If [T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T') doesn't match the JSON structure, properties that don't match will be left at their default values.

### See Also
- [Alternative method name for parsing JSON](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AsJson_T_() 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)')
- [Parse JSON as dynamic object without a class](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AsJsonDynamic() 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')
- [The JSON string being parsed](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Body 'CurlDotNet\.Core\.CurlResult\.Body')

<a name='CurlDotNet.Core.CurlResult.Print()'></a>

## CurlResult\.Print\(\) Method


<b>Print status code and body to console.</b>

More detailed debug output:

```csharp
result.Print();
// Output:
// Status: 200
// {"name":"John","age":30}
```

```csharp
public CurlDotNet.Core.CurlResult Print();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Core.CurlResult.PrintBody()'></a>

## CurlResult\.PrintBody\(\) Method


<b>Print the response body to console.</b>

Quick debugging output:

```csharp
result.PrintBody();  // Just prints the body

// Chain with other operations
result
    .PrintBody()           // Debug output
    .SaveToFile("out.txt") // Also save it
    .ParseJson<Data>();   // Then parse
```

```csharp
public CurlDotNet.Core.CurlResult PrintBody();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result \(for chaining\)

<a name='CurlDotNet.Core.CurlResult.PrintVerbose()'></a>

## CurlResult\.PrintVerbose\(\) Method


<b>Print everything - status, headers, and body (like curl -v).</b>

Full debug output:

```csharp
result.PrintVerbose();
// Output:
// Status: 200
// Headers:
//   Content-Type: application/json
//   Content-Length: 123
// Body:
// {"name":"John"}
```

```csharp
public CurlDotNet.Core.CurlResult PrintVerbose();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Core.CurlResult.Retry()'></a>

## CurlResult\.Retry\(\) Method


<b>Retry the same curl command again.</b>

Simple retry for transient failures:

```csharp
// First attempt
var result = await Curl.Execute("curl https://flaky-api.example.com");

// Retry if it failed
if (!result.IsSuccess)
{
    result = await result.Retry();
}

// Retry with delay
if (result.StatusCode == 429)  // Too many requests
{
    await Task.Delay(5000);
    result = await result.Retry();
}
```

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> Retry();
```

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
New result from retrying the command

<a name='CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_)'></a>

## CurlResult\.RetryWith\(Action\<CurlSettings\>\) Method


<b>Retry with modifications to the original command.</b>

Retry with different settings:

```csharp
// Retry with longer timeout
var result = await result.RetryWith(settings =>
{
    settings.Timeout = TimeSpan.FromSeconds(60);
});

// Retry with authentication
var result = await result.RetryWith(settings =>
{
    settings.AddHeader("Authorization", "Bearer " + token);
});
```

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> RetryWith(System.Action<CurlDotNet.Core.CurlSettings> configure);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlResult.SaveAsCsv(string)'></a>

## CurlResult\.SaveAsCsv\(string\) Method


<b>Save JSON response as CSV file (for JSON arrays).</b>

Converts JSON arrays to CSV for Excel:

```csharp
// JSON: [{"name":"John","age":30}, {"name":"Jane","age":25}]
result.SaveAsCsv("users.csv");

// Creates CSV:
// name,age
// John,30
// Jane,25

// Open in Excel
Process.Start("users.csv");
```

<b>Note:</b> Only works with JSON arrays of objects.

```csharp
public CurlDotNet.Core.CurlResult SaveAsCsv(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveAsCsv(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Core.CurlResult.SaveAsJson(string,bool)'></a>

## CurlResult\.SaveAsJson\(string, bool\) Method


<b>Save as formatted JSON file (pretty-printed).</b>

Makes JSON human-readable with indentation:

```csharp
// Save with nice formatting
result.SaveAsJson("data.json");           // Pretty-printed
result.SaveAsJson("data.json", false);    // Minified

// Before: {"name":"John","age":30}
// After:  {
//           "name": "John",
//           "age": 30
//         }
```

```csharp
public CurlDotNet.Core.CurlResult SaveAsJson(string filePath, bool indented=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save the JSON file

<a name='CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).indented'></a>

`indented` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

true for pretty formatting \(default\), false for minified

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result \(for chaining\)

<a name='CurlDotNet.Core.CurlResult.SaveToFile(string)'></a>

## CurlResult\.SaveToFile\(string\) Method


<b>Save the response to a file - works for both text and binary!</b>

Smart saving - automatically handles text vs binary:

```csharp
// Save any response
result.SaveToFile("output.txt");     // Text saved as text
result.SaveToFile("image.png");      // Binary saved as binary

// Chain operations (returns this)
result
    .SaveToFile("backup.json")       // Save a backup
    .ParseJson<Data>();              // Then parse it
```

<b>Path examples:</b>

```csharp
result.SaveToFile("file.txt");              // Current directory
result.SaveToFile("data/file.txt");         // Relative path
result.SaveToFile(@"C:\temp\file.txt");     // Absolute path
result.SaveToFile("/home/user/file.txt");   // Linux/Mac path
```

```csharp
public CurlDotNet.Core.CurlResult SaveToFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveToFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save the file

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result \(for chaining\)

### Example

```csharp
// Download and save JSON response
var result = await Curl.ExecuteAsync("curl https://api.example.com/data.json");
result.SaveToFile("data.json");
// File is now saved to disk AND still available in result.Body

// Download image and save
var result = await Curl.ExecuteAsync("curl https://example.com/logo.png");
result.SaveToFile("logo.png");
Console.WriteLine($"Saved {result.BinaryData.Length} bytes");

// Chain with parsing
var result = await Curl.ExecuteAsync("curl https://api.example.com/users");
var users = result
    .SaveToFile("backup-users.json")  // Save backup
    .ParseJson<List<User>>();    // Then parse

// Save with relative path
result.SaveToFile("downloads/report.pdf");

// Save with absolute path
result.SaveToFile(@"C:\Temp\output.txt");  // Windows
result.SaveToFile("/tmp/output.txt");       // Linux/Mac
```

### See Also
- [Async version that doesn't block](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.SaveToFileAsync(string) 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)')
- [Save JSON with formatting](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.SaveAsJson(string,bool) 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)')
- [Append to existing file instead of overwriting](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.AppendToFile(string) 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)')

<a name='CurlDotNet.Core.CurlResult.SaveToFileAsync(string)'></a>

## CurlResult\.SaveToFileAsync\(string\) Method


<b>Save the response to a file asynchronously.</b>

Same as SaveToFile but doesn't block:

```csharp
await result.SaveToFileAsync("large-file.json");

// Or chain async operations
await result
    .SaveToFileAsync("backup.json")
    .ContinueWith(_ => Console.WriteLine("Saved!"));
```

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> SaveToFileAsync(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveToFileAsync(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Core.CurlResult.ToStream()'></a>

## CurlResult\.ToStream\(\) Method


<b>Convert the response to a Stream for reading.</b>

Useful for streaming or processing data:

```csharp
using var stream = result.ToStream();
using var reader = new StreamReader(stream);
var line = await reader.ReadLineAsync();

// Or for binary data
using var stream = result.ToStream();
var buffer = new byte[1024];
var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
```

```csharp
public System.IO.Stream ToStream();
```

#### Returns
[System\.IO\.Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream 'System\.IO\.Stream')  
A MemoryStream containing the response data

<a name='CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_)'></a>

## CurlResult\.Transform\<T\>\(Func\<CurlResult,T\>\) Method


<b>Transform the result using your own function.</b>

Extract or convert data however you need:

```csharp
// Extract just what you need
var name = result.Transform(r =>
{
    var user = r.ParseJson<User>();
    return user.Name;
});

// Convert to your own type
var summary = result.Transform(r => new
{
    Success = r.IsSuccess,
    Size = r.Body?.Length ?? 0,
    Type = r.GetHeader("Content-Type")
});
```

```csharp
public T Transform<T>(System.Func<CurlDotNet.Core.CurlResult,T> transformer);
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T'></a>

`T`
#### Parameters

<a name='CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).transformer'></a>

`transformer` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

#### Returns
[T](CurlDotNet.Core.CurlResult.md#CurlDotNet.Core.CurlResult.Transform_T_(System.Func_CurlDotNet.Core.CurlResult,T_).T 'CurlDotNet\.Core\.CurlResult\.Transform\<T\>\(System\.Func\<CurlDotNet\.Core\.CurlResult,T\>\)\.T')