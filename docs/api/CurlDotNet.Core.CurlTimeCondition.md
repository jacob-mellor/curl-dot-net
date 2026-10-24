#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlTimeCondition Enum

The comparison a conditional request makes against the server's Last\-Modified time, set with
[TimeConditionType](CurlDotNet.Core.CurlOptions.TimeConditionType.md 'CurlDotNet\.Core\.CurlOptions\.TimeConditionType') or curl's `-z DATE`, `-z -DATE` and `-z =DATE`\.

```csharp
public enum CurlTimeCondition
```
### Fields

<a name='CurlDotNet.Core.CurlTimeCondition.IfModifiedSince'></a>

`IfModifiedSince` 0

Only fetch the document if it changed after the date: sends `If-Modified-Since` \(curl's `-z DATE`\);
the default, used to refresh a cached copy\.

### Remarks

Example: `TimeConditionType = CurlTimeCondition.IfModifiedSince` with yesterday's date downloads
            only a newer file; otherwise the result is a 304 with no body.

### See Also
- [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition')

<a name='CurlDotNet.Core.CurlTimeCondition.IfUnmodifiedSince'></a>

`IfUnmodifiedSince` 1

Only fetch the document if it did not change after the date: sends `If-Unmodified-Since` \(curl's
`-z -DATE`\); useful to make sure a resource is still the version you saw\.

### Remarks

Example: `curl -z "-Jan 1 2024" URL` is `TimeConditionType = CurlTimeCondition.IfUnmodifiedSince`.

### See Also
- [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition')

<a name='CurlDotNet.Core.CurlTimeCondition.LastModified'></a>

`LastModified` 2

curl's `-z =DATE`: sends `Last-Modified: DATE` and, like curl, only keeps a document whose Last\-Modified
is newer than the date \(curl applies the "newer" test to this mode too\)\.

### Remarks

Example: `curl -z "=Jan 1 2024" URL` is `TimeConditionType = CurlTimeCondition.LastModified`.

### See Also
- [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition')

### Remarks

Example: set [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition') to the date and [TimeConditionType](CurlDotNet.Core.CurlOptions.TimeConditionType.md 'CurlDotNet\.Core\.CurlOptions\.TimeConditionType') to
            `CurlTimeCondition.IfUnmodifiedSince`, or write it curl's way: `curl -z -DATE URL`. A condition that is not met
            gives [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') 304 and an empty body.

### See Also
- [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition')
- [TimeConditionType](CurlDotNet.Core.CurlOptions.TimeConditionType.md 'CurlDotNet\.Core\.CurlOptions\.TimeConditionType')