#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.ToCurlCommand\(\) Method

Returns the curl command line equivalent to this builder, such as
`curl -X POST -H 'Content-Type: application/json' -d '{"a":1}' https://api.example.com/items`,
for logging, bug reports or running the same request in a shell\.

```csharp
public string ToCurlCommand();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A single\-line command starting with `curl` and ending with the URL\.

### Remarks

Values are quoted for a POSIX shell (bash, zsh) with single quotes, so the string can be pasted into
            a terminal or back into [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'), which parses it into the same request.
            Flags printed: `-X` (non-GET methods), `-H`, `-A`, `-e`, `-d`, `--data-binary`
            (only for bytes that are plain UTF-8 text), `-F`, `-u`, `-L`, `-k`, `-v`,
            `-s`, `-f`, `-i`, `--compressed`, `--max-time`, `--connect-timeout`,
            `--max-redirs` (when not 50), `--http1.0`/`--http1.1`/`--http2`/`--http3`,
            `-r`, `-x`, `--proxy-user`, `-b`, `-c`, `-o`, `-O`, `--trace`,
            `--trace-ascii`, `--trace-time`, then the URL.

Not printed, because curl has no flag for them: real binary bodies from [WithBinaryData\(byte\[\]\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryData\(byte\[\]\)'),
            [AsBinary\(\)](CurlDotNet.Core.CurlRequestBuilder.AsBinary().md 'CurlDotNet\.Core\.CurlRequestBuilder\.AsBinary\(\)'), [WithBinaryContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryContentType\(string\)') and [WithTextContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithTextContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTextContentType\(string\)').

Security: passwords from [WithBasicAuth\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBasicAuth\(string, string\)') and [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)')
            and tokens in headers appear in clear text; do not write this string to shared logs.

Example: `Console.WriteLine(CurlRequestBuilder.Get("https://example.com").WithHeader("Accept", "text/html").FollowRedirects().ToCurlCommand());`
            prints `curl -H 'Accept: text/html' -L https://example.com`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md

### See Also
- [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
- [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')
- [GetOptions\(\)](CurlDotNet.Core.CurlRequestBuilder.GetOptions().md 'CurlDotNet\.Core\.CurlRequestBuilder\.GetOptions\(\)')