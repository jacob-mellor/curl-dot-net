#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Put\(string\) Method

Starts a PUT request \(`curl -X PUT URL`\), usually to replace a resource; add the body with
[WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)'), [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)') or [WithBinaryData\(byte\[\]\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryData\(byte\[\]\)')\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Put(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Put(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL of the resource, such as `https://api.example.com/users/42`\. Must not be null\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder whose method is PUT\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Core.CurlRequestBuilder.Put(string).md#CurlDotNet.Core.CurlRequestBuilder.Put(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Put\(string\)\.url') is null\. Pass the URL string; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Remarks

Unlike a GET, adding a body keeps the method PUT. To upload a file's raw bytes as the PUT
            body (curl's `-T file`), read it with `File.ReadAllBytes` and pass it to
            [WithBinaryData\(byte\[\]\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryData\(byte\[\]\)').

Example: `await CurlRequestBuilder.Put("https://api.example.com/users/42").WithJson(new { name = "Jane" }).WithBearerToken(token).ExecuteAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [PutAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PutAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')
- [Patch\(string\)](CurlDotNet.Core.CurlRequestBuilder.Patch(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Patch\(string\)')