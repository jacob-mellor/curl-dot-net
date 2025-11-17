#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlFileSizeExceededException Class

CURLE\_FILESIZE\_EXCEEDED \(63\) \- File size exceeded

```csharp
public class CurlFileSizeExceededException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFileSizeExceededException
### Constructors

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string)'></a>

## CurlFileSizeExceededException\(long, long, string\) Constructor

Initializes a new instance of the [CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException') class\.

```csharp
public CurlFileSizeExceededException(long maxSize, long actualSize, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).maxSize'></a>

`maxSize` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The maximum allowed file size in bytes\.

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).actualSize'></a>

`actualSize` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The actual file size in bytes that exceeded the limit\.

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.CurlFileSizeExceededException(long,long,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.ActualSize'></a>

## CurlFileSizeExceededException\.ActualSize Property

Gets the actual file size in bytes that exceeded the limit

```csharp
public long ActualSize { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='CurlDotNet.Exceptions.CurlFileSizeExceededException.MaxSize'></a>

## CurlFileSizeExceededException\.MaxSize Property

Gets the maximum allowed file size in bytes

```csharp
public long MaxSize { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')