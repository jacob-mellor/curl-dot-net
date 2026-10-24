#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.DataRaw Property

Whether the data was provided via \-\-data\-raw, which prevents @file expansion\.
When true, a leading @ in [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data') is treated as a literal character
rather than a file reference\. This matches curl's \-\-data\-raw behavior\.

```csharp
public bool DataRaw { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

### Remarks

In curl, --data-raw is identical to --data except that @ has no special meaning.
            This is useful when the data literally starts with @ and should not be interpreted
            as a file reference.

See: https://curl.se/docs/manpage.html#--data-raw