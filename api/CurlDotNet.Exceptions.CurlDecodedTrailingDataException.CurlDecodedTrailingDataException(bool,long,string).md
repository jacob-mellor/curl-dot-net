#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlDecodedTrailingDataException](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException')

## CurlDecodedTrailingDataException\(bool, long, string\) Constructor

Creates the error \(exit code 23\) for [trailingBytes](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).md#CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).trailingBytes 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.CurlDecodedTrailingDataException\(bool, long, string\)\.trailingBytes') bytes found after the end of a gzip
member in a body decoded for `--compressed`\.

```csharp
public CurlDecodedTrailingDataException(bool anotherGzipMember, long trailingBytes, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).anotherGzipMember'></a>

`anotherGzipMember` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True when the bytes after the first member start a second gzip member \(`1f 8b`\)\.

<a name='CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).trailingBytes'></a>

`trailingBytes` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

How many bytes followed the end of the first member\.

<a name='CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

The message is curl's own text: `Multi-member gzip response not supported` when
            [anotherGzipMember](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).md#CurlDotNet.Exceptions.CurlDecodedTrailingDataException.CurlDecodedTrailingDataException(bool,long,string).anotherGzipMember 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.CurlDecodedTrailingDataException\(bool, long, string\)\.anotherGzipMember') is true, otherwise `Failed writing received data to disk/application`.
            Example: `throw new CurlDecodedTrailingDataException(true, 27, command);`

### See Also
- [AnotherGzipMember](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.AnotherGzipMember.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.AnotherGzipMember')