#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.IsRetryable\(\) Method

Check if this error is potentially retryable\.

```csharp
public virtual bool IsRetryable();
```

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if the error might succeed on retry