#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.BinaryData Property

The response body as raw bytes when it is binary \(image, PDF, zip, Office file, or anything that is not
text\); null for text responses, whose content is in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

```csharp
public byte[] BinaryData { get; set; }
```

#### Property Value
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')  
The bytes exactly as received \(after `--compressed` decoding\) for a binary response, or null for a
            text response\. A binary body written to a file with `-o` is kept here too; a text body written with
            `-o` is in neither [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') nor this property, only in the file\.

### Remarks

A response is binary when its `Content-Type` is not a text type, or when the bytes look binary even
            under a text type (a server sending an .xlsx as `text/plain`). Force it with
            [AsBinary\(\)](CurlDotNet.Core.CurlRequestBuilder.AsBinary().md 'CurlDotNet\.Core\.CurlRequestBuilder\.AsBinary\(\)'), or register types with [WithBinaryContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryContentType\(string\)').

Example: `var r = await Curl.ExecuteAsync("curl https://example.com/logo.png"); if (r.IsBinary) File.WriteAllBytes("logo.png", r.BinaryData);`
            - or simply `r.SaveToFile("logo.png")`, which picks bytes or text for you.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [IsBinary](CurlDotNet.Core.CurlResult.IsBinary.md 'CurlDotNet\.Core\.CurlResult\.IsBinary')
- [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
- [ToStream\(\)](CurlDotNet.Core.CurlResult.ToStream().md 'CurlDotNet\.Core\.CurlResult\.ToStream\(\)')