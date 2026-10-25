#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlOutOfMemoryException](CurlDotNet.Exceptions.CurlOutOfMemoryException.md 'CurlDotNet\.Exceptions\.CurlOutOfMemoryException')

## CurlOutOfMemoryException\(string\) Constructor

Creates the out\-of\-memory error \(exit code 27\) with advice to stream large responses to a file instead of
memory\.

```csharp
public CurlOutOfMemoryException(string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOutOfMemoryException.CurlOutOfMemoryException(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Out of memory`, the advice to use `-o` and `--max-filesize`, and the download guide.

### See Also
- [CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException')