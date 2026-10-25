#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Delete\(string\) Method

Starts a DELETE request \(`curl -X DELETE URL`\) to remove the resource at
[url](CurlDotNet.Core.CurlRequestBuilder.Delete(string).md#CurlDotNet.Core.CurlRequestBuilder.Delete(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Delete\(string\)\.url'); a body is optional\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Delete(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Delete(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL of the resource to delete\. Must not be null\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder whose method is DELETE\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Core.CurlRequestBuilder.Delete(string).md#CurlDotNet.Core.CurlRequestBuilder.Delete(string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Delete\(string\)\.url') is null\. Pass the URL string; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Remarks

Most APIs answer 200, 202 or 204 (no body). Add [FailOnError\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FailOnError(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FailOnError\(bool\)') to turn a 404 or 403
            into an exception instead of a result with that status code.

Example: `var r = await CurlRequestBuilder.Delete("https://api.example.com/users/42").WithBearerToken(token).ExecuteAsync();`
            then `if (r.StatusCode == 204) Console.WriteLine("deleted");`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [DeleteAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.DeleteAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')