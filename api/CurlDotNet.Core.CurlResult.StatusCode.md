#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.StatusCode Property


<b>The HTTP status code - tells you what happened.</b>

Common codes you'll see:

```csharp
200 = OK, it worked!
201 = Created something new
204 = Success, but no content returned
400 = Bad request (you sent something wrong)
401 = Unauthorized (need to login)
403 = Forbidden (not allowed)
404 = Not found
429 = Too many requests (slow down!)
500 = Server error (their fault, not yours)
503 = Service unavailable (try again later)
```

<b>Example - Handle different statuses:</b>

```csharp
switch (result.StatusCode)
{
    case 200: ProcessData(result.Body); break;
    case 404: Console.WriteLine("Not found"); break;
    case 401: RedirectToLogin(); break;
    case >= 500: Console.WriteLine("Server error, retry later"); break;
}
```

```csharp
public int StatusCode { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')