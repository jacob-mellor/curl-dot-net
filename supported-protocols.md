# Supported Protocols

## Overview

CurlDotNet supports various network protocols for data transfer. This document lists all supported protocols and their usage.

## Core Protocols

### HTTP/HTTPS
Most common web protocols.

```csharp
// HTTP
var result = await curl.ExecuteAsync("curl http://example.com");

// HTTPS (secure)
var result = await curl.ExecuteAsync("curl https://example.com");
```

### FTP/FTPS
File Transfer Protocol.

```csharp
// FTP
var result = await curl.ExecuteAsync("curl ftp://ftp.example.com/file.txt");

// FTPS (secure)
var result = await curl.ExecuteAsync("curl ftps://ftp.example.com/file.txt");
```

### FILE
Local file access.

```csharp
var result = await curl.ExecuteAsync("curl file:///path/to/local/file.txt");
```

## Protocol Features

### HTTP/2
```csharp
// Force HTTP/2
var result = await curl.ExecuteAsync("curl --http2 https://example.com");
```

### HTTP/3 (QUIC)
```csharp
// Try HTTP/3 with fallback
var result = await curl.ExecuteAsync("curl --http3 https://example.com");
```

## Protocol Selection

```csharp
public string GetProtocolFromUrl(string url)
{
    var uri = new Uri(url);
    return uri.Scheme switch
    {
        "http" => "HTTP",
        "https" => "HTTPS",
        "ftp" => "FTP",
        "ftps" => "FTPS",
        "file" => "FILE",
        _ => "Unknown"
    };
}
```

## Security Considerations

- Always prefer HTTPS over HTTP
- Use FTPS instead of FTP when possible
- Validate certificates for secure protocols

## Protocol-Specific Options

### HTTP/HTTPS Options
- Headers: `-H "Header: Value"`
- Methods: `-X GET|POST|PUT|DELETE`
- Data: `-d "data"`

### FTP Options
- Authentication: `-u user:pass`
- Upload: `-T file`
- List: `-l`

## Related Documentation
- [URL Syntax Errors](exceptions/url-errors.md)
- [SSL/TLS Errors](exceptions/ssl-errors.md)
- [Protocol Specifications](https://curl.se/docs/protdocs.html)