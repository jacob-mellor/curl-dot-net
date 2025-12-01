#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.ParseJson\<T\>\(\) Method


<b>Parse the JSON response into your C# class.</b>

The most common operation - turning JSON into objects. This method uses [System\.Text\.Json\.JsonSerializer](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer 'System\.Text\.Json\.JsonSerializer') 
             in .NET 6+ and [Newtonsoft\.Json\.JsonConvert](https://learn.microsoft.com/en-us/dotnet/api/newtonsoft.json.jsonconvert 'Newtonsoft\.Json\.JsonConvert') in .NET Standard 2.0 for maximum compatibility.

<b>Example:</b>

```csharp
// Define your class matching the JSON structure
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Id { get; set; }
}

// Parse the response
var user = result.ParseJson<User>();
Console.WriteLine($"Hello {user.Name}!");

// Or parse arrays
var users = result.ParseJson<List<User>>();
Console.WriteLine($"Found {users.Count} users");
```

<b>Tip:</b> Use https://json2csharp.com to generate C# classes from JSON!

```csharp
public T ParseJson<T>();
```
#### Type parameters

<a name='CurlDotNet.Core.CurlResult.ParseJson_T_().T'></a>

`T`

The type to deserialize to\. Must match the JSON structure\. Can be a class, struct, or collection type like [System\.Collections\.Generic\.List&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1') or [System\.Collections\.Generic\.Dictionary&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')\.

#### Returns
[T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T')  
An instance of [T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T') with data from the JSON [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
Thrown when [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') is null or empty\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
Thrown when JSON deserialization fails or JSON doesn't match type [T](CurlDotNet.Core.CurlResult.ParseJson_T_().md#CurlDotNet.Core.CurlResult.ParseJson_T_().T 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)\.T')\.

[JsonException](https://learn.microsoft.com/en-us/dotnet/api/jsonexception 'JsonException')  
Thrown when the JSON syntax is invalid\. See [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException') for details\.

### Remarks

This method automatically detects whether to use System.Text.Json or Newtonsoft.Json based on the target framework.

For complex JSON structures, consider using [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)') for exploration, then creating a typed class.

If T doesn't match the JSON structure, properties that don't match will be left at their default values.

### See Also
- [Alternative method name for parsing JSON](CurlDotNet.Core.CurlResult.AsJson_T_().md 'CurlDotNet\.Core\.CurlResult\.AsJson\<T\>\(\)')
- [Parse JSON as dynamic object without a class](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')
- [The JSON string being parsed](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')