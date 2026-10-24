#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.DataRaw Property

Sends [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data') literally even when it starts with `@`, instead of reading a file;
curl's `--data-raw`, which is `-d` without the `@file` feature\.

```csharp
public bool DataRaw { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to send a leading `@` as a character\. Default: false\.

### Remarks

Use it when the body really begins with `@`, such as a mention or a decorator name.
            Example: `curl --data-raw "@channel deploy done" URL` is
            `new CurlOptions { Url = url, Method = "POST", Data = "@channel deploy done", DataRaw = true }`; without
            `DataRaw` CurlDotNet would try to read a file called `channel deploy done`.

curl manual: https://curl.se/docs/manpage.html#--data-raw

### See Also
- [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data')