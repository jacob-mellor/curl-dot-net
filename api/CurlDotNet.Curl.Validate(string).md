#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.Validate\(string\) Method


<b>Check if a curl command is valid without executing it.</b>

Useful for validating user input or checking commands before running them.
             This only checks syntax, not whether the URL actually exists.

<b>Example:</b>

```csharp
// Check if command is valid
var validation = Curl.Validate("curl -X POST https://api.example.com");

if (validation.IsValid)
{
    Console.WriteLine("✅ Command is valid!");
    // Safe to execute
    var result = await Curl.Execute(command);
}
else
{
    Console.WriteLine($"❌ Invalid command: {validation.ErrorMessage}");
    Console.WriteLine($"Problem at position {validation.ErrorPosition}");
}
```

<b>Validate User Input:</b>

```csharp
Console.Write("Enter curl command: ");
var userCommand = Console.ReadLine();

var validation = Curl.Validate(userCommand);
if (!validation.IsValid)
{
    Console.WriteLine($"Error: {validation.ErrorMessage}");
    if (validation.Suggestions.Any())
    {
        Console.WriteLine("Did you mean:");
        foreach (var suggestion in validation.Suggestions)
        {
            Console.WriteLine($"  - {suggestion}");
        }
    }
}
```

```csharp
public static CurlDotNet.Core.ValidationResult Validate(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.Validate(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command string to validate\.

#### Returns
[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')  
A [ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult') containing:
- <b>IsValid</b> - true if command is valid
- <b>ErrorMessage</b> - Description of what's wrong (if invalid)
- <b>ErrorPosition</b> - Character position of error
- <b>Suggestions</b> - Possible fixes for common mistakes