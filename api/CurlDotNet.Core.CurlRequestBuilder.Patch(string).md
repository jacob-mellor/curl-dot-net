#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Patch\(string\) Method

Starts a PATCH request \(`curl -X PATCH URL`\) for partial updates; add the changed fields with
[WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)') or [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)')\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Patch(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Patch(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL of the resource to update\. Must not be null\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder whose method is PATCH\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Core.CurlRequestBuilder.Patch(string).md#CurlDotNet.Core.CurlRequestBuilder.Patch(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Patch\(string\)\.url') is null\. Pass the URL string; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Remarks

For JSON Merge Patch or JSON Patch set the media type yourself, for example
            `.WithData(patchJson).WithHeader("Content-Type", "application/merge-patch+json")`.

Example: `await CurlRequestBuilder.Patch("https://api.example.com/users/42").WithJson(new { email = "new@example.com" }).ExecuteAsync();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [PatchAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PatchAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')
- [Put\(string\)](CurlDotNet.Core.CurlRequestBuilder.Put(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Put\(string\)')