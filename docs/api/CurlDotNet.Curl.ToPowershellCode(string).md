#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ToPowershellCode\(string\) Method


<b>Convert a curl command to PowerShell code.</b>

Generates PowerShell code using Invoke-RestMethod that performs the same request.

<b>Example:</b>

```powershell
var curlCommand = "curl -X POST https://api.example.com/data -d '{\"key\":\"value\"}'";

string psCode = Curl.ToPowershellCode(curlCommand);
Console.WriteLine(psCode);

// Output:
// Invoke-RestMethod -Uri "https://api.example.com/data" -Method POST -Body "{\"key\":\"value\"}" -ContentType "application/json"
```

```csharp
public static string ToPowershellCode(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToPowershellCode(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
PowerShell code using Invoke\-RestMethod\.