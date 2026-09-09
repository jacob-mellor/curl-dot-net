#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.IsSuccess Property


<b>Quick success check - true if status is 200-299.</b>

The easiest way to check if your request worked:

```csharp
if (result.IsSuccess)
{
    // It worked! Do something with result.Body
}
else
{
    // Something went wrong, check result.StatusCode
}
```

What's considered success: 200 OK, 201 Created, 204 No Content, etc.

What's NOT success: 404 Not Found, 500 Server Error, etc.

```csharp
public bool IsSuccess { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')