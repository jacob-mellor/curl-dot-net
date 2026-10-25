---
title: curl CLI Flags Supported in C# - CurlDotNet Compatibility
description: Generated reference of every curl command-line option and its status in CurlDotNet (implemented, terminal-only, or accepted and ignored), plus how shell syntax from Linux, macOS and Windows is parsed.
---

# curl CLI Flags Supported in C# (CurlDotNet Compatibility Reference)

This page lists every curl command-line option and what CurlDotNet does with it when you run a command string in C#. CurlDotNet's parser carries curl's complete option table (everything in curl 8.7.1's `curl --help all`, plus the newer options that curl's current test suite uses, such as `--follow`, `--out-null`, `--skip-existing`, `--ech` and `--mptcp`), so every real option consumes its value correctly even when CurlDotNet does not act on it. The leading `curl` word is optional in every command string.

Summary: 281 options in curl's table: **136 implemented**, 4 parsed but without effect, 4 terminal-only, 137 accepted and ignored (recorded in `UnsupportedOptions`).

## How this table was produced

The table was generated at version 9.8.0, not written by hand, and has since been updated by hand for the options implemented in 9.8.11 (`--haproxy-clientip` and `-#`). The probe program loaded the built `CurlDotNet.dll` (net10.0), read every `Opt(...)` row of the `CurlOptionTable` in `src/CurlDotNet/Core/CommandParser.cs`, and parsed `curl --<option> <value> http://example.com/` for each one. An option is:

- **Ignored** when the parser recorded it in `CurlOptions.UnsupportedOptions` (no handler in `ProcessOption` / `ProcessTransportOption`).
- **Implemented** when a handler took it and the `CurlOptions` property it sets is read by the transfer code.
- **Parsed, not applied** when a handler took it but it has no effect on the transfer and is not recorded in `UnsupportedOptions` (checked with a source search for readers of the property, then reviewed by hand). After the TLS, rate-limit and low-speed options were wired up this list is down to four connection-tuning switches.
- **Terminal-only** when it only changes what the curl tool prints on a terminal, which has no meaning in a library.

The single-letter aliases are curl's own. Long options also accept curl's `--no-` prefix for booleans (`--no-include`), `--option=value`, and the `--expand-` prefix for `--variable` expansion.

## Check a command in code

`Curl.Validate` parses without sending anything. Typos make the command invalid; real curl options that CurlDotNet ignores leave it valid but are listed:

```csharp
using CurlDotNet;

var check = Curl.Validate("curl --parallel --retyr 3 https://example.com");
Console.WriteLine(check.IsValid);   // False: --retyr is not a curl option
Console.WriteLine(check.Error);

var ok = Curl.Validate("curl --parallel https://example.com");
Console.WriteLine(ok.IsValid);      // True
Console.WriteLine(string.Join(", ", ok.ParsedOptions!.UnsupportedOptions)); // --parallel
```

`CurlOptions.UnknownOptions` holds the typos and `CurlOptions.UnsupportedOptions` the ignored real options, so you can log or reject either before running a command that came from configuration or user input.

## Implemented options

| Option | Takes a value | Notes |
|---|---|---|
| `-#`, `--progress-bar` | no | draws curl's progress bar (`#` characters, the width of `COLUMNS` or 79) to `--stderr FILE` or standard error; `-s` turns it off |
| `--anyauth` | no | prefers Digest, NTLM, Basic |
| `--aws-sigv4` | yes | request signing; malformed value is exit 2 |
| `--basic` | no |  |
| `--cacert` | yes | custom trust: only these CAs are trusted; .NET 5+; listed in `UnsupportedOptions` on netstandard2.0/.NET Framework |
| `--capath` | yes | folder of CA certificates; .NET 5+; listed in `UnsupportedOptions` on netstandard2.0/.NET Framework |
| `-E`, `--cert` | yes | client certificate, `FILE[:password]`; .NET 5+; listed in `UnsupportedOptions` on netstandard2.0/.NET Framework |
| `--cert-type` | yes | `PEM`, `DER` or `P12`; .NET 5+; listed in `UnsupportedOptions` on netstandard2.0/.NET Framework |
| `--ciphers` | yes | IANA names via `CipherSuitesPolicy` (Linux, .NET 5+); elsewhere listed in `UnsupportedOptions` |
| `--compressed` | no | gzip, deflate, br decoded in-library |
| `-K`, `--config` | yes | nested `-K` files; `~/.curlrc` is never read implicitly |
| `--connect-timeout` | yes | enforced on .NET 5+, best effort on netstandard2.0/.NET Framework |
| `--connect-to` | yes |  |
| `-C`, `--continue-at` | yes | `-C -` resumes from the file size |
| `-b`, `--cookie` | yes |  |
| `-c`, `--cookie-jar` | yes |  |
| `--create-dirs` | no |  |
| `-d`, `--data` | yes |  |
| `--data-ascii` | yes |  |
| `--data-binary` | yes |  |
| `--data-raw` | yes |  |
| `--data-urlencode` | yes |  |
| `--digest` | no | in-library, MD5 and SHA-256 (and -sess) |
| `-D`, `--dump-header` | yes |  |
| `--etag-compare` | yes |  |
| `--etag-save` | yes |  |
| `--expect100-timeout` | yes |  |
| `-f`, `--fail` | no | throws `CurlHttpReturnedErrorException` (22) |
| `--fail-early` | no |  |
| `--fail-with-body` | no | throws, keeping the body |
| `--follow` | no | like `-L`, but 301/302/303 may change an `-X` method to GET |
| `-F`, `--form` | yes |  |
| `--form-escape` | no |  |
| `--form-string` | yes |  |
| `--ftp-pasv` | no | FTP only (passive is the default) |
| `--ftp-ssl` | no | FTP only |
| `-G`, `--get` | no |  |
| `-g`, `--globoff` | no | disables `{}`/`[]` globbing |
| `-I`, `--head` | no |  |
| `-H`, `--header` | yes |  |
| `--haproxy-clientip` | yes | sends this address as both source and destination in the PROXY v1 line, and turns `--haproxy-protocol` on (.NET 5+) |
| `--haproxy-protocol` | no | sends a HAProxy PROXY protocol v1 line before the request, after any `-p` tunnel and before TLS (.NET 5+; exit 4 with an explanation on .NET Framework and netstandard2.0) |
| `--http0.9` | no |  |
| `-0`, `--http1.0` | no |  |
| `--http1.1` | no |  |
| `--http2` | no | `HttpRequestMessage.Version` 2.0 |
| `--http2-prior-knowledge` | no | HTTP/2 with `RequestVersionExact` |
| `--http3` | no | requests HTTP/3 through `HttpRequestMessage.Version` (.NET 5+) |
| `--http3-only` | no | HTTP/3 with `RequestVersionExact` |
| `--ignore-content-length` | no |  |
| `-i`, `--include` | no |  |
| `-k`, `--insecure` | no |  |
| `--json` | yes |  |
| `-j`, `--junk-session-cookies` | no |  |
| `--key` | yes | private key for `--cert`; .NET 5+; listed in `UnsupportedOptions` on netstandard2.0/.NET Framework |
| `--key-type` | yes | PEM keys only; other types are listed in `UnsupportedOptions` |
| `--limit-rate` | yes | download throttle, `CurlOptions.LimitRate`; `k`/`M`/`G` suffixes |
| `-L`, `--location` | no |  |
| `--location-trusted` | no |  |
| `--max-filesize` | yes | exit 63 |
| `--max-redirs` | yes |  |
| `-m`, `--max-time` | yes |  |
| `--negotiate` | no | Kerberos/SPNEGO left to the platform handler |
| `-n`, `--netrc` | no | also `$NETRC` |
| `--netrc-file` | yes |  |
| `--netrc-optional` | no |  |
| `-:`, `--next` | no | per-group options |
| `--no-clobber` | no |  |
| `--noproxy` | yes |  |
| `--ntlm` | no | NTLMv1/v2 in pure C# |
| `--oauth2-bearer` | yes |  |
| `--out-null` | no |  |
| `-o`, `--output` | yes |  |
| `--output-dir` | yes |  |
| `--pass` | yes | passphrase for an encrypted key; .NET 5+; listed in `UnsupportedOptions` on netstandard2.0/.NET Framework |
| `--path-as-is` | no |  |
| `--pinnedpubkey` | yes | `sha256//BASE64` pins or a key file; exit 90 on mismatch, exit 4 where it cannot be checked |
| `--post301` | no |  |
| `--post302` | no |  |
| `--post303` | no |  |
| `--proto-default` | yes |  |
| `-x`, `--proxy` | yes |  |
| `--proxy-anyauth` | no |  |
| `--proxy-basic` | no |  |
| `--proxy-digest` | no |  |
| `--proxy-header` | yes | sent only to the proxy (CONNECT request or proxied request) |
| `--proxy-negotiate` | no | Kerberos/SPNEGO left to the platform handler |
| `--proxy-ntlm` | no |  |
| `-U`, `--proxy-user` | yes |  |
| `--proxy1.0` | yes |  |
| `-p`, `--proxytunnel` | no | CONNECT tunnel, .NET 5+ |
| `-r`, `--range` | yes |  |
| `--raw` | no | .NET 5+ only |
| `-e`, `--referer` | yes |  |
| `-J`, `--remote-header-name` | no |  |
| `-O`, `--remote-name` | no |  |
| `--remote-name-all` | no |  |
| `-R`, `--remote-time` | no |  |
| `--remove-on-error` | no |  |
| `-X`, `--request` | yes |  |
| `--request-target` | yes | .NET 5+ only |
| `--resolve` | yes |  |
| `--retry` | yes | timeouts, DNS/proxy failures (5, 6), HTTP 408/429/500/502/503/504; waits the larger of the delay and `Retry-After` |
| `--retry-all-errors` | no |  |
| `--retry-connrefused` | no |  |
| `--retry-delay` | yes |  |
| `--retry-max-time` | yes |  |
| `--show-headers` | no |  |
| `--skip-existing` | no |  |
| `--socks4` | yes | proxy URL scheme `socks4://` |
| `--socks4a` | yes | proxy URL scheme `socks4a://` |
| `--socks5` | yes |  |
| `--socks5-hostname` | yes | proxy URL scheme `socks5h://` |
| `-Y`, `--speed-limit` | yes | aborts with exit 28 when slower for `--speed-time` seconds |
| `-y`, `--speed-time` | yes | window for `--speed-limit` (default 30 s) |
| `--stderr` | yes |  |
| `-z`, `--time-cond` | yes | unmet condition is reported as 304 |
| `--tls-max` | yes | maximum TLS version |
| `--tls13-ciphers` | yes | IANA names via `CipherSuitesPolicy` (Linux, .NET 5+); elsewhere listed in `UnsupportedOptions` |
| `-1`, `--tlsv1` | no | minimum TLS version |
| `--tlsv1.0` | no | minimum TLS version |
| `--tlsv1.1` | no | minimum TLS version |
| `--tlsv1.2` | no | minimum TLS version |
| `--tlsv1.3` | no | minimum TLS version (OS support needed) |
| `--tr-encoding` | no |  |
| `--trace` | yes | file or `-` |
| `--trace-ascii` | yes |  |
| `--trace-time` | no |  |
| `-T`, `--upload-file` | yes | PUT; filename appended to a URL ending in `/` |
| `--url` | yes |  |
| `--url-query` | yes |  |
| `-u`, `--user` | yes |  |
| `-A`, `--user-agent` | yes |  |
| `--variable` | yes | `{{name}}` with `:trim :json :url :b64 :64dec` functions |
| `-v`, `--verbose` | no |  |
| `-w`, `--write-out` | yes | every curl variable; `%{json}` emits numbers and null |

## Parsed, not applied

These few options are recognised and consumed but are not listed in `UnsupportedOptions`, although they only tune the TCP connection and have little or no effect: none of them changes the request or response bytes.

| Option | Takes a value | Notes |
|---|---|---|
| `--keepalive-time` | yes | only switches keep-alive on for FTP; HTTP uses HttpClient pooling. Not listed in `UnsupportedOptions` |
| `--no-keepalive` | no | accepted, no effect |
| `--tcp-fastopen` | no | accepted, no effect |
| `--tcp-nodelay` | no | accepted, no effect (.NET sets TCP_NODELAY anyway) |

## Terminal-only options

These change what the curl tool prints on a terminal. A library returns a `CurlResult` or throws an exception instead, so they have no visible effect.

| Option | Takes a value | Notes |
|---|---|---|
| `-N`, `--no-buffer` | no | output is not buffered to a terminal |
| `--no-progress-meter` | no | no progress meter in a library |
| `-S`, `--show-error` | no | no stderr in a library; errors are exceptions |
| `-s`, `--silent` | no | hides the `-#` progress bar and curl's warnings; kept for `-w %{stderr}` and error text |

## Accepted and ignored

Each of these is parsed (its value is consumed, so the rest of the command is read correctly) and recorded in `CurlOptions.UnsupportedOptions`. Most belong to protocols CurlDotNet does not implement (SMTP, IMAP, TFTP, TELNET, SSH), to TLS backend tuning that .NET does not expose per request (CRL files, curves, proxy-side TLS options), to connection details HttpClient does not expose (`--interface`, `--dns-servers`, `--local-port`), to raw FTP commands (`--quote`, `--disable-epsv`, `--disable-eprt`), or to curl tool features such as `--parallel` (transfers always run one after another), `--libcurl` and `--help`. `--ciphers`, `--tls13-ciphers` and a non-PEM `--key-type` appear here only on platforms that cannot apply them (see the notes in the implemented table); on netstandard2.0 and .NET Framework `--cacert`, `--capath`, `--cert`, `--key`, `--cert-type`, `--pass` and `--pinnedpubkey` are recorded here as well.

