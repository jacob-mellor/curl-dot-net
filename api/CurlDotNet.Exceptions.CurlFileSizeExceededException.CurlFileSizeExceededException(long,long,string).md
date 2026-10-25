#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException')

## CurlFileSizeExceededException\(long, long, string\) Constructor

Creates the error \(exit code 63\) with curl's wording `Exceeded the maximum allowed file size (MAX)` and the
actual size\.

```csharp
public CurlFileSizeExceededException(long maxSize, long actualSize, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).maxSize'></a>

`maxSize` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The maximum allowed file size in bytes \(the \-\-max\-filesize value\)\.

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).actualSize'></a>

`actualSize` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The actual or announced size in bytes that exceeded the limit\.

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Example: `throw new CurlFileSizeExceededException(10_000_000, 52_428_800, command);`

### See Also
- [CurlOutOfMemoryException](CurlDotNet.Exceptions.CurlOutOfMemoryException.md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException')