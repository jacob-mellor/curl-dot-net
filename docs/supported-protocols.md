---
title: "Protocols Supported by CurlDotNet (HTTP, HTTPS, FTP) in C#"
description: "Protocols supported by CurlDotNet in C#: HTTP and HTTPS in full, FTP and FTPS downloads, uploads and listings, and file:// URLs. Other schemes throw."
---

# Protocols Supported by CurlDotNet (HTTP, HTTPS, FTP) in C#

The protocols supported by CurlDotNet are HTTP, HTTPS, FTP, FTPS and `file://`. HTTP and HTTPS are the primary, fully featured path; FTP/FTPS and `file://` have dedicated handlers with a smaller option set. Any other URL scheme throws `CurlUnsupportedProtocolException`.

## HTTP and HTTPS

All curl HTTP options described in [curl CLI Compatibility](reference/curl-cli-compatibility.html) apply: methods, headers, request bodies, forms, authentication, redirects, proxies, timeouts, cookies and output files.

```csharp
using CurlDotNet;

var http = await Curl.ExecuteAsync("curl http://httpbin.org/get");
var https = await Curl.ExecuteAsync("curl https://httpbin.org/get");
Console.WriteLine($"{http.StatusCode} {https.StatusCode}");
```

The HTTP version flags are applied to each request: `--http1.0` and `--http1.1` pin the version, `--http2` and `--http3` ask for HTTP/2 or HTTP/3 and allow a lower version when the server does not offer it, and `--http2-prior-knowledge` and `--http3-only` require it. On .NET 5 and later this sets `HttpRequestMessage.Version` and `VersionPolicy`; on .NET Standard 2.0 and .NET Framework only the version is set and the runtime's `HttpClient` decides. HTTP/3 also needs QUIC support in the operating system.

TLS is handled by the .NET runtime against the operating system's trust store (`--cacert`, `--cert` and `--key` are parsed but not applied). `-k` / `--insecure` disables certificate validation for development:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -k https://self-signed.badssl.com/");
Console.WriteLine(result.StatusCode);
```

## FTP and FTPS

FTP and FTPS are served by `FtpHandler`, built on the runtime's `FtpWebRequest`. The operation is chosen from the command:

| Command shape | FTP operation |
|---------------|---------------|
| `curl ftp://host/path/file.txt` | Download the file (`RETR`) |
| `curl ftp://host/path/` (trailing slash) | List the directory (`NLST`; `-v` gives the detailed `LIST`) |
| `curl -T local.txt ftp://host/path/file.txt` | Upload a local file byte for byte (`STOR`) |
| `curl -d 'content' ftp://host/path/file.txt` | Upload the string body (`STOR`) |
| `curl -X DELE ftp://host/path/file.txt` | Custom FTP command: `DELE`, `MKD`, `RMD`, `PWD`, `SIZE`, `MDTM`, `LIST`, `NLST`, `RETR`, `STOR` |

Supported options: `-u user:pass` (anonymous login is used otherwise), `-o file` to save the download, `--connect-timeout`, `-x proxy`, `--ftp-pasv` and `--ftp-ssl`. An `ftps://` URL enables TLS automatically.

```csharp
using CurlDotNet;

var listing = await Curl.ExecuteAsync("curl -u demo:password ftp://test.rebex.net/");
Console.WriteLine(listing.Body);

var file = await Curl.ExecuteAsync(
    "curl -u demo:password -o readme.txt ftp://test.rebex.net/readme.txt");
Console.WriteLine(file.OutputFiles[0]);
```

Binary downloads land in `result.BinaryData`; text downloads and listings in `result.Body`. FTP server errors throw `CurlFtpException` (with `FtpStatusCode`); a host that cannot be reached throws `CurlCouldntConnectException`; a missing `-T` file throws `CurlReadErrorException` (exit code 26). FTP goes through `FtpWebRequest`, so the HTTP-only options (`--retry`, `-w`, cookies, resume with `-C`) do not apply to it, and on a runtime without `FtpWebRequest` an FTP URL fails with exit code 1.

## Local Files (file://)

`file://` URLs read a local file and return it as a result with status 200 (or 404 if the file is missing):

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl file:///etc/hosts");
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.GetHeader("Content-Length"));
Console.WriteLine(result.Body);
```

Binary files are returned in `BinaryData`; `-o` copies the file to the output path.

## Unsupported Schemes

`sftp://`, `scp://`, `smtp://`, `imap://`, `pop3://`, `ldap://`, `telnet://`, `dict://`, `tftp://`, `mqtt://` and the other non-HTTP, non-FTP schemes curl supports are not implemented. Executing one throws `CurlDotNet.Exceptions.CurlUnsupportedProtocolException` (curl exit code 1):

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl sftp://example.com/file");
}
catch (CurlUnsupportedProtocolException ex)
{
    Console.WriteLine(ex.Message);
}
```

A URL without a scheme gets `http://`, as in curl (`ftp.` host names get `ftp://`), and `--proto-default` changes that default.

## Security Considerations

- Prefer HTTPS over HTTP and FTPS over FTP.
- Use `-k` only against development servers.
- Pass credentials through environment variables (`-u $FTP_USER:$FTP_PASS`) rather than hard-coding them.

## Related Documentation

- [curl CLI Compatibility](reference/curl-cli-compatibility.html) - supported flags
- [URL Errors](exceptions/url-errors.html) - malformed and unsupported URLs
- [SSL/TLS Errors](exceptions/ssl-errors.html)
- [curl protocol documentation](https://curl.se/docs/protdocs.html)
