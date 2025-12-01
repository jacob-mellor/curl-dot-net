#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlEngine Class

Core engine that processes and executes curl commands\.

```csharp
internal class CurlEngine : System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlEngine

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

### Remarks

This is the heart of CurlDotNet - translates curl commands to HTTP operations.

AI-Usage: This class handles the actual curl command execution.

| Constructors | |
| :--- | :--- |
| [CurlEngine\(\)](CurlDotNet.Core.CurlEngine.#ctor.md#CurlDotNet.Core.CurlEngine.CurlEngine() 'CurlDotNet\.Core\.CurlEngine\.CurlEngine\(\)') | Create a new CurlEngine with default HttpClient\. |
| [CurlEngine\(HttpClient\)](CurlDotNet.Core.CurlEngine.#ctor.md#CurlDotNet.Core.CurlEngine.CurlEngine(System.Net.Http.HttpClient) 'CurlDotNet\.Core\.CurlEngine\.CurlEngine\(System\.Net\.Http\.HttpClient\)') | Create a new CurlEngine with custom HttpClient\. |

| Methods | |
| :--- | :--- |
| [Dispose\(\)](CurlDotNet.Core.CurlEngine.Dispose().md 'CurlDotNet\.Core\.CurlEngine\.Dispose\(\)') | |
| [ExecuteAsync\(CurlOptions\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)') | Execute with parsed options\. |
| [ExecuteAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)') | Execute with parsed options and cancellation\. |
| [ExecuteAsync\(string\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(string) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(string\)') | Execute a curl command string\. |
| [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Execute a curl command with custom settings\. |
| [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Core.CurlEngine.ExecuteAsync.md#CurlDotNet.Core.CurlEngine.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlEngine\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)') | Execute a curl command with cancellation support\. |
| [ToFetchCode\(string\)](CurlDotNet.Core.CurlEngine.ToFetchCode(string).md 'CurlDotNet\.Core\.CurlEngine\.ToFetchCode\(string\)') | Convert curl command to JavaScript fetch\. |
| [ToHttpClientCode\(string\)](CurlDotNet.Core.CurlEngine.ToHttpClientCode(string).md 'CurlDotNet\.Core\.CurlEngine\.ToHttpClientCode\(string\)') | Convert curl command to HttpClient code\. |
| [ToPythonCode\(string\)](CurlDotNet.Core.CurlEngine.ToPythonCode(string).md 'CurlDotNet\.Core\.CurlEngine\.ToPythonCode\(string\)') | Convert curl command to Python requests\. |
| [Validate\(string\)](CurlDotNet.Core.CurlEngine.Validate(string).md 'CurlDotNet\.Core\.CurlEngine\.Validate\(string\)') | Validate a curl command without executing it\. |
