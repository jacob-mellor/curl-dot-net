#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

## CurlPartialFileException\.BytesReceived Property

Body bytes that arrived before the connection closed; also the resume offset for `-C -` when they were
written to a file\.

```csharp
public long BytesReceived { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
A byte count, 0 or more\.

### Remarks

Example: `Console.WriteLine($"{100.0 * ex.BytesReceived / ex.BytesExpected:F0}% received");`

### See Also
- [BytesExpected](CurlDotNet.Exceptions.CurlPartialFileException.BytesExpected.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.BytesExpected')