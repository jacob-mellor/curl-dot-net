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

`@` means "read this file". Relative paths resolve against the process's current directory (`Environment.CurrentDirectory`), so use absolute paths in services. A missing file throws an `IOException` (for example `FileNotFoundException`) before anything is sent.

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

For a single file with no other fields, `.WithFile("file", "report.txt")` is shorter. In version 9.7.0, combining `WithFile` with `WithFormData` drops the text fields, so use `WithMultipartForm` whenever you need both.

## Upload several files

Repeat `-F`, or add more entries to the files dictionary:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -F 'invoice=@invoice.pdf' -F 'receipt=@receipt.png' https://httpbin.org/post");

Console.WriteLine(result.StatusCode);
```

## Raw body upload instead of multipart

Some APIs (S3 pre-signed URLs, many blob stores) want the file as the raw request body rather than a form. Use `--data-binary @file`:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -X PUT --data-binary @photo.jpg -H 'Content-Type: image/jpeg' https://httpbin.org/put");

Console.WriteLine(result.StatusCode);
```

With the builder, read the bytes yourself and call `.WithBinaryData(File.ReadAllBytes("photo.jpg"))`.

## Limitations

- The `;type=` and `;filename=` modifiers after `@path` (as in `-F 'file=@a.jpg;type=image/jpeg'`) are not parsed in 9.7.0; they are read as part of the file name.
- `-T` / `--upload-file` is not supported; use `-X PUT --data-binary @file`.

## Related

- [POST JSON](../curl-post-json-csharp/README.md)
- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [PUT, PATCH and DELETE](../curl-put-delete-patch-csharp/README.md)
- [Timeouts for large uploads](../curl-timeout-csharp/README.md)
- [All how-to guides](../README.md)
