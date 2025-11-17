#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.EnsureStatus\(int\) Method


<b>Throw if status doesn't match what you expect.</b>

Validate specific status codes:

```csharp
// Expect 201 Created
result.EnsureStatus(201);

// Expect 204 No Content
result.EnsureStatus(204);
```

```csharp
public CurlDotNet.Core.CurlResult EnsureStatus(int expectedStatus);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.EnsureStatus(int).expectedStatus'></a>

`expectedStatus` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The status code you expect

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This result if status matches \(for chaining\)

#### Exceptions

[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')  
Thrown if status doesn't match