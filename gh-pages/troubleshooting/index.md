# Troubleshooting CurlDotNet

Quick access to troubleshooting guides for common issues with CurlDotNet.

## 🔧 Troubleshooting by Category

### [Connection Issues](connection-errors.md)
- Network unreachable
- Connection refused
- DNS resolution failures
- Proxy configuration

### [Authentication Problems](/exceptions/auth-errors.md)
- 401 Unauthorized
- Invalid API keys
- Token expiration
- OAuth issues

### [SSL/TLS Errors](/exceptions/ssl-errors.md)
- Certificate validation
- Self-signed certificates
- TLS version mismatches
- Cipher suite problems

### [Timeout Issues](/exceptions/timeout-errors.md)
- Operation timeouts
- Connection timeouts
- Read timeouts
- Large file downloads

### [HTTP Errors](/exceptions/http-errors.md)
- 404 Not Found
- 500 Internal Server Error
- Rate limiting (429)
- Bad requests (400)

## 🚀 Quick Fixes

### Enable Verbose Mode
```csharp
// See what's happening under the hood
var result = await Curl.ExecuteAsync("curl -v https://api.example.com");
Console.WriteLine(result.VerboseOutput);
```

### Skip SSL Verification (Dev Only!)
```csharp
// WARNING: Only for development
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001");
```

### Increase Timeout
```csharp
// Give slow servers more time
var result = await Curl.ExecuteAsync("curl --max-time 60 https://slow-api.example.com");
```

### Use Proxy
```csharp
// Route through corporate proxy
var result = await Curl.ExecuteAsync("curl -x proxy:8080 https://api.example.com");
```

## 📋 Diagnostic Checklist

Before reporting an issue, check:

- [ ] URL is correct and accessible
- [ ] Network connectivity is working
- [ ] Authentication credentials are valid
- [ ] SSL certificates are valid
- [ ] Firewall/proxy settings are correct
- [ ] Request format matches API documentation

## 🔍 Debug Tools

### 1. Verbose Output
Get detailed information about the request:
```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.example.com");
```

### 2. Trace Network
See exact data sent/received:
```csharp
var result = await Curl.ExecuteAsync("curl --trace - https://api.example.com");
```

### 3. Show Headers Only
Debug response headers:
```csharp
var result = await Curl.ExecuteAsync("curl -I https://api.example.com");
```

## 📚 Documentation

- [Complete Troubleshooting Guide](README.md)
- [Common Issues](common-issues.md)
- [Exception Reference](/exceptions/)
- [API Documentation](/api/)

## 💬 Getting Help

1. Check the documentation above
2. Search [existing issues](https://github.com/jacob-mellor/curl-dot-net/issues)
3. Ask on [Stack Overflow](https://stackoverflow.com/questions/tagged/curldotnet)
4. Create a [new issue](https://github.com/jacob-mellor/curl-dot-net/issues/new)

---
*CurlDotNet Troubleshooting Guide*