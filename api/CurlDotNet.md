#### [CurlDotNet](index.md 'index')

## CurlDotNet Namespace

| Classes | |
| :--- | :--- |
| [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl') | Runs curl commands from C\#: pass the command text you would type in a terminal \(for example `curl -H 'Accept: application/json' https://api.example.com`\) and get a [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') back\. |
| [CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient') | A tiny REST client bound to one base URL: `GetAsync("users/1")`, `PostJsonAsync("users", obj)`, PUT, PATCH and DELETE, each with the same time limit \- for scripts that call one API many times\. |
| [DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl') | The same API as [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl') under different names \(`DotNetCurl.Curl(command)`, `DotNetCurl.CurlAsync(command)`\), for code that prefers to read "curl this command"\. |
| [ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions') | Extra [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') helpers in the root `CurlDotNet` namespace: JSON parsing with your own [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions'), a non\-throwing `TryParseJson`, `EnsureSuccessStatusCode` and a \(success, body, error\) tuple\. |

| Enums | |
| :--- | :--- |
| [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor') | Names the shell a curl command string was written for, so CurlDotNet applies that shell's quoting, escaping, line\-continuation and variable rules before reading the curl options\. |
