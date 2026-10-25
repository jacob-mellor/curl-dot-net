#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')

## CurlRangeErrorException\(long, int, string\) Constructor

Creates the range error \(exit code 33\) with curl's wording
`HTTP server doesn't seem to support byte ranges. Cannot resume.` and the offset and status involved\.

```csharp
public CurlRangeErrorException(long offset, int statusCode, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRangeErrorException.CurlRangeErrorException(long,int,string).offset'></a>

`offset` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The byte offset the transfer tried to resume from\.

<a name='CurlDotNet.Exceptions.CurlRangeErrorException.CurlRangeErrorException(long,int,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status the server answered with \(usually 200 instead of 206\)\.

<a name='CurlDotNet.Exceptions.CurlRangeErrorException.CurlRangeErrorException(long,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

Example: `throw new CurlRangeErrorException(1048576, 200, command);`

### See Also
- [CurlBadDownloadResumeException](CurlDotNet.Exceptions.CurlBadDownloadResumeException.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException')