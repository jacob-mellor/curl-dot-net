#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.DataUrlEncode Property

Records that the body came from curl's `--data-urlencode`; the parser has already percent\-encoded the
value into [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data'), so setting this yourself changes nothing on the wire\.

```csharp
public bool DataUrlEncode { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True after parsing a command that used `--data-urlencode`\. Default: false\.

### Remarks

curl's forms are `content`, `=content`, `name=content`, `@file` and `name@file`;
            the content is encoded with `%XX` escapes and joined to other `-d` parts with `&`.
            For hand-built options encode the value yourself, for example
            `Data = "q=" + Uri.EscapeDataString("C# & .NET")`, which is what
            `curl --data-urlencode "q=C# & .NET" URL` sends.

### See Also
- [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data')
- [UseGet](CurlDotNet.Core.CurlOptions.UseGet.md 'CurlDotNet\.Core\.CurlOptions\.UseGet')