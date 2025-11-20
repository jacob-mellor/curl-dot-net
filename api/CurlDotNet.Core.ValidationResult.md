#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

## ValidationResult Class

Result of command validation\.

```csharp
public class ValidationResult
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ValidationResult
### Properties

<a name='CurlDotNet.Core.ValidationResult.Error'></a>

## ValidationResult\.Error Property

Error message if validation failed, null if valid\.

```csharp
public string? Error { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.ValidationResult.IsValid'></a>

## ValidationResult\.IsValid Property

Whether the validation succeeded\.

```csharp
public bool IsValid { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Core.ValidationResult.ParsedOptions'></a>

## ValidationResult\.ParsedOptions Property

Parsed options if validation succeeded, null if invalid\.

```csharp
public CurlDotNet.Core.CurlOptions? ParsedOptions { get; set; }
```

#### Property Value
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')