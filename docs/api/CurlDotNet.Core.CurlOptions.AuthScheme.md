#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.AuthScheme Property

HTTP authentication scheme selected on the command line
\(\-\-basic, \-\-digest, \-\-ntlm, \-\-negotiate, \-\-anyauth\)\.
Null means curl's default: preemptive Basic when credentials are present\.
Challenge\-based schemes \(digest/ntlm/negotiate/anyauth\) are performed via the
HTTP 401 challenge\-response flow instead of a preemptive Authorization header\.

```csharp
public string? AuthScheme { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')