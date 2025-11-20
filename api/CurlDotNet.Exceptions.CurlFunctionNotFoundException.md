#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlFunctionNotFoundException Class

CURLE\_FUNCTION\_NOT\_FOUND \(41\) \- Function not found

```csharp
public class CurlFunctionNotFoundException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFunctionNotFoundException
### Constructors

<a name='CurlDotNet.Exceptions.CurlFunctionNotFoundException.CurlFunctionNotFoundException(string,string)'></a>

## CurlFunctionNotFoundException\(string, string\) Constructor

Initializes a new instance of the [CurlFunctionNotFoundException](CurlDotNet.Exceptions.CurlFunctionNotFoundException.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException') class\.

```csharp
public CurlFunctionNotFoundException(string functionName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFunctionNotFoundException.CurlFunctionNotFoundException(string,string).functionName'></a>

`functionName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The function name that was not found\.

<a name='CurlDotNet.Exceptions.CurlFunctionNotFoundException.CurlFunctionNotFoundException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.
### Properties

<a name='CurlDotNet.Exceptions.CurlFunctionNotFoundException.FunctionName'></a>

## CurlFunctionNotFoundException\.FunctionName Property

Gets the function name that was not found

```csharp
public string FunctionName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')