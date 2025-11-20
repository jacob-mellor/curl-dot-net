#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlUnsupportedProtocolException Class

CURLE\_UNSUPPORTED\_PROTOCOL \(1\) \- Unsupported protocol

```csharp
public class CurlUnsupportedProtocolException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlUnsupportedProtocolException
### Constructors

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string)'></a>

## CurlUnsupportedProtocolException\(string, string\) Constructor

Initializes a new instance of the [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException') class\.

```csharp
public CurlUnsupportedProtocolException(string protocol, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string).protocol'></a>

`protocol` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The protocol that is not supported \(e\.g\., "gopher", "telnet"\)\.

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.
### Properties

<a name='CurlDotNet.Exceptions.CurlUnsupportedProtocolException.Protocol'></a>

## CurlUnsupportedProtocolException\.Protocol Property

Gets the unsupported protocol that was attempted

```csharp
public string Protocol { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')