---
title: curl CLI Flags Supported in C# - CurlDotNet Compatibility
description: Generated reference of every curl command-line option and its status in CurlDotNet (implemented, parsed but not applied, or ignored), plus how shell syntax from Linux, macOS and Windows is parsed.
---

# curl CLI Flags Supported in C# (CurlDotNet Compatibility Reference)

This page lists every curl command-line option and what CurlDotNet does with it when you run a command string in C#. CurlDotNet's parser carries curl's complete option table (everything in curl 8.7.1's `curl --help all`, plus the newer options that curl's current test suite uses, such as `--follow`, `--out-null`, `--skip-existing`, `--ech` and `--mptcp`), so every real option consumes its value correctly even when CurlDotNet does not act on it. The leading `curl` word is optional in every command string.

Summary: 281 options in curl's table: **114 implemented**, 15 parsed but not applied, 5 terminal-only, 147 accepted and ignored.

## How this table was produced

The table is generated, not written by hand. A small probe program loaded the built `CurlDotNet.dll` (net10.0, version 9.8.0), read every `Opt(...)` row of the `CurlOptionTable` in `src/CurlDotNet/Core/CommandParser.cs`, and parsed `curl --<option> <value> http://example.com/` for each one. An option is:

- **Ignored** when the parser recorded it in `CurlOptions.UnsupportedOptions` (no handler in `ProcessOption` / `ProcessTransportOption`).
- **Implemented** when a handler took it and the `CurlOptions` property it sets is read by the transfer code.
- **Parsed, not applied** when a handler stored it but nothing outside the parser reads that property (checked with a source search, then reviewed by hand).
- **Terminal-only** when it only changes what the curl tool prints on a terminal, which has no meaning in a library.

The single-letter aliases are curl's own. Long options also accept curl's `--no-` prefix for booleans (`--no-include`), `--option=value`, and the `--expand-` prefix for `--variable` expansion.

## Check a command in code

`Curl.Validate` parses without sending anything. Typos make the command invalid; real curl options that CurlDotNet ignores leave it valid but are listed:

```csharp
using CurlDotNet;

var check = Curl.Validate("curl --tlsv1.3 --retyr 3 https://example.com");
Console.WriteLine(check.IsValid);   // False: --retyr is not a curl option
Console.WriteLine(check.Error);

var ok = Curl.Validate("curl --tlsv1.3 https://example.com");
Console.WriteLine(ok.IsValid);      // True
Console.WriteLine(string.Join(", ", ok.ParsedOptions!.UnsupportedOptions)); // --tlsv1.3
```

`CurlOptions.UnknownOptions` holds the typos and `CurlOptions.UnsupportedOptions` the ignored real options, so you can log or reject either before running a command that came from configuration or user input.

## Implemented options

| Option | Takes a value | Notes |
|---|---|---|
| `--anyauth` | no | prefers Digest, NTLM, Basic |
| `--aws-sigv4` | yes | request signing |
| `--basic` | no |  |
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
| `--path-as-is` | no |  |
| `--post301` | no |  |
| `--post302` | no |  |
| `--post303` | no |  |
| `--proto-default` | yes |  |
| `-x`, `--proxy` | yes |  |
| `--proxy-anyauth` | no |  |
| `--proxy-basic` | no |  |
| `--proxy-digest` | no |  |
| `--proxy-header` | yes | applied to requests sent through the proxy (the name is also recorded in `UnsupportedOptions`) |
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
| `--retry` | yes | timeouts, 408/429/500/502/503/504 |
| `--retry-all-errors` | no |  |
| `--retry-connrefused` | no |  |
| `--retry-delay` | yes |  |
| `--retry-max-time` | yes |  |
| `--show-headers` | no |  |
| `--skip-existing` | no |  |
| `--socks4` | yes | proxy URL scheme `socks4://` |
| `--socks4a` | yes | proxy URL scheme `socks4a://` |
| `--socks5` | yes | proxy URL scheme `socks5://` |
| `--socks5-hostname` | yes | proxy URL scheme `socks5h://` |
| `--stderr` | yes |  |
| `-z`, `--time-cond` | yes | unmet condition is reported as 304 |
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
| `-w`, `--write-out` | yes | every curl variable |

## Parsed, not applied

These options are recognised and stored in `CurlOptions`, but the transfer does not use them yet. They are not listed in `UnsupportedOptions`, so check this table if one of them matters to you. Client certificates and custom CA bundles in particular are not available: use the operating system trust store, or call `HttpClient` directly with a configured handler (`Curl.ToHttpClient` generates equivalent `HttpClient` C# code to start from).

| Option | Takes a value | Notes |
|---|---|---|
| `--cacert` | yes | stored in `CurlOptions.CaCertFile`; use the OS trust store |
| `-E`, `--cert` | yes | stored in `CurlOptions.CertFile`; client certificates are not sent |
| `--disable-eprt` | no | FtpWebRequest has no EPRT switch |
| `--disable-epsv` | no | FtpWebRequest has no EPSV switch |
| `--dns-servers` | yes | stored only |
| `--interface` | yes | stored only |
| `--keepalive-time` | yes | FTP keep-alive flag only |
| `--key` | yes | stored in `CurlOptions.KeyFile` |
| `--limit-rate` | yes | stored in `SpeedLimit`; no throttling |
| `--no-keepalive` | no | accepted, no effect |
| `-Q`, `--quote` | yes | stored only; FTP commands are not sent |
| `-Y`, `--speed-limit` | yes | stored only; no low-speed abort |
| `-y`, `--speed-time` | yes | stored only |
| `--tcp-fastopen` | no | accepted, no effect |
| `--tcp-nodelay` | no | accepted, no effect (.NET sets TCP_NODELAY anyway) |

## Terminal-only options

These change what the curl tool prints on a terminal. A library returns a `CurlResult` or throws an exception instead, so they have no visible effect.

| Option | Takes a value | Notes |
|---|---|---|
| `-N`, `--no-buffer` | no | output is not buffered to a terminal |
| `--no-progress-meter` | no | no progress meter in a library |
| `-#`, `--progress-bar` | no | no progress meter in a library |
| `-S`, `--show-error` | no | no stderr in a library; errors are exceptions |
| `-s`, `--silent` | no | no progress meter to hide; kept for `-w %{stderr}` and error text |

## Accepted and ignored

Each of these is parsed (its value is consumed, so the rest of the command is read correctly) and recorded in `CurlOptions.UnsupportedOptions`. Most belong to protocols CurlDotNet does not implement (SMTP, IMAP, TFTP, TELNET, SSH), to TLS backend tuning that .NET does not expose per request, or to curl tool features such as `--parallel` (transfers always run one after another), `--libcurl` and `--help`.

`--abstract-unix-socket`, `--alt-svc`, `-a`, `--append`, `--ca-native`, `--capath`, `--cert-status`, `--cert-type`, `--ciphers`, `--compressed-ssh`, `--create-file-mode`, `--crlf`, `--crlfile`, `--curves`, `--delegation`, `-q`, `--disable`, `--disallow-username-in-url`, `--dns-interface`, `--dns-ipv4-addr`, `--dns-ipv6-addr`, `--doh-cert-status`, `--doh-insecure`, `--doh-url`, `--dump-ca-embed`, `--ech`, `--egd-file`, `--engine`, `--false-start`, `--ftp-account`, `--ftp-alternative-to-user`, `--ftp-create-dirs`, `--ftp-method`, `-P`, `--ftp-port`, `--ftp-pret`, `--ftp-skip-pasv-ip`, `--ftp-ssl-ccc`, `--ftp-ssl-ccc-mode`, `--ftp-ssl-control`, `--ftp-ssl-reqd`, `--happy-eyeballs-timeout-ms`, `--haproxy-clientip`, `--haproxy-protocol`, `-h`, `--help`, `--hostpubmd5`, `--hostpubsha256`, `--hsts`, `--httpsig-algo`, `--httpsig-headers`, `--httpsig-key`, `--httpsig-keyid`, `--ip-tos`, `--ipfs-gateway`, `-4`, `--ipv4`, `-6`, `--ipv6`, `--keepalive-cnt`, `--key-type`, `--knownhosts`, `--krb`, `--krb4`, `--libcurl`, `-l`, `--list-only`, `--local-port`, `--login-options`, `--mail-auth`, `--mail-from`, `--mail-rcpt`, `--mail-rcpt-allowfails`, `-M`, `--manual`, `--metalink`, `--mptcp`, `--no-alpn`, `--no-npn`, `--no-sessionid`, `--ntlm-wb`, `-Z`, `--parallel`, `--parallel-immediate`, `--parallel-max`, `--pass`, `--pinnedpubkey`, `--preproxy`, `--proto`, `--proto-redir`, `--proxy-ca-native`, `--proxy-cacert`, `--proxy-capath`, `--proxy-cert`, `--proxy-cert-type`, `--proxy-ciphers`, `--proxy-crlfile`, `--proxy-dump-ca-embed`, `--proxy-http2`, `--proxy-insecure`, `--proxy-key`, `--proxy-key-type`, `--proxy-pass`, `--proxy-pinnedpubkey`, `--proxy-service-name`, `--proxy-ssl-allow-beast`, `--proxy-ssl-auto-client-cert`, `--proxy-tls13-ciphers`, `--proxy-tlsauthtype`, `--proxy-tlspassword`, `--proxy-tlsuser`, `--proxy-tlsv1`, `--pubkey`, `--random-file`, `--rate`, `--sasl-authzid`, `--sasl-ir`, `--service-name`, `--sigalgs`, `--socks5-basic`, `--socks5-gssapi`, `--socks5-gssapi-nec`, `--socks5-gssapi-service`, `--ssl`, `--ssl-allow-beast`, `--ssl-auto-client-cert`, `--ssl-no-revoke`, `--ssl-reqd`, `--ssl-revoke-best-effort`, `--ssl-sessions`, `-2`, `--sslv2`, `-3`, `--sslv3`, `--styled-output`, `--suppress-connect-headers`, `-t`, `--telnet-option`, `--tftp-blksize`, `--tftp-no-options`, `--tls-earlydata`, `--tls-max`, `--tls13-ciphers`, `--tlsauthtype`, `--tlspassword`, `--tlsuser`, `-1`, `--tlsv1`, `--tlsv1.0`, `--tlsv1.1`, `--tlsv1.2`, `--tlsv1.3`, `--trace-config`, `--trace-ids`, `--unix-socket`, `--upload-flags`, `-B`, `--use-ascii`, `-V`, `--version`, `--vlan-priority`, `--xattr`

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

Exit codes for bad command lines follow curl: a missing option value or two conflicting methods (`-I` with `-d`, `-F` with `-d`) is code 2, a bad URL or glob is 3, and a header containing CR or LF is 43. See the [curl exit code reference](curl-exit-codes.html).

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

On .NET 5 and later, CurlDotNet drives its own connection layer (`SocketsHttpHandler` with a connect callback). On netstandard2.0 and .NET Framework it relies on `HttpClient`'s parsed headers, so these options are not available there: `--request-target`, removing the `Host` header, sending blank headers, `--raw`, `-p` / `--proxytunnel` tunnels and a separate `--connect-timeout` (the overall `--max-time` still applies). See [framework compatibility](../framework-compatibility.html).

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
