#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## HttpHandler Class

Handler for HTTP and HTTPS protocols\.

```csharp
internal class HttpHandler
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; HttpHandler

| Constructors | |
| :--- | :--- |
| [HttpHandler\(\)](CurlDotNet.Core.HttpHandler.#ctor.md#CurlDotNet.Core.HttpHandler.HttpHandler() 'CurlDotNet\.Core\.HttpHandler\.HttpHandler\(\)') | Create handler with default HttpClient\. |
| [HttpHandler\(HttpClient, bool\)](CurlDotNet.Core.HttpHandler.#ctor.md#CurlDotNet.Core.HttpHandler.HttpHandler(System.Net.Http.HttpClient,bool) 'CurlDotNet\.Core\.HttpHandler\.HttpHandler\(System\.Net\.Http\.HttpClient, bool\)') | Create handler with custom HttpClient\. |

| Fields | |
| :--- | :--- |
| [BinaryMagicNumbers](CurlDotNet.Core.HttpHandler.BinaryMagicNumbers.md 'CurlDotNet\.Core\.HttpHandler\.BinaryMagicNumbers') | Well\-known binary file signatures\. Servers frequently serve binary files under text/\* or application/xml Content\-Types \(misconfigured MIME maps default unknown extensions to text/plain\); real curl is immune because it never decodes bytes\. Sniffing the payload lets us match that behavior\. |
| [KnownBinaryMimeTypes](CurlDotNet.Core.HttpHandler.KnownBinaryMimeTypes.md 'CurlDotNet\.Core\.HttpHandler\.KnownBinaryMimeTypes') | Known binary MIME types that must never be treated as text, even if their MIME type string contains substrings like "xml" \(e\.g\., openxmlformats\)\. |

| Methods | |
| :--- | :--- |
| [BuildHeaderBlock\(HttpResponseMessage\)](CurlDotNet.Core.HttpHandler.BuildHeaderBlock(System.Net.Http.HttpResponseMessage).md 'CurlDotNet\.Core\.HttpHandler\.BuildHeaderBlock\(System\.Net\.Http\.HttpResponseMessage\)') | Rebuilds the response header block the way curl \-i prints it: status line, one header per line, CRLF line endings, no trailing blank line\. |
| [BuildIncludedHeaderText\(HttpResponseMessage, IReadOnlyList&lt;string&gt;\)](CurlDotNet.Core.HttpHandler.BuildIncludedHeaderText(System.Net.Http.HttpResponseMessage,System.Collections.Generic.IReadOnlyList_string_).md 'CurlDotNet\.Core\.HttpHandler\.BuildIncludedHeaderText\(System\.Net\.Http\.HttpResponseMessage, System\.Collections\.Generic\.IReadOnlyList\<string\>\)') | The complete header text curl \-i emits before the body: every redirect hop's block \(with \-L\), then the final block, each followed by an empty line\. |
| [CreateConfiguredClient\(CurlOptions\)](CurlDotNet.Core.HttpHandler.CreateConfiguredClient(CurlDotNet.Core.CurlOptions).md 'CurlDotNet\.Core\.HttpHandler\.CreateConfiguredClient\(CurlDotNet\.Core\.CurlOptions\)') | Builds an HttpClient configured with the command's proxy and TLS options so that `--proxy`, `--socks5`, `--proxy-user` and `-k` are honored\. |
| [DecodeContentEncoding\(byte\[\], ICollection&lt;string&gt;\)](CurlDotNet.Core.HttpHandler.DecodeContentEncoding(byte[],System.Collections.Generic.ICollection_string_).md 'CurlDotNet\.Core\.HttpHandler\.DecodeContentEncoding\(byte\[\], System\.Collections\.Generic\.ICollection\<string\>\)') | Decodes gzip, deflate \(zlib or raw\) and, where the framework supports it, Brotli bodies for \-\-compressed\. Unknown or identity encodings are returned untouched\. |
| [DecodeTextBody\(byte\[\], string\)](CurlDotNet.Core.HttpHandler.DecodeTextBody(byte[],string).md 'CurlDotNet\.Core\.HttpHandler\.DecodeTextBody\(byte\[\], string\)') | Decodes a text body from its raw bytes honoring the declared charset and any byte\-order mark, falling back to UTF\-8\. |
| [DescribeProxy\(CurlOptions, Uri\)](CurlDotNet.Core.HttpHandler.DescribeProxy(CurlDotNet.Core.CurlOptions,System.Uri).md 'CurlDotNet\.Core\.HttpHandler\.DescribeProxy\(CurlDotNet\.Core\.CurlOptions, System\.Uri\)') | Produces a human\-readable description of the proxy that will be used for the request \(explicit, SOCKS5, system, or none\) for inclusion in the trace\. |
| [IsTextContent\(HttpContent, CurlOptions\)](CurlDotNet.Core.HttpHandler.IsTextContent(System.Net.Http.HttpContent,CurlDotNet.Core.CurlOptions).md 'CurlDotNet\.Core\.HttpHandler\.IsTextContent\(System\.Net\.Http\.HttpContent, CurlDotNet\.Core\.CurlOptions\)') | Determines whether the HTTP content should be treated as text or binary\. Uses precise MIME type matching, a comprehensive known\-binary registry, and user\-configurable overrides to avoid corrupting binary data\. |
| [IsTransientStatus\(int\)](CurlDotNet.Core.HttpHandler.IsTransientStatus(int).md 'CurlDotNet\.Core\.HttpHandler\.IsTransientStatus\(int\)') | curl's "transient" HTTP responses for \-\-retry\. |
| [IsUserForcedText\(HttpContent, CurlOptions\)](CurlDotNet.Core.HttpHandler.IsUserForcedText(System.Net.Http.HttpContent,CurlDotNet.Core.CurlOptions).md 'CurlDotNet\.Core\.HttpHandler\.IsUserForcedText\(System\.Net\.Http\.HttpContent, CurlDotNet\.Core\.CurlOptions\)') | True when the user explicitly registered this response's media type as text via [TextContentTypes](CurlDotNet.Core.CurlOptions.TextContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.TextContentTypes')\. Explicit user overrides beat content sniffing\. |
| [LooksBinary\(byte\[\], string\)](CurlDotNet.Core.HttpHandler.LooksBinary(byte[],string).md 'CurlDotNet\.Core\.HttpHandler\.LooksBinary\(byte\[\], string\)') | Sniffs a payload that headers classified as text for binary content, so a mislabeled binary file is stored as bytes instead of being corrupted by string decoding\. |
| [NeedsCustomTransport\(CurlOptions\)](CurlDotNet.Core.HttpHandler.NeedsCustomTransport(CurlDotNet.Core.CurlOptions).md 'CurlDotNet\.Core\.HttpHandler\.NeedsCustomTransport\(CurlDotNet\.Core\.CurlOptions\)') | Determines whether the request needs a dedicated HttpClient because it specifies transport\-level options \(explicit proxy or relaxed TLS\) that the shared pooled client does not carry\. |
| [NormalizeProxyUrl\(string, string\)](CurlDotNet.Core.HttpHandler.NormalizeProxyUrl(string,string).md 'CurlDotNet\.Core\.HttpHandler\.NormalizeProxyUrl\(string, string\)') | Prepends a default scheme to a proxy address when the user supplied a bare host:port \(curl allows `proxy.example.com:8080`\), since WebProxy requires a scheme\. |
| [UsesChallengeAuth\(CurlOptions\)](CurlDotNet.Core.HttpHandler.UsesChallengeAuth(CurlDotNet.Core.CurlOptions).md 'CurlDotNet\.Core\.HttpHandler\.UsesChallengeAuth\(CurlDotNet\.Core\.CurlOptions\)') | True when the command selected a challenge\-response auth scheme \(\-\-digest, \-\-ntlm, \-\-negotiate, \-\-anyauth\) with credentials\. These schemes are performed by the handler's 401 challenge flow, not a preemptive header, so they need a dedicated HttpClient carrying the credentials\. |
