#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.EmptyHeaders Property

Header names sent with an empty value, from curl's `-H "Name;"` syntax \(as opposed to `-H "Name:"`,
which removes the header\)\.

```csharp
public System.Collections.Generic.HashSet<string> EmptyHeaders { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.HashSet&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1 'System\.Collections\.Generic\.HashSet\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1 'System\.Collections\.Generic\.HashSet\`1')  
Header names, compared case\-insensitively\. Default: empty\.

### Remarks

curl distinguishes `-H "X-Empty;"` (send `X-Empty:` with no value) from `-H "X-Empty:"` (do not send the
            header at all, even one curl would add itself). The latter is stored in [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers') with an empty value; the
            former is stored here. A non-empty value for the same name in [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers') wins.

Example: `curl -H "X-Requested-With;" https://example.com/` is `options.EmptyHeaders.Add("X-Requested-With");`.

### See Also
- [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers')