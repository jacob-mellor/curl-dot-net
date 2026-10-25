#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')

## CurlHttpReturnedErrorException\.CurlErrorMessage Property

curl's own error text for this failure, `The requested URL returned error: ` followed by the status code,
exactly as the curl tool prints it after `curl: (22) `\.

```csharp
public override string CurlErrorMessage { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
For a 404, `The requested URL returned error: 404`: no reason phrase and no URL, like curl\.

### Remarks

The same text is used for `-f` and `--fail-with-body`. [WhatHappened](CurlDotNet.Exceptions.CurlException.WhatHappened.md 'CurlDotNet\.Exceptions\.CurlException\.WhatHappened')
            keeps CurlDotNet's longer first line, which adds the reason phrase and the URL.

Example: `catch (CurlHttpReturnedErrorException ex) { Console.Error.WriteLine($"curl: (22) {ex.CurlErrorMessage}"); }`

### See Also
- [CurlErrorMessage](CurlDotNet.Exceptions.CurlException.CurlErrorMessage.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorMessage')