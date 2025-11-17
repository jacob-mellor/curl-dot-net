#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlBadDownloadResumeException Class

CURLE\_BAD\_DOWNLOAD\_RESUME \(36\) \- Bad download resume

```csharp
public class CurlBadDownloadResumeException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlBadDownloadResumeException
### Constructors

<a name='CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string)'></a>

## CurlBadDownloadResumeException\(long, string\) Constructor

Initializes a new instance of the [CurlBadDownloadResumeException](CurlDotNet.Exceptions.CurlBadDownloadResumeException.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException') class\.

```csharp
public CurlBadDownloadResumeException(long offset, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).offset'></a>

`offset` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The offset where the bad download resume occurred\.

<a name='CurlDotNet.Exceptions.CurlBadDownloadResumeException.CurlBadDownloadResumeException(long,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlBadDownloadResumeException.ResumeOffset'></a>

## CurlBadDownloadResumeException\.ResumeOffset Property

Gets the offset where the bad download resume occurred

```csharp
public long ResumeOffset { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')