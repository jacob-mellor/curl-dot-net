#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TimeCondition Property

The date for a conditional request, like curl's `-z`/`--time-cond`: the document is only downloaded
when it changed after \(or before\) this date; null for no condition\.

```csharp
public System.Nullable<System.DateTimeOffset> TimeCondition { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset 'System\.DateTimeOffset')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
A date and time \(sent as RFC 1123 GMT\)\. Default: null\.

### Remarks

Sent as `If-Modified-Since` or `If-Unmodified-Since` depending on [TimeConditionType](CurlDotNet.Core.CurlOptions.TimeConditionType.md 'CurlDotNet\.Core\.CurlOptions\.TimeConditionType'). The
            parser accepts the date formats curl's `curl_getdate` does (`"Sun, 12 Dec 1999 12:00:00 GMT"`,
            `"dec 12 12:00:00 1999 GMT"`, `"19991212"`, `"12 Dec 1999"`, ...) or the name of a local file, in which case
            the file's last-modified time is used. A leading `-` asks for "unmodified since", a leading `=` for an exact
            Last-Modified match. When the condition is not met the body is dropped and [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') is 304.

Example: `curl -z "Sun, 12 Dec 1999 12:00:00 GMT" https://example.com/feed.xml` is
            `new CurlOptions { Url = url, TimeCondition = new DateTimeOffset(1999, 12, 12, 12, 0, 0, TimeSpan.Zero) }`.

### See Also
- [TimeConditionType](CurlDotNet.Core.CurlOptions.TimeConditionType.md 'CurlDotNet\.Core\.CurlOptions\.TimeConditionType')
- [CurlTimeCondition](CurlDotNet.Core.CurlTimeCondition.md 'CurlDotNet\.Core\.CurlTimeCondition')