---
title: "curl Transfer and File Errors in C#: Exit Codes 18, 23, 26, 52, 56"
description: "Fix curl transfer and file errors in C#: partial file (18), write error (23), read error (26), range (33), empty reply (52), receive (56), encoding (61), max-filesize (63) and FTP errors in CurlDotNet."
---

# curl Transfer and File Errors in C# (Exit Codes 8 to 100)

curl transfer and file errors in C# cover everything that goes wrong after the connection is up or around the local files a command reads and writes: a server that sends a broken or truncated reply, an output file that cannot be created, an input file that does not exist. CurlDotNet throws one `CurlException` subclass per case, with the same exit code in `CurlErrorCode` that the curl tool would return. This page lists each one, when it is thrown and how to fix it.

## At a Glance

| Code | Exception | Thrown when |
|-----:|-----------|-------------|
| 8 | `CurlWeirdServerReplyException` | The response head is malformed |
| 9 | `CurlDotNet.Core.CurlFtpException` | An FTP server answered with an error |
| 18 | `CurlPartialFileException` | The body was cut short |
| 23 | `CurlWriteErrorException` | An output, header or cookie-jar file cannot be written |
| 26 | `CurlReadErrorException` | An input file is missing or unreadable |
| 33 | `CurlRangeErrorException` | `-C` asked to resume but the server sent the whole file |
| 37 | `CurlFileCouldntReadException` | A single `-d @file` / `--json @file` is missing |
| 43 | `CurlBadFunctionArgumentException` | `-C` combined with a request body |
| 52 | `CurlGotNothingException` | The server closed the connection without replying |
| 56 | `CurlReceiveErrorException` | The connection failed while reading the response |
| 61 | `CurlBadContentEncodingException` | The body or transfer coding cannot be decoded |
| 63 | `CurlFileSizeExceededException` | `--max-filesize` was exceeded |
| 100 | `CurlTooLargeException` | The response headers are too many or too long |

All of them are in `CurlDotNet.Exceptions` except the FTP type. The complete list of every type and code is in the [error reference](../troubleshooting/error-reference.html).

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -o downloads/report.pdf https://httpbin.org/bytes/1024");
}
catch (CurlWriteErrorException ex)
{
    Console.WriteLine($"Cannot write {ex.FilePath} (exit code {ex.CurlErrorCode})");   // 23
}
catch (CurlException ex)
{
    Console.WriteLine($"{ex.GetType().Name} ({ex.CurlErrorCode}): {ex.Message}");
}
```

## Output Files: CurlWriteErrorException (23)

**Property:** `FilePath`.

Thrown when a file the command writes cannot be created or written:

- `-o`/`--output`, `-O`/`--remote-name`, `-D`/`--dump-header` or `-c`/`--cookie-jar` names a directory that does not exist
- the name is not a valid file name, the file is read-only, permission is denied, or the disk is full
- `--create-dirs` could not create the directory
- `-O -J` (`--remote-header-name`) would overwrite an existing file, which curl never does
- `--no-clobber` found the file and every numbered alternative (`file.1` to `file.100`) taken

```csharp
// Fails with 23: the directory does not exist
// await Curl.ExecuteAsync("curl -o out/data.json https://httpbin.org/json");

// Works: --create-dirs makes the directory first
var result = await Curl.ExecuteAsync("curl --create-dirs -o out/data.json https://httpbin.org/json");
Console.WriteLine(result.OutputFiles[0]);   // the path that was written
```

**Fix:** create the directory first or add `--create-dirs`, choose a writable path, or remove the old file. Remember that with `-o`/`-O` the body goes to the file and `result.Body` is empty, as curl writes nothing to standard output then.

## Input Files: CurlReadErrorException (26) and CurlFileCouldntReadException (37)

**Property:** `FilePath`.

`CurlReadErrorException` is thrown when a file named on the command line is missing or unreadable:

- `-T`/`--upload-file FILE`
- `-F 'name=@file'` and `-F 'name=<file'`
- `--data-urlencode @file` and `name@file`, `--url-query @file`, `--variable name@file`
- `-K`/`--config FILE`
- `-d @file` when the command has other `-d` items too
- `-` (standard input) for these options, which CurlDotNet does not read: save the input to a file instead
- a `.netrc` file with a quoted value that is never closed
- an I/O error while reading a `file://` URL

A single `-d @file`, `--data-binary @file` or `--json @file` whose file does not exist throws `CurlFileCouldntReadException` (37) instead, and so does a `file://` URL that cannot be opened for lack of permission. A `file://` URL that does not exist returns a `CurlResult` with `StatusCode` 404.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -F 'upload=@missing.bin' https://httpbin.org/post");
}
catch (CurlReadErrorException ex)
{
    Console.WriteLine($"Cannot read {ex.FilePath}");   // exit code 26
}
```

**Fix:** use an absolute path, or one relative to the process working directory (`Directory.GetCurrentDirectory()`), not to the source file. To send a value that starts with `@` or `<` literally, use `--form-string` or `--data-raw`.

## Broken Replies

### CurlPartialFileException (18)

**Properties:** `Url`, `BytesReceived`, `BytesExpected` (`null` for a chunked body).

The server announced `Content-Length: N` but closed the connection earlier, or a chunked body ended without its final zero-length chunk. Whatever arrived is still written to the `-o` file unless `--remove-on-error` is given.

**Fix:** retry (`--retry 3`), or resume the download with `-C -`. `--ignore-content-length` makes CurlDotNet accept whatever arrived as the complete body.

### CurlWeirdServerReplyException (8)

The response head is malformed: an invalid header line, a status code curl rejects, or a NUL byte in a chunked trailer. This is a server (or intermediary) bug; inspect the raw reply with `-v` or `--trace-ascii -`. A status line in an HTTP version curl does not speak is `CurlUnsupportedProtocolException` (1) instead; see [URL errors](url-errors.html).

### CurlGotNothingException (52)

The server (or the proxy) accepted the connection and the request, then closed it without sending a single byte. Common causes are the wrong port, a TLS port reached with `http://`, or a server that crashed on the request.

**Fix:** check the server logs, try `-v`, or retry with `--retry`.

### CurlReceiveErrorException (56)

The connection failed while the response was being read, or the response headers of one transfer (redirect hops included) add up to more than 300 KB.

**Fix:** usually transient: retry with `--retry`. Persistent failures point at the server or a proxy cutting connections.

### CurlTooLargeException (100)

The response has more than 5000 header fields or a single header line over 100 KB. curl refuses such responses to protect the client; no option accepts them.

### CurlBadContentEncodingException (61)

**Property:** `Encoding`.

Thrown when a `--compressed` body announced as `gzip`, `deflate` or `br` cannot be decoded, or when the `Transfer-Encoding` header breaks curl's rules: `chunked` is not the last coding, an unknown coding is used, more than five codings are stacked, or `gzip`/`deflate` is used without `--tr-encoding`.

**Fix:** report the broken encoding to the server owner; drop `--compressed` to keep the raw bytes.

## Size and Resume Limits

### CurlFileSizeExceededException (63)

**Properties:** `MaxSize`, `ActualSize`.

The response is larger than `--max-filesize` allows, either because `Content-Length` announced it or because more bytes arrived while downloading.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --max-filesize 1k https://httpbin.org/bytes/4096");
}
catch (CurlFileSizeExceededException ex)
{
    Console.WriteLine($"{ex.ActualSize} bytes is over the {ex.MaxSize}-byte limit");
}
```

### CurlRangeErrorException (33)

**Properties:** `Offset`, `StatusCode`.

`-C N` or `-C -` (`--continue-at`) asked the server for the rest of a file, but the server answered with the whole document (no `Content-Range`). curl refuses to append a full response to a partial file because it would corrupt it.

**Fix:** download the whole file again without `-C`, or use a server that supports byte ranges.

### CurlBadFunctionArgumentException (43) for Resumed Uploads

`-C`/`--continue-at` together with a request body (`-d`, `-F` or `-T`) on an HTTP URL throws `CurlBadFunctionArgumentException` before anything is sent: an HTTP upload cannot be resumed. Remove `-C` to send the whole body. The same type is used for CR/LF in headers; see [URL and command errors](url-errors.html).

## FTP Errors

`ftp://` URLs go through the FTP handler. When the server answers with an error reply, it throws `CurlDotNet.Core.CurlFtpException` (exit code 9) with the server's reply code in `FtpStatusCode`; a connection failure is `CurlCouldntConnectException` (7) and an invalid URL `CurlMalformedUrlException` (3).

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -u anonymous:guest ftp://ftp.example.com/missing.txt");
}
catch (CurlDotNet.Core.CurlFtpException ex)
{
    Console.WriteLine($"FTP {ex.FtpStatusCode}: {ex.Message}");
}
```

**Fix:** check the path, the `-u user:password` credentials (anonymous is used without `-u`) and that the server allows the operation. On a runtime without `FtpWebRequest`, an `ftp://` URL throws a plain `CurlException` with code 1.

## Reserved Types

These types exist for parity with curl's error list but nothing in the library constructs them today:

| Type | `CurlErrorCode` |
|------|----:|
| `CurlDotNet.Exceptions.CurlFtpException` (property `FtpCode`) | 9 |
| `CurlRemoteAccessDeniedException` | 9 |
| `CurlFtpAcceptFailedException` | 10 |
| `CurlFtpWeirdPassReplyException` | 11 |
| `CurlFtpAcceptTimeoutException` (derives from `CurlTimeoutException`) | 28 |
| `CurlUploadFailedException` | 25 |
| `CurlHttpPostErrorException` | 34 |
| `CurlBadDownloadResumeException` | 36 |
| `CurlSendErrorException` | 55 |
| `CurlFileException` | none |
| `CurlParsingException` | none |

## What the Command Printed Before It Failed

curl prints as it goes, so a failed transfer may already have produced output: headers from `-i`, earlier URLs of the same command, `-w` text. CurlDotNet keeps that text on the exception in `StandardOutput` (also `ex.Data["CurlDotNet.Stdout"]`), and the `-w` part alone in `ex.Data["CurlDotNet.WriteOut"]`:

```csharp
try
{
    await Curl.ExecuteAsync("curl -w '%{exitcode} %{errormsg}' --max-filesize 1k https://httpbin.org/bytes/4096");
}
catch (CurlException ex)
{
    Console.WriteLine(ex.Data[CurlException.WriteOutDataKey]);   // "63 File size 4096 exceeded maximum 1024"
}
```

## Related Documentation

- [Error reference: every exception and exit code](../troubleshooting/error-reference.html)
- [curl exit codes](../reference/curl-exit-codes.html)
- [Timeout errors](timeout-errors.html)
- [HTTP errors](http-errors.html)
- [Exception reference](index.html)
