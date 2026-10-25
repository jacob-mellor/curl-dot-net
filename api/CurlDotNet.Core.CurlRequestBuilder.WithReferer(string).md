#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithReferer\(string\) Method

Sets the `Referer` header, the page the request claims to come from \(curl's `-e` /
`--referer`\); some sites require it for downloads or hot\-link protection\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithReferer(string referer);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithReferer(string).referer'></a>

`referer` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The referring URL; null or empty sends no Referer header\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Pass a full URL such as `"https://www.example.com/products"`. curl's `-e ';auto'`
            (update the referer on redirects) is not available here; use the command string for that.

Example: `CurlRequestBuilder.Get(imageUrl).WithReferer("https://www.example.com/gallery").SaveToFile("photo.jpg")`
            is `curl -e 'https://www.example.com/gallery' -o photo.jpg IMAGEURL`.

### See Also
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')
- [Referer](CurlDotNet.Core.CurlOptions.Referer.md 'CurlDotNet\.Core\.CurlOptions\.Referer')