#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

## CurlFtpException Class

FTP\-specific exception\.

```csharp
public class CurlFtpException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFtpException
### Constructors

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int)'></a>

## CurlFtpException\(string, int\) Constructor

Initializes a new instance of the CurlFtpException class\.

```csharp
public CurlFtpException(string message, int ftpStatusCode);
```
#### Parameters

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int).ftpStatusCode'></a>

`ftpStatusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The FTP status code\.
### Properties

<a name='CurlDotNet.Core.CurlFtpException.FtpStatusCode'></a>

## CurlFtpException\.FtpStatusCode Property

Gets the FTP status code\.

```csharp
public int FtpStatusCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')