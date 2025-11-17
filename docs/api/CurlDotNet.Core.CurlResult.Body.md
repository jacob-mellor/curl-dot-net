#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

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

<b>Note:</b> For binary data (images, PDFs), use [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') instead.

<b>Note:</b> Can be null for 204 No Content or binary responses.

```csharp
public string Body { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')