#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler Constructors

| Overloads | |
| :--- | :--- |
| [HttpHandler\(\)](CurlDotNet.Core.HttpHandler.#ctor.md#CurlDotNet.Core.HttpHandler.HttpHandler() 'CurlDotNet\.Core\.HttpHandler\.HttpHandler\(\)') | Create handler with default HttpClient\. |
| [HttpHandler\(HttpClient, bool\)](CurlDotNet.Core.HttpHandler.#ctor.md#CurlDotNet.Core.HttpHandler.HttpHandler(System.Net.Http.HttpClient,bool) 'CurlDotNet\.Core\.HttpHandler\.HttpHandler\(System\.Net\.Http\.HttpClient, bool\)') | Create handler with custom HttpClient\. |

<a name='ctor.md#CurlDotNet.Core.HttpHandler.HttpHandler()'></a>

## HttpHandler\(\) Constructor

Create handler with default HttpClient\.

```csharp
public HttpHandler();
```

<a name='ctor.md#CurlDotNet.Core.HttpHandler.HttpHandler(System.Net.Http.HttpClient,bool)'></a>

## HttpHandler\(HttpClient, bool\) Constructor

Create handler with custom HttpClient\.

```csharp
public HttpHandler(System.Net.Http.HttpClient httpClient, bool ownsHttpClient=false);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.HttpHandler(System.Net.Http.HttpClient,bool).httpClient'></a>

`httpClient` [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')

<a name='CurlDotNet.Core.HttpHandler.HttpHandler(System.Net.Http.HttpClient,bool).ownsHttpClient'></a>

`ownsHttpClient` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')