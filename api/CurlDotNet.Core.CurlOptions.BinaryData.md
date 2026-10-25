#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.BinaryData Property

Raw bytes sent as the request body without any text encoding, for images, archives and protobuf;
also where the bytes of a `-T`/`--upload-file` file end up after parsing\.

```csharp
public byte[]? BinaryData { get; set; }
```

#### Property Value
[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')  
The exact body bytes\. Default: null\.

### Remarks

Used when [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data') is not the body source: with [UploadFile](CurlDotNet.Core.CurlOptions.UploadFile.md 'CurlDotNet\.Core\.CurlOptions\.UploadFile') it is the uploaded file,
            otherwise it is sent as `ByteArrayContent` on a POST/PUT/PATCH (set [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method'); GET and HEAD send
            no body). No Content-Type is added: set `Headers["Content-Type"]` (for example `application/octet-stream`).
            Multipart fields ([FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData'), [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files'), [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts')) take precedence over it.

Example: `curl --data-binary @photo.jpg -H "Content-Type: image/jpeg" URL` is
            `new CurlOptions { Url = url, Method = "POST", BinaryData = File.ReadAllBytes("photo.jpg"), Headers = { ["Content-Type"] = "image/jpeg" } }`.

### See Also
- [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data')
- [UploadFile](CurlDotNet.Core.CurlOptions.UploadFile.md 'CurlDotNet\.Core\.CurlOptions\.UploadFile')
- [WithBinaryData\(byte\[\]\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryData\(byte\[\]\)')