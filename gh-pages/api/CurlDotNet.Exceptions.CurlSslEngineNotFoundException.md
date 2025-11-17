#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlSslEngineNotFoundException Class

CURLE\_SSL\_ENGINE\_NOTFOUND \(53\) \- SSL engine not found

```csharp
public class CurlSslEngineNotFoundException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslEngineNotFoundException
### Constructors

<a name='CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string)'></a>

## CurlSslEngineNotFoundException\(string, string\) Constructor

Initializes a new instance of the [CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException') class\.

```csharp
public CurlSslEngineNotFoundException(string engine, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string).engine'></a>

`engine` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The SSL engine that was not found\.

<a name='CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.