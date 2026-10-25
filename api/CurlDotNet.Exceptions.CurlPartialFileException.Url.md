#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

## CurlPartialFileException\.Url Property

The URL whose body was cut short, as requested \(it may contain credentials; the message shows a safe
version\)\.

```csharp
public string Url { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An absolute URL\.

### Remarks

Use it to resume: `curl -C - -o FILE {Url}`.

### See Also
- [BytesReceived](CurlDotNet.Exceptions.CurlPartialFileException.BytesReceived.md 'CurlDotNet\.Exceptions\.CurlPartialFileException\.BytesReceived')