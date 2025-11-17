#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

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
- [Async version that doesn't block](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)')
- [Save JSON with formatting](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)')
- [Append to existing file instead of overwriting](CurlDotNet.Core.CurlResult.AppendToFile(string).md 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)')