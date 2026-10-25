#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient')

## CurlApiClient\(string, Nullable\<TimeSpan\>\) Constructor

Creates a client for [baseUrl](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md#CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).baseUrl 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)\.baseUrl') whose requests all use [defaultTimeout](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md#CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).defaultTimeout 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)\.defaultTimeout') \(30 seconds
when null\) as their total time limit\.

```csharp
public CurlApiClient(string baseUrl, System.Nullable<System.TimeSpan> defaultTimeout=null);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).baseUrl'></a>

`baseUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute base URL including scheme, such as `https://api.example.com/v1`; must not be null\.

<a name='CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).defaultTimeout'></a>

`defaultTimeout` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

Total time limit per request \(curl `--max-time`\); null means 30 seconds\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[baseUrl](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md#CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).baseUrl 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)\.baseUrl') is null; pass the API's root URL\.

### Remarks

A trailing `/` on [baseUrl](CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).md#CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).baseUrl 'CurlDotNet\.CurlApiClient\.CurlApiClient\(string, System\.Nullable\<System\.TimeSpan\>\)\.baseUrl') is removed, so `"https://api.example.com/v1/"` and
            `"https://api.example.com/v1"` behave the same. The URL is not validated here; a malformed one fails on the first
            request with [CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException').

Example: `var api = new CurlApiClient("https://api.example.com/v1", TimeSpan.FromSeconds(15));`

### See Also
- [GetAsync\(string\)](CurlDotNet.CurlApiClient.GetAsync(string).md 'CurlDotNet\.CurlApiClient\.GetAsync\(string\)')