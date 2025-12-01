#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

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