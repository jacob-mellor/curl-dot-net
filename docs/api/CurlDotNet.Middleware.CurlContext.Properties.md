#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

## CurlContext\.Properties Property

A per\-request bag for passing values between middleware and the final handler, such as a correlation
id, a stopwatch or the user the request is made for\.

```csharp
public System.Collections.Generic.Dictionary<string,object> Properties { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
A case\-sensitive dictionary, empty by default\. [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware') writes
            `TimingStart` and `TimingEnd` \(UTC [System\.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime 'System\.DateTime')\) and `TimingDuration` \([System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')\)\.

### Remarks

Set values with [WithProperty\(string, object\)](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)') or the indexer, and read them back typed with
            [GetProperty&lt;T&gt;\(string\)](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)'). Keys are ordinary strings; prefix your own keys (for example
            `"myapp.tenant"`) to avoid clashes with other middleware.

Example: `context.Properties["requestId"] = Guid.NewGuid();` in one middleware and
            `var id = context.GetProperty<Guid>("requestId");` in another.

### See Also
- [WithProperty\(string, object\)](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)')
- [GetProperty&lt;T&gt;\(string\)](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)')