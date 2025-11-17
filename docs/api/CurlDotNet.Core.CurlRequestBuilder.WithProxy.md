#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithProxy Method

| Overloads | |
| :--- | :--- |
| [WithProxy\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string\)') | Set proxy URL\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)') | Set proxy with authentication\. |

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string)'></a>

## CurlRequestBuilder\.WithProxy\(string\) Method

Set proxy URL\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithProxy(string proxyUrl);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string)'></a>

## CurlRequestBuilder\.WithProxy\(string, string, string\) Method

Set proxy with authentication\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithProxy(string proxyUrl, string username, string password);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')