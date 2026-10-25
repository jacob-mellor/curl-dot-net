#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadDownloadResumeException](CurlDotNet.Exceptions.CurlBadDownloadResumeException.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException')

## CurlBadDownloadResumeException\(long, string\) Constructor

Creates the resume error \(exit code 36\) for [offset](CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).md#CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).offset 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException\.CurlBadDownloadResumeException\(long, string\)\.offset'), with advice to restart the download\.

```csharp
public CurlBadDownloadResumeException(long offset, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).offset'></a>

`offset` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The byte offset where the resume was attempted\.

<a name='CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Bad download resume at offset N`. Example: `throw new CurlBadDownloadResumeException(1048576, command);`

### See Also
- [CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')