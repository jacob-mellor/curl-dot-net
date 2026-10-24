#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.TransportOptionsRequiringValue Field

Long option names handled by [ProcessTransportOption\(string, string, CurlOptions, bool\)](CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).md 'CurlDotNet\.Core\.CommandParser\.ProcessTransportOption\(string, string, CurlDotNet\.Core\.CurlOptions, bool\)') that take a value, so the
tokenizer consumes the following argument instead of mistaking it for the URL\.

```csharp
private static readonly HashSet<string> TransportOptionsRequiringValue;
```

#### Field Value
[System\.Collections\.Generic\.HashSet&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1 'System\.Collections\.Generic\.HashSet\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1 'System\.Collections\.Generic\.HashSet\`1')