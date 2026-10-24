#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

## CurlPartialFileException\.BytesExpected Property

Body bytes the server announced with Content\-Length, or null for chunked bodies, where the total is unknown\.

```csharp
public System.Nullable<long> BytesExpected { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
A byte count, or null\.

### Remarks

`BytesExpected - BytesReceived` is the number of missing bytes.

### See Also
- [BytesReceived](CurlDotNet.Exceptions.CurlPartialFileException.BytesReceived.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.BytesReceived')