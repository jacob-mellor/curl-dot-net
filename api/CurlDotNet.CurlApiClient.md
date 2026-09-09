#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## CurlApiClient Class

Simplified API client builder for common HTTP operations\.
Provides a more ergonomic alternative to raw curl commands\.

```csharp
public class CurlApiClient
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlApiClient

| Constructors | |
| :--- | :--- |
| [CurlApiClient\(string, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)') | Create a new API client with a base URL\. |

| Methods | |
| :--- | :--- |
| [DeleteAsync\(string\)](CurlDotNet.CurlApiClient.DeleteAsync(string).md 'CurlDotNet\.CurlApiClient\.DeleteAsync\(string\)') | Perform a DELETE request\. |
| [GetAsync\(string\)](CurlDotNet.CurlApiClient.GetAsync(string).md 'CurlDotNet\.CurlApiClient\.GetAsync\(string\)') | Perform a GET request\. |
| [PatchJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PatchJsonAsync\(string, object\)') | Perform a PATCH request with JSON body\. |
| [PostJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PostJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PostJsonAsync\(string, object\)') | Perform a POST request with JSON body\. |
| [PutJsonAsync\(string, object\)](CurlDotNet.CurlApiClient.PutJsonAsync(string,object).md 'CurlDotNet\.CurlApiClient\.PutJsonAsync\(string, object\)') | Perform a PUT request with JSON body\. |