`--abstract-unix-socket`, `--alt-svc`, `-a`, `--append`, `--ca-native`, `--cert-status`, `--compressed-ssh`, `--create-file-mode`, `--crlf`, `--crlfile`, `--curves`, `--delegation`, `-q`, `--disable`, `--disable-eprt`, `--disable-epsv`, `--disallow-username-in-url`, `--dns-interface`, `--dns-ipv4-addr`, `--dns-ipv6-addr`, `--dns-servers`, `--doh-cert-status`, `--doh-insecure`, `--doh-url`, `--dump-ca-embed`, `--ech`, `--egd-file`, `--engine`, `--false-start`, `--ftp-account`, `--ftp-alternative-to-user`, `--ftp-create-dirs`, `--ftp-method`, `-P`, `--ftp-port`, `--ftp-pret`, `--ftp-skip-pasv-ip`, `--ftp-ssl-ccc`, `--ftp-ssl-ccc-mode`, `--ftp-ssl-control`, `--ftp-ssl-reqd`, `--happy-eyeballs-timeout-ms`, `-h`, `--help`, `--hostpubmd5`, `--hostpubsha256`, `--hsts`, `--httpsig-algo`, `--httpsig-headers`, `--httpsig-key`, `--httpsig-keyid`, `--interface`, `--ip-tos`, `--ipfs-gateway`, `-4`, `--ipv4`, `-6`, `--ipv6`, `--keepalive-cnt`, `--knownhosts`, `--krb`, `--krb4`, `--libcurl`, `-l`, `--list-only`, `--local-port`, `--login-options`, `--mail-auth`, `--mail-from`, `--mail-rcpt`, `--mail-rcpt-allowfails`, `-M`, `--manual`, `--metalink`, `--mptcp`, `--no-alpn`, `--no-npn`, `--no-sessionid`, `--ntlm-wb`, `-Z`, `--parallel`, `--parallel-immediate`, `--parallel-max`, `--preproxy`, `--proto`, `--proto-redir`, `--proxy-ca-native`, `--proxy-cacert`, `--proxy-capath`, `--proxy-cert`, `--proxy-cert-type`, `--proxy-ciphers`, `--proxy-crlfile`, `--proxy-dump-ca-embed`, `--proxy-http2`, `--proxy-insecure`, `--proxy-key`, `--proxy-key-type`, `--proxy-pass`, `--proxy-pinnedpubkey`, `--proxy-service-name`, `--proxy-ssl-allow-beast`, `--proxy-ssl-auto-client-cert`, `--proxy-tls13-ciphers`, `--proxy-tlsauthtype`, `--proxy-tlspassword`, `--proxy-tlsuser`, `--proxy-tlsv1`, `--pubkey`, `-Q`, `--quote`, `--random-file`, `--rate`, `--sasl-authzid`, `--sasl-ir`, `--service-name`, `--sigalgs`, `--socks5-basic`, `--socks5-gssapi`, `--socks5-gssapi-nec`, `--socks5-gssapi-service`, `--ssl`, `--ssl-allow-beast`, `--ssl-auto-client-cert`, `--ssl-no-revoke`, `--ssl-reqd`, `--ssl-revoke-best-effort`, `--ssl-sessions`, `-2`, `--sslv2`, `-3`, `--sslv3`, `--styled-output`, `--suppress-connect-headers`, `-t`, `--telnet-option`, `--tftp-blksize`, `--tftp-no-options`, `--tls-earlydata`, `--tlsauthtype`, `--tlspassword`, `--tlsuser`, `--trace-config`, `--trace-ids`, `--unix-socket`, `--upload-flags`, `-B`, `--use-ascii`, `-V`, `--version`, `--vlan-priority`, `--xattr`

