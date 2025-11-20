#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlSslCipherException Class

CURLE\_SSL\_CIPHER \(59\) \- Couldn't use SSL cipher

```csharp
public class CurlSslCipherException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslCipherException
### Constructors

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string)'></a>

## CurlSslCipherException\(string, string\) Constructor

Initializes a new instance of the [CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException') class\.

```csharp
public CurlSslCipherException(string cipher, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string).cipher'></a>

`cipher` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the SSL cipher that could not be used\.

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlSslCipherException.CipherName'></a>

## CurlSslCipherException\.CipherName Property

Gets the name of the SSL cipher that could not be used

```csharp
public string CipherName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')