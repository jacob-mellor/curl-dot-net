#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlBadContentEncodingException Class

CURLE\_BAD\_CONTENT\_ENCODING \(61\) \- Unrecognized content encoding

```csharp
public class CurlBadContentEncodingException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlBadContentEncodingException
### Constructors

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string)'></a>

## CurlBadContentEncodingException\(string, string\) Constructor

Initializes a new instance of the [CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException') class\.

```csharp
public CurlBadContentEncodingException(string encoding, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string).encoding'></a>

`encoding` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The content encoding that was not recognized or could not be decoded\.

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlBadContentEncodingException.Encoding'></a>

## CurlBadContentEncodingException\.Encoding Property

Gets the content encoding that was not recognized or could not be decoded

```csharp
public string Encoding { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')