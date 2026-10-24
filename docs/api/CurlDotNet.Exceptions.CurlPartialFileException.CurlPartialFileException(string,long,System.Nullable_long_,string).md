#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

## CurlPartialFileException\(string, long, Nullable\<long\>, string\) Constructor

Creates the partial\-transfer error \(exit code 18\) with curl's wording
`transfer closed with N bytes remaining to read` and advice to retry or resume\.

```csharp
public CurlPartialFileException(string url, long bytesReceived, System.Nullable<long> bytesExpected, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlPartialFileException.CurlPartialFileException(string,long,System.Nullable_long_,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL whose body was cut short\.

<a name='CurlDotNet.Exceptions.CurlPartialFileException.CurlPartialFileException(string,long,System.Nullable_long_,string).bytesReceived'></a>

`bytesReceived` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

Body bytes that arrived before the connection closed\.

<a name='CurlDotNet.Exceptions.CurlPartialFileException.CurlPartialFileException(string,long,System.Nullable_long_,string).bytesExpected'></a>

`bytesExpected` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

Body bytes the server announced with Content\-Length, or null for a chunked body\.

<a name='CurlDotNet.Exceptions.CurlPartialFileException.CurlPartialFileException(string,long,System.Nullable_long_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

For a chunked body ([bytesExpected](CurlDotNet.Exceptions.CurlPartialFileException.CurlPartialFileException(string,long,System.Nullable_long_,string).md#CurlDotNet.Exceptions.CurlPartialFileException.CurlPartialFileException(string,long,System.Nullable_long_,string).bytesExpected 'CurlDotNet\.Exceptions\.CurlPartialFileException\.CurlPartialFileException\(string, long, System\.Nullable\<long\>, string\)\.bytesExpected') null) the wording is
            `transfer closed with outstanding read data remaining`. The URL is printed without credentials or query
            string. Example: `throw new CurlPartialFileException(url, 512, 1024, command);`

### See Also
- [CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')