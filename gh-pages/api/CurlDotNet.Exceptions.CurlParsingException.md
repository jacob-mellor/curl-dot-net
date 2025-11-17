#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlParsingException Class

Thrown when content parsing fails

```csharp
public class CurlParsingException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlParsingException
### Constructors

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception)'></a>

## CurlParsingException\(string, string, Type, string, Exception\) Constructor

Initializes a new instance of the [CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException') class

```csharp
public CurlParsingException(string message, string contentType, System.Type expectedType, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the parsing failure

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).contentType'></a>

`contentType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The content type of the unparseable data

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).expectedType'></a>

`expectedType` [System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')

The type that was expected

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed

<a name='CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The underlying parsing exception
### Properties

<a name='CurlDotNet.Exceptions.CurlParsingException.ContentType'></a>

## CurlParsingException\.ContentType Property

Gets the content type of the data that failed to parse

```csharp
public string ContentType { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Exceptions.CurlParsingException.ExpectedType'></a>

## CurlParsingException\.ExpectedType Property

Gets the expected type that the content could not be parsed into

```csharp
public System.Type ExpectedType { get; }
```

#### Property Value
[System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')