#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UnsupportedOptions Property

Real curl options that appeared on the command line but that CurlDotNet does not act on yet, in the order
they were given \(for example `--tcp-nodelay` or `--ciphers`\)\.

```csharp
public System.Collections.Generic.List<string> UnsupportedOptions { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
The long option names; empty when every option was understood \(and for hand\-built options\)\.

### Remarks

The parser knows every option of curl 8.7.1 (and the newer ones used by curl's own test suite), so a
            recognised-but-unimplemented option still consumes its argument correctly and the URL is never mistaken for it:
            `curl --ciphers ALL https://example.com` requests `https://example.com`. Such options land here instead of
            in [UnknownOptions](CurlDotNet.Core.CurlOptions.UnknownOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnknownOptions'), which only holds names that curl itself would reject with exit code 2.

Each entry is the long form as typed, for example `--tcp-nodelay` or `--no-sessionid`; short forms are
            recorded by their long name (`-q` becomes `--disable`). Options that are parsed into a property which
            cannot be applied are listed here too: `--interface`, `--dns-servers`, `--quote`,
            `--disable-epsv`, `--disable-eprt`, `--key-type` other than PEM, `--ciphers`/`--tls13-ciphers`
            where the platform cannot choose cipher suites, and on .NET Standard 2.0/.NET Framework `--cacert`,
            `--capath`, `--cert`, `--key` and `--pinnedpubkey`.

Example: after `var o = Curl.Validate(cmd).ParsedOptions;` check `o.UnsupportedOptions.Count == 0`
            to know the request will be sent exactly as curl would send it.

### See Also
- [UnknownOptions](CurlDotNet.Core.CurlOptions.UnknownOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnknownOptions')
- [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)')