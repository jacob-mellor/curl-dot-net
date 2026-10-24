---
title: "How to Do Common curl Tasks in C# (.NET Guides)"
description: "How to do common curl tasks in C#: run curl commands, POST JSON, authenticate, upload, download, and use CurlDotNet in ASP.NET Core and serverless."
---

# How to do common curl tasks in C#

Each guide below answers one curl-in-C# question with a curl command you can paste into `Curl.ExecuteAsync`, the equivalent fluent `CurlRequestBuilder` code, and the gotchas we found when running it against CurlDotNet 9.8.0.

```bash
dotnet add package CurlDotNet
```

## Getting started

| Guide | Question it answers |
|---|---|
| [Run a curl command in C#](run-curl-command-in-csharp/README.md) | How do I run a curl command in C#? |
| [Convert curl to C#](convert-curl-to-csharp/README.md) | How do I turn a curl command into HttpClient, Python, fetch or PowerShell code? |
| [Run curl synchronously](curl-synchronously-in-csharp/README.md) | How do I make a blocking call with `Curl.Execute` in a console app or script? |
| [How CurlDotNet works](how-curldotnet-works/README.md) | What happens between the curl string and the HTTP request? |
| [curl vs HttpClient](curl-vs-httpclient-csharp/README.md) | When should I use CurlDotNet and when raw HttpClient? |

## Requests and data

| Guide | Question it answers |
|---|---|
| [POST JSON](curl-post-json-csharp/README.md) | How do I send `curl -X POST` JSON in C#? |
| [PUT, DELETE and PATCH](curl-put-delete-patch-csharp/README.md) | How do I send the other REST verbs? |
| [HEAD requests](curl-head-request-csharp/README.md) | How do I run `curl -I` to read headers only? |
| [Custom headers and User-Agent](curl-custom-headers-user-agent-csharp/README.md) | How do I set `-H`, `-A`, `-e` and `-b`? |
| [Multipart upload with -F](curl-file-upload-csharp/README.md) | How do I upload a file with `curl -F`? |
| [Download a file with -o](curl-download-file-csharp/README.md) | How do I download a file, binary-safe? |
| [Parse JSON responses](parse-json-from-curl-in-csharp/README.md) | How do I turn the response into C# objects? |

## Authentication and security

| Guide | Question it answers |
|---|---|
| [Bearer token](curl-bearer-token-csharp/README.md) | How do I send `Authorization: Bearer`? |
| [Basic auth with -u](curl-basic-auth-csharp/README.md) | How do I use `curl -u`, `--digest` or `--ntlm`? |
| [Ignore SSL errors with -k](curl-ignore-ssl-certificate-csharp/README.md) | How do I use `curl -k`, and when is it safe? |

## Network behavior

| Guide | Question it answers |
|---|---|
| [Follow redirects with -L](curl-follow-redirects-csharp/README.md) | How do I follow 3xx redirects? |
| [Timeouts with -m](curl-timeout-csharp/README.md) | How do I set a timeout or cancel a request? |
| [Proxy with -x](curl-proxy-csharp/README.md) | How do I go through an HTTP or SOCKS5 proxy? |
| [Error handling](curl-error-handling-csharp/README.md) | What is thrown, what is not, and how do I retry with `--retry`? |

## Application types

| Guide | Question it answers |
|---|---|
| [ASP.NET Core](curl-in-aspnet-core/README.md) | How do I register CurlDotNet in DI and call it from controllers or minimal APIs? |
| [Blazor and .NET MAUI](curl-in-blazor-maui/README.md) | How do I use it in Blazor Server, WebAssembly and MAUI? |
| [Azure Functions and AWS Lambda](curl-in-azure-functions-aws-lambda/README.md) | How do I use it in serverless functions? |

## PowerShell

Using CurlDotNet from PowerShell or `.csx` scripts? See [Using CurlDotNet from PowerShell and .NET scripts](../powershell/README.md).

## About CurlDotNet

CurlDotNet is a pure C# library (no curl.exe, no native libcurl, no P/Invoke) for netstandard2.0, .NET Framework 4.7.2 and 4.8, and .NET 8, 9 and 10. It is MIT licensed, written by Jacob Mellor and sponsored by Iron Software. Source: [github.com/jacob-mellor/curl-dot-net](https://github.com/jacob-mellor/curl-dot-net).
