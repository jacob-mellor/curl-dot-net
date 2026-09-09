#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.AsJsonDynamic\(\) Method


<b>Parse JSON as dynamic object (when you don't have a class).</b>

Useful for quick exploration or simple JSON structures. This method returns a dynamic object that allows 
             you to access JSON properties without defining a C# class. However, there's no compile-time checking, so prefer 
             [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') with typed classes when possible.

<b>Example:</b>

```csharp
dynamic json = result.AsJsonDynamic();
Console.WriteLine(json.name);           // Access properties directly
Console.WriteLine(json.users[0].email); // Navigate arrays

// Iterate dynamic arrays
foreach (var item in json.items)
{
    Console.WriteLine(item.title);
}
```

```csharp
public dynamic AsJsonDynamic();
```

#### Returns
[dynamic](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/using-type-dynamic 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/programming\-guide/types/using\-type\-dynamic')  

A dynamic object representing the JSON. In .NET 6+, this is a [System\.Text\.Json\.JsonDocument](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument 'System\.Text\.Json\.JsonDocument').
             In .NET Standard 2.0, this is a `JObject` from Newtonsoft.Json.

Access properties like: `dynamicObj.propertyName` or `dynamicObj["propertyName"]`

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
Thrown when [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') is null or empty\.

[JsonException](https://learn.microsoft.com/en-us/dotnet/api/jsonexception 'JsonException')  
Thrown when the JSON syntax is invalid\.

### Remarks

<b>⚠️ Warning:</b> No compile-time checking! If a property doesn't exist, you'll get a runtime exception.

For production code, prefer [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)') with typed classes for better safety and IntelliSense support.

This method is useful for:
- Quick prototyping and exploration
- Working with highly dynamic JSON structures
- One-off scripts and tools

### See Also
- [Parse JSON into typed classes \(recommended\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [Alternative method name for typed parsing](CurlDotNet.Core.CurlResult.AsJson_T_().md 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)')
- [The JSON string being parsed](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')