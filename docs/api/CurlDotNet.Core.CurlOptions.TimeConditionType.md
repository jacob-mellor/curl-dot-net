#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TimeConditionType Property

Which comparison [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition') requests, like curl's `-z DATE` \(modified since\),
`-z -DATE` \(unmodified since\) and `-z =DATE` \(Last\-Modified match\)\.

```csharp
public CurlDotNet.Core.CurlTimeCondition TimeConditionType { get; set; }
```

#### Property Value
[CurlTimeCondition](CurlDotNet.Core.CurlTimeCondition.md 'CurlDotNet\.Core\.CurlTimeCondition')  
A [CurlTimeCondition](CurlDotNet.Core.CurlTimeCondition.md 'CurlDotNet\.Core\.CurlTimeCondition')\. Default: [IfModifiedSince](CurlDotNet.Core.CurlTimeCondition.md#CurlDotNet.Core.CurlTimeCondition.IfModifiedSince 'CurlDotNet\.Core\.CurlTimeCondition\.IfModifiedSince'); ignored while
            [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition') is null\.

### Remarks

Example: `curl -z "-Jan 1 2024" https://example.com/` sends `If-Unmodified-Since: Mon, 01 Jan 2024 00:00:00 GMT`;
            in C#, `TimeCondition = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TimeConditionType = CurlTimeCondition.IfUnmodifiedSince`.

### See Also
- [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition')