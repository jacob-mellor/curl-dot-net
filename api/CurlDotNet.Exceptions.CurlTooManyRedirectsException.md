#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlTooManyRedirectsException Class

CURLE\_TOO\_MANY\_REDIRECTS \(47\) \- Too many redirects

```csharp
public class CurlTooManyRedirectsException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlTooManyRedirectsException
### Constructors

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string)'></a>

## CurlTooManyRedirectsException\(int, string\) Constructor

Initializes a new instance of the [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') class\.

```csharp
public CurlTooManyRedirectsException(int count, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of redirects that were attempted before failing\.

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.RedirectCount'></a>

## CurlTooManyRedirectsException\.RedirectCount Property

Gets the number of redirects that were attempted before failing

```csharp
public int RedirectCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')