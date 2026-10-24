#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UnknownOptions Property

Options on the command line that CurlDotNet did not recognise\. Execution ignores them
\(curl itself would exit with code 2\), and [Validate\(string\)](CurlDotNet.Core.CurlEngine.Validate(string).md 'CurlDotNet\.Core\.CurlEngine\.Validate\(string\)') reports
them so typos such as `--hedaer` are caught before a request is made\.

```csharp
public System.Collections.Generic.List<string> UnknownOptions { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')