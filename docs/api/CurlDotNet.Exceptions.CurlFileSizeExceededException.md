#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFileSizeExceededException Class

CURLE\_FILESIZE\_EXCEEDED \(63\) \- File size exceeded

```csharp
public class CurlFileSizeExceededException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFileSizeExceededException

| Constructors | |
| :--- | :--- |
| [CurlFileSizeExceededException\(long, long, string\)](CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.CurlFileSizeExceededException\(long, long, string\)') | Initializes a new instance of the [CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException') class\. |

| Properties | |
| :--- | :--- |
| [ActualSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.ActualSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.ActualSize') | Gets the actual file size in bytes that exceeded the limit |
| [MaxSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.MaxSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.MaxSize') | Gets the maximum allowed file size in bytes |
