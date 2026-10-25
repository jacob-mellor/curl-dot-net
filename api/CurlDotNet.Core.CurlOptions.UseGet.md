#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UseGet Property

Sends the `-d`/`--data*` data as a query string on a GET request instead of a POST body, like
curl's `-G`/`--get`; handy for search forms and APIs with many query parameters\.

```csharp
public bool UseGet { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to move the data into the URL\. Default: false\.

### Remarks

The parser applies this itself: the joined data is appended to [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') after a `?` (or `&`
            when the URL already has a query), [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data') is cleared and the method stays GET (HEAD when `-I` is also
            given). Setting the property on hand-built options has the same effect at send time.

Example: `curl -G -d q=dotnet -d page=2 https://example.com/search` requests
            `GET /search?q=dotnet&page=2`; in C#, `new CurlOptions { Url = "https://example.com/search", Data = "q=dotnet&page=2", UseGet = true }`.

### See Also
- [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data')
- [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url')