---
title: "Beginner curl Recipes in C# - CurlDotNet Cookbook"
description: "Beginner curl recipes in C# with CurlDotNet: GET requests, POST JSON, form data, file upload and download, error handling and calling a REST API."
---

# Beginner curl recipes in C#

These beginner curl recipes in C# cover the first things most people need from CurlDotNet: fetch a URL, send JSON or a form, move files up and down, handle errors and talk to a REST API. Work through them in order, or jump to the one that matches your task.

## Contents

1. [curl GET request in C#](simple-get.html) - run a curl command, check the status code, read headers and the body
2. [curl -X POST JSON in C#](send-json.html) - `-d` with a `Content-Type` header, `--json`, the `WithJson` builder, `ParseJson<T>()`
3. [curl -d form data POST in C#](post-form.html) - URL-encoded forms, `--data-urlencode`, `WithFormData`
4. [curl -F file upload in C#](upload-file.html) - multipart/form-data, extra fields, `-T`/`--upload-file` PUT uploads, `--data-binary`
5. [curl download file in C# (curl -o)](download-file.html) - `-o`, `-O`, `-J`, `--output-dir`, resuming with `-C -`, `Curl.DownloadAsync`, `SaveToFile`
6. [Handle curl errors and exceptions in C#](handle-errors.html) - HTTP status results, `-f`, `EnsureSuccess()`, timeouts, connection failures, cancellation, `--retry`
7. [Call a REST API with curl in C#](call-api.html) - bearer tokens, all the verbs, a reusable `LibCurl` client

## Next

The [one-flag-at-a-time recipes](../index.html#one-flag-at-a-time) cover redirects, cookies, proxies, timeouts, retries, HEAD requests, PUT/DELETE/PATCH and authentication schemes. When a command fails with a curl exit code, the [curl exit code reference](../../reference/curl-exit-codes.html) lists the matching exception.