`-q` and `--disable` stay in this list because CurlDotNet never reads a default `.curlrc` on its own. When you opt in with `Curl.DefaultReadConfigFile = true`, they work as in curl: given first, they stop that file from being read.

## Command-line syntax

| Syntax | Example | Behaviour |
|--------|---------|-----------|
| Combined short flags | `-sSL` | Same as `-s -S -L` |
| Attached values | `-ofile.txt`, `-m10`, `-H"X-A: 1"` | The value is the rest of the word |
| `--long=value` | `--max-time=2.5` | Same as `--max-time 2.5` |
| Values starting with `-` | `-d -1` | Taken as the value, as curl does |
| `--url` | `--url https://example.com` | Adds a URL, also `--url=...` |
| `--` | `curl -s -- https://example.com/` | Ends option parsing; every later word is a URL |
| Several URLs | `curl -o a.txt URL1 -o b.txt URL2` | Run in order; `-o`, `-O` and `-T` pair with URLs left to right |
| `--next` / `-:` | `curl -d x URL1 --next URL2` | Starts a new group with its own options |
| Globbing | `https://x/{a,b}/[1-3].txt` | Expanded like curl, `#1` in `-o`; `-g` disables |
| Config files | `-K opts.txt` | curl's config syntax, nested `-K` allowed |
| Variables | `--variable name=v --expand-url 'https://x/{{name:url}}'` | `trim`, `json`, `url`, `b64`, `64dec` functions |

Exit codes for bad command lines follow curl: no URL at all, a missing option value, a non-numeric value for a numeric option (`-m`, `--connect-timeout`, `--max-redirs`, `--retry*`, `--speed-*`, `--limit-rate`; numbers are read like curl, so `1e1` and `+5` are accepted), a malformed `--aws-sigv4` or two conflicting methods (`-I` with `-d`, `-F` with `-d`) is code 2, a bad URL or glob is 3, `-H @file` with a missing file is 26, and a header containing CR or LF is 43. See the [curl exit code reference](curl-exit-codes.html).

Multiple URLs, globbing and `--next` are covered in [multiple URLs and URL globbing](../cookbook/multiple-urls-and-globbing.html); `-K`, `--variable` and `.netrc` in [config files and variables](../cookbook/config-files-and-variables.html).

## Shell syntax: Linux, macOS and Windows

Commands copied from any terminal are normalized before parsing. Bash/zsh (Linux and macOS) is the canonical syntax, matching the curl manual; CMD and PowerShell conveniences are translated to it.

| Concern | Bash / zsh | CMD | PowerShell |
|---------|------------|-----|------------|
| Quotes | `'...'` and `"..."` | `"..."` | `'...'` and `"..."` |
| Line continuation | trailing `\` | trailing `^` | trailing `` ` `` |
| Environment variables | `$TOKEN`, `${TOKEN}` | `%TOKEN%` | `$env:TOKEN` |

All three continuation characters are joined into a single line. Environment variables are expanded from the process environment only outside single quotes, as a shell would: `'$HOME'` stays literal, and `\$` keeps a literal dollar sign. An unset variable is left as typed. PowerShell-style escaped JSON such as `-d '{\"k\":\"v\"}'` is unescaped to `{"k":"v"}`; this differs from bash, where the backslashes would be sent, and a value that also contains a bare `"` is left untouched.

```bash
curl -X POST https://httpbin.org/post \
  -H "Authorization: Bearer $API_TOKEN" \
  -d '{"name":"Ada","role":"admin"}'
```

```powershell
curl -H "Accept: application/json" `
     -H "Authorization: Bearer $env:API_TOKEN" `
     https://httpbin.org/get
```

```cmd
curl -X POST ^
     -H "Content-Type: application/json" ^
     -d "{\"foo\":\"bar\"}" ^
     https://httpbin.org/post
```

Guidance:

- Prefer single quotes around JSON bodies; inside a C# verbatim string, double quotes are written as `""`.
- Avoid smart quotes copied from rich-text editors; replace them with `'` or `"`.
- Shell features such as process substitution (`<(...)`), command substitution (`$(...)`) and aliases are not evaluated.
- `~` is not expanded; pass full paths to `-o`, `-d @file` and `-F file=@path`.
- `~/.curlrc` is never read implicitly, so a developer's personal curl settings cannot change what your application sends. Pass a config file explicitly with `-K`.
- When the values come from variables, skip quoting entirely: build a `CurlOptions` object and call `Curl.ExecuteAsync(options)`.

## Platform differences

On .NET 5 and later, CurlDotNet drives its own connection layer (`SocketsHttpHandler` with a connect callback). On netstandard2.0 and .NET Framework it relies on `HttpClient`'s parsed headers, so these options are not available there: `--request-target`, removing the `Host` header, sending blank headers, `--raw`, `-p` / `--proxytunnel` tunnels and a separate `--connect-timeout` (the overall `--max-time` still applies). The trust and client-certificate options (`--cacert`, `--capath`, `--cert`, `--key`, `--cert-type`, `--pass`) are recorded in `UnsupportedOptions` there and the operating system trust store is used; `--pinnedpubkey` is recorded there too, and an https transfer that uses it fails with exit code 4 rather than skip the check. `--ciphers` and `--tls13-ciphers` need `CipherSuitesPolicy`, which .NET supports on Linux only. See [framework compatibility](../framework-compatibility.html).

## Debugging a command

Build the request in C# and print the curl string it produces:

```csharp
using CurlDotNet.Core;

var command = CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada" })
    .WithBearerToken("token")
    .ToCurlCommand();
Console.WriteLine(command);
```

Add `-v` to see the request and response headers at the start of `result.Body` (the curl tool writes them to stderr; CurlDotNet puts them in the result), or `-w '%{json}'` for the transfer details (see [curl -w write-out in C#](../cookbook/write-out.html)).

## Related Pages

- [curl exit codes in C#](curl-exit-codes.html)
- [How CurlDotNet is tested against curl's own test suite](curl-test-suite.html)
- [Supported Protocols](../supported-protocols.html)
- [libcurl Mapping](libcurl-mapping.html)
- [Common Issues](../troubleshooting/common-issues.html)
