#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.UsesChallengeAuth\(CurlOptions\) Method

True when the command selected a challenge\-response auth scheme
\(\-\-digest, \-\-ntlm, \-\-negotiate, \-\-anyauth\) with credentials\. These schemes
are performed by the handler's 401 challenge flow, not a preemptive header,
so they need a dedicated HttpClient carrying the credentials\.

```csharp
internal static bool UsesChallengeAuth(CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.UsesChallengeAuth(CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')