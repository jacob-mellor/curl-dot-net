#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlBadFunctionArgumentException Class

CURLE\_BAD\_FUNCTION\_ARGUMENT \(43\) \- Bad function argument

```csharp
public class CurlBadFunctionArgumentException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlBadFunctionArgumentException
### Constructors

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string)'></a>

## CurlBadFunctionArgumentException\(string, string\) Constructor

Initializes a new instance of the [CurlBadFunctionArgumentException](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException') class\.

```csharp
public CurlBadFunctionArgumentException(string argumentName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string).argumentName'></a>

`argumentName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the argument that was invalid\.

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.ArgumentName'></a>

## CurlBadFunctionArgumentException\.ArgumentName Property

Gets the name of the argument that was invalid

```csharp
public string ArgumentName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')