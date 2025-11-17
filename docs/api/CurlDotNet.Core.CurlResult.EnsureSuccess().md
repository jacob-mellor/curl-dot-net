#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

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
Thrown if status is not 200\-299\. The exception contains [StatusCode](CurlDotNet.Core.CurlHttpException.StatusCode.md 'CurlDotNet\.Core\.CurlHttpException\.StatusCode') and [ResponseBody](CurlDotNet.Core.CurlHttpException.ResponseBody.md 'CurlDotNet\.Core\.CurlHttpException\.ResponseBody')\.

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
- [Ensure specific status code](CurlDotNet.Core.CurlResult.EnsureStatus(int).md 'CurlDotNet\.Core\.CurlResult\.EnsureStatus\(int\)')
- [Ensure response contains text](CurlDotNet.Core.CurlResult.EnsureContains(string).md 'CurlDotNet\.Core\.CurlResult\.EnsureContains\(string\)')
- [Check success without throwing](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess')