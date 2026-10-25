#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.StandardOutput Property

The text curl would have written to standard output before this failure, or null when nothing was written\.

```csharp
public string StandardOutput { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Response headers \(`-i`\), bodies, `-w` output and the output of earlier transfers of
            the same command, exactly as the curl tool prints them\.

### Remarks

Usage: run `curl -i -f URL`; on a 404 the [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')
            has `StandardOutput` set to the response headers, just like the terminal output of curl.
            With `curl URL1 URL2` where the second fails, it holds the body of the first.

### See Also
- [StandardOutputDataKey](CurlDotNet.Exceptions.CurlException.StandardOutputDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutputDataKey')