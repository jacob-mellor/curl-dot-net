---
title: Protocols Supported by CurlDotNet (HTTP, HTTPS, FTP) in C#
description: Protocols supported by CurlDotNet in C#: HTTP and HTTPS in full, FTP and FTPS downloads, uploads and listings, and file:// URLs. Other schemes throw.
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

TLS is handled by the .NET runtime. `-k` / `--insecure` disables certificate validation for development:

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

Binary downloads land in `result.BinaryData`; text downloads and listings in `result.Body`. FTP server errors throw `CurlFtpException` (with `FtpStatusCode`); a host that cannot be reached throws `CurlCouldntConnectException`. `-T` (upload a local file) is not implemented yet; use `-d @file` style bodies or `-X STOR` with `-d`.

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

`sftp://`, `scp://`, `smtp://`, `imap://`, `ldap://`, `telnet://` and other curl-supported schemes are not implemented. Executing one throws `CurlDotNet.Exceptions.CurlUnsupportedProtocolException`:

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

HTTP/2 and HTTP/3 selection flags (`--http2`, `--http3`) are not supported yet; the runtime negotiates HTTP/1.1 or HTTP/2 on its own.

## Security Considerations

- Prefer HTTPS over HTTP and FTPS over FTP.
- Use `-k` only against development servers.
- Pass credentials through environment variables (`-u $FTP_USER:$FTP_PASS`) rather than hard-coding them.

## Related Documentation

- [curl CLI Compatibility](reference/curl-cli-compatibility.html) - supported flags
- [URL Errors](exceptions/url-errors.html) - malformed and unsupported URLs
- [SSL/TLS Errors](exceptions/ssl-errors.html)
- [curl protocol documentation](https://curl.se/docs/protdocs.html)
