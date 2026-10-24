---
title: "How to Do a curl -F Multipart Upload in C#"
description: "curl -F multipart upload in C#: run curl -F 'file=@report.pdf' with CurlDotNet, or use WithMultipartForm to send files and form fields together."
---

# How to do a curl -F multipart upload in C#

To do a `curl -F` multipart upload in C#, pass `-F 'field=@path/to/file'` (plus any `-F 'name=value'` text fields) to `Curl.ExecuteAsync`, or call `CurlRequestBuilder.Post(url).WithMultipartForm(fields, files)`. The request is sent as `multipart/form-data`, like the curl CLI.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -F 'file=@report.txt' \
         -F 'description=Quarterly report' \
         https://httpbin.org/post
    """);

Console.WriteLine(result.StatusCode);  // 200
Console.WriteLine(result.Body);        // httpbin echoes "files" and "form"
```

`@` means "read this file". Relative paths resolve against the process's current directory (`Environment.CurrentDirectory`), so use absolute paths in services. A missing file throws `CurlReadErrorException` (curl exit code 26) before anything is sent.

The request body follows curl's format: curl's boundary style, quoted field names, no `Content-Type` on text fields, and for files a `filename=` plus a content type guessed from the extension.

## Files and fields with the fluent builder

`WithMultipartForm` takes two dictionaries: text fields, and field name to file path.

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithMultipartForm(
        fields: new Dictionary<string, string> { ["description"] = "Quarterly report" },
        files:  new Dictionary<string, string> { ["file"] = "report.txt" })
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

For a single file with no other fields, `.WithFile("file", "report.txt")` is shorter. `WithFormData` builds a URL-encoded body, not multipart fields, so combining it with `WithFile` drops the text fields; use `WithMultipartForm` whenever you need both.

## Upload several files

Repeat `-F`, or add more entries to the files dictionary:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -F 'invoice=@invoice.pdf' -F 'receipt=@receipt.png' https://httpbin.org/post");

Console.WriteLine(result.StatusCode);
```

## Raw body upload instead of multipart

Some APIs (S3 pre-signed URLs, many blob stores) want the file as the raw request body rather than a form. Use `-T` / `--upload-file`, which sends a `PUT` with the file's bytes (when the URL ends in `/`, the file name is appended, as in curl):

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -T photo.jpg -H 'Content-Type: image/jpeg' https://httpbin.org/put");

Console.WriteLine(result.StatusCode);
```

`-X PUT --data-binary @photo.jpg` sends the same bytes. Use `--data-binary` rather than `-d @file`: `-d` strips carriage returns, newlines and NUL bytes from the file, as curl does.

With the builder, read the bytes yourself and call `.WithBinaryData(File.ReadAllBytes("photo.jpg"))`.

## Field modifiers

curl's `-F` syntax is supported in full:

- `;type=` and `;filename=` after `@path`, as in `-F 'file=@a.jpg;type=image/jpeg;filename=avatar.jpg'`, and `;headers=` for extra part headers.
- `-F 'name=<file'` sends the file's contents as a text field rather than as a file part.
- `--form-string 'name=@literal'` sends a value that starts with `@` or `<` as-is.
- `--form-escape` escapes field and file names the way curl's option does.

## Related

- [POST JSON](../curl-post-json-csharp/README.md)
- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [PUT, PATCH and DELETE](../curl-put-delete-patch-csharp/README.md)
- [Timeouts for large uploads](../curl-timeout-csharp/README.md)
- [All how-to guides](../README.md)
