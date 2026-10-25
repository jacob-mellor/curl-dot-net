---
title: "curl -w write-out in C# (%{http_code}, %{time_total}, %{json})"
description: "Use curl -w / --write-out in C# with CurlDotNet: every curl variable, %{json}, %header{name}, %output{file}, %{stderr}, @file formats, and reading -w output when a transfer fails."
---

# curl -w write-out in C#

`-w` / `--write-out` prints details about a finished transfer, such as the status code or how long it took. CurlDotNet supports curl's complete write-out syntax in C#: the text is expanded after the transfer and appended to `result.Body` exactly where curl would print it, after the body and with no extra newline.

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -s -o page.html -w '%{http_code} %{time_total}s %{size_download} bytes' https://httpbin.org/html");

Console.WriteLine(result.Body);   // 200 0.412345s 3741 bytes
```

With `-o` the body goes to the file and `Body` holds only the `-w` text, which makes `-o FILE -w FORMAT` the easiest way to get just the numbers. Without `-o`, `Body` is the response body followed by the `-w` text, so start the format with `\n` if you want it on its own line.

## Variables

Every variable curl documents for HTTP transfers is available, including `%{num_retries}` (how many `--retry` attempts were made; with `--retry` the `-w` text is written once, for the final attempt). The most used:

| Variable | Value |
|----------|-------|
| `%{http_code}`, `%{response_code}` | final HTTP status code |
| `%{http_version}` | `1.1`, `2` ... |
| `%{url}`, `%{url_effective}` | the URL as given, and the last one fetched (after `-L`) |
| `%{redirect_url}` | the `Location` that was not followed (without `-L`) |
| `%{num_redirects}`, `%{num_connects}`, `%{num_headers}` | counts for the transfer |
| `%{content_type}` | the response `Content-Type` |
| `%{method}`, `%{scheme}` | the request method and URL scheme of the last request |
| `%{size_download}`, `%{size_upload}`, `%{size_header}`, `%{size_request}` | bytes |
| `%{speed_download}`, `%{speed_upload}` | bytes per second |
| `%{time_namelookup}`, `%{time_connect}`, `%{time_appconnect}`, `%{time_pretransfer}`, `%{time_starttransfer}`, `%{time_redirect}`, `%{time_total}` | seconds, six decimals like curl |
| `%{local_ip}`, `%{local_port}`, `%{remote_ip}`, `%{remote_port}` | connection end points |
| `%{exitcode}`, `%{errormsg}` | curl exit code and error text (0 and empty on success) |
| `%{filename_effective}` | the file `-o` / `-O` / `-J` wrote |
| `%{urlnum}` | index of the URL in a multi-URL command, from 0 |
| `%header{name}` | the value of one response header |
| `%{url.host}`, `%{url.path}`, `%{url.query}` ... | parts of the URL; `%{urle.*}` gives the effective URL's parts |

`%%` prints a percent sign, and `\n`, `\r` and `\t` are expanded. An unknown variable expands to nothing, as in curl.

## Everything as JSON

`%{json}` prints all variables as one JSON object in curl's variable order, and `%{header_json}` the response headers. As in curl, numeric variables (`http_code`, `size_*`, `time_*`, `num_*` ...) are JSON numbers and a variable with no value is `null`, not a quoted string, so `GetInt32()` and `GetDouble()` work directly:

```csharp
using System.Text.Json;
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -s -o get.json -w '%{json}' https://httpbin.org/get");

using var doc = JsonDocument.Parse(result.Body);
var root = doc.RootElement;
Console.WriteLine(root.GetProperty("http_code").GetInt32());     // 200
Console.WriteLine(root.GetProperty("time_total").GetDouble());   // seconds
```

## Send output elsewhere

| Syntax | Effect |
|--------|--------|
| `%{stderr}` | the text after it goes to standard error (or the `--stderr FILE`), not to `Body` |
| `%{stdout}` | switches back to `Body` |
| `%output{file}` | the text after it is written to `file`; `%output{>>file}` appends |
| `%{onerror}` | the text after it is printed only when the transfer failed |
| `-w @format.txt` | reads the format from a file |

```csharp
using CurlDotNet;

// Append one CSV line per request to a log file, keep Body clean
var result = await Curl.ExecuteAsync(
    "curl -s -w '%output{>>timings.csv}%{url_effective},%{http_code},%{time_total}\\n' https://httpbin.org/get");
Console.WriteLine(File.ReadAllText("timings.csv"));
```

## When the transfer fails

curl still expands `-w` when a transfer fails, which is where `%{exitcode}` and `%{errormsg}` are useful. CurlDotNet throws an exception instead of returning a result, so the text travels on the exception:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -f -s -w '%{http_code} exit=%{exitcode}' https://httpbin.org/status/503");
}
catch (CurlException ex)
{
    Console.WriteLine(ex.Data[CurlException.WriteOutDataKey]);   // 503 exit=22
    Console.WriteLine(ex.StandardOutput);                        // everything curl would have printed, -w included
}
```

`CurlException.StandardOutput` is the complete standard output (`-i` headers, a `--fail-with-body` page, earlier URLs of the same command, and the `-w` text). `ex.Data["CurlDotNet.WriteOut"]` (the `CurlException.WriteOutDataKey` constant) holds only the `-w` part.

## Several URLs

With several URLs, globs or `--next` groups, `-w` is expanded after each transfer and `%{urlnum}` tells them apart:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -s -o 'out#1.txt' -w '%{urlnum} %{http_code}\\n' 'https://httpbin.org/status/{200,204}'");
Console.Write(result.Body);
// 0 200
// 1 204
```

## Without -w

The same numbers are on the result as properties when you do not need curl's text format: `result.StatusCode`, `result.Headers` and `result.Timings` (`Total`, `StartTransfer`, `Connect` and so on, in seconds).

## Related

- [curl multiple URLs and URL globbing in C#](multiple-urls-and-globbing.html)
- [curl --retry in C#](retry.html)
- [curl exit codes in C#](../reference/curl-exit-codes.html)
- [Debugging requests](../tutorials/14-debugging-requests.html)
- [Cookbook index](index.html)
