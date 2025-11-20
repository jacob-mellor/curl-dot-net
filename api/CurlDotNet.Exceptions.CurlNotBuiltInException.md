#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlNotBuiltInException Class

CURLE\_NOT\_BUILT\_IN \(4\) \- Feature not built in

```csharp
public class CurlNotBuiltInException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlNotBuiltInException
### Constructors

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string)'></a>

## CurlNotBuiltInException\(string, string\) Constructor

Initializes a new instance of the [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException') class\.

```csharp
public CurlNotBuiltInException(string feature, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).feature'></a>

`feature` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The feature that is not available in this build\.

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.
### Properties

<a name='CurlDotNet.Exceptions.CurlNotBuiltInException.Feature'></a>

## CurlNotBuiltInException\.Feature Property

Gets the feature that is not available

```csharp
public string Feature { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')