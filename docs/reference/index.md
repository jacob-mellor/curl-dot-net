---
title: "CurlDotNet Reference - libcurl Mapping, curl Flags and Exit Codes in C#"
description: "CurlDotNet reference pages: documentation map, libcurl options mapped to C#, every curl CLI flag and its status, curl exit codes, and how CurlDotNet is tested against curl's own test suite."
---

# CurlDotNet Reference: libcurl Mapping, curl CLI Flags and Exit Codes in C#

CurlDotNet reference pages answer five questions: where each piece of documentation lives, how libcurl options map to C# methods, what CurlDotNet does with each curl CLI flag, which exception each curl exit code becomes, and how compatibility with curl is tested.

- [Documentation Overview](documentation.html) - Map of every documentation section with direct links
- [libcurl Mapping](libcurl-mapping.html) - libcurl `CURLOPT_*`, `CURLINFO_*` and `CURLE_*` mapped to CurlDotNet methods, properties and exceptions
- [curl CLI Compatibility](curl-cli-compatibility.html) - Generated table of all curl 8.7.1 options (implemented, parsed but not applied, ignored), command-line syntax and shell quoting on Linux, macOS and Windows
- [curl Exit Codes in C#](curl-exit-codes.html) - Every curl exit code CurlDotNet produces, the exception type it throws and how to fix it
- [curl Test Suite](curl-test-suite.html) - How CurlDotNet replays 1092 of curl's own test files, how to run them and read the report

Related top-level pages:

- [Framework Compatibility](../framework-compatibility.html) - .NET Framework 4.7.2 and 4.8, .NET Standard 2.0, .NET 8/9/10
- [Supported Protocols](../supported-protocols.html) - HTTP, HTTPS, FTP, FTPS and file URLs
- [API Guide](../api-guide/README.html) - String API, builder API and LibCurl instance API
