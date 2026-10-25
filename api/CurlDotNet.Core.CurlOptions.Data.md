#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Data Property

The request body text, as given with curl's `-d`/`--data`; a leading `@` names a file whose
bytes are sent instead, unless [DataRaw](CurlDotNet.Core.CurlOptions.DataRaw.md 'CurlDotNet\.Core\.CurlOptions\.DataRaw') is true\.

```csharp
public string? Data { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The body text, sent as UTF\-8\. Default: null \(no body\)\.

### Remarks

Without a `Content-Type` entry in [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers') the body is labelled
            `application/x-www-form-urlencoded`, like curl's `-d`; add `Headers["Content-Type"] = "application/json"`
            for JSON. For hand-built options, set [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method') to POST/PUT/PATCH: a GET or HEAD sends no body.
            `Data = "@payload.json"` reads that file as raw bytes; a missing file fails with
            [CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException') (exit 37).

For parsed commands, several `-d` values are joined with `&` and the exact bytes curl would send
            (including `--data-binary` and `--json` parts) are kept internally; editing this property afterwards
            replaces them with your text.

Example: `curl -d '{"name":"Ada"}' -H "Content-Type: application/json" https://api.example.com/users` is
            `new CurlOptions { Url = url, Method = "POST", Data = "{\"name\":\"Ada\"}", Headers = { ["Content-Type"] = "application/json" } }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [DataRaw](CurlDotNet.Core.CurlOptions.DataRaw.md 'CurlDotNet\.Core\.CurlOptions\.DataRaw')
- [BinaryData](CurlDotNet.Core.CurlOptions.BinaryData.md 'CurlDotNet\.Core\.CurlOptions\.BinaryData')
- [UseGet](CurlDotNet.Core.CurlOptions.UseGet.md 'CurlDotNet\.Core\.CurlOptions\.UseGet')
- [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)')
- [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)')