#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[HttpHandler](CurlDotNet.Core.HttpHandler.md 'CurlDotNet\.Core\.HttpHandler')

## HttpHandler\.IsTransientStatus\(int\) Method

curl's "transient" HTTP responses for \-\-retry\.

```csharp
private static bool IsTransientStatus(int statusCode);
```
#### Parameters

<a name='CurlDotNet.Core.HttpHandler.IsTransientStatus(int).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')