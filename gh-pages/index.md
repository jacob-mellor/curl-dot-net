---
layout: default
title: CurlDotNet - Pure .NET curl for C#
---

# CurlDotNet Documentation

A pure .NET implementation of curl for C#. No native dependencies, just clean C# code.

## Quick Start

```csharp
// Simple GET request
var response = await Curl.GetAsync("https://api.example.com/data");
Console.WriteLine(response.Body);
```

## 🆕 New to curl?

**[Start Here: New to curl? Complete Guide →](new-to-curl.md)**

Learn what curl is, how it works, and how to use curl commands in C# with comprehensive examples.

## Documentation

### Essential Guides
- **[New to curl? Complete Guide](new-to-curl.md)** - Start here if you're new to curl
- **[C# curl Commands Complete Guide](csharp-curl-commands-complete-guide.md)** - Master curl in C#
- **[UserLand.NET Clean Room Development](userland-dotnet-clean-room-development.md)** - Our development philosophy
- **[Iron Software's Microsoft Commitment](iron-software-microsoft-dotnet-commitment.md)** - Enterprise ecosystem

### Technical Documentation
- [API Reference](api/) - Complete API documentation
- [Getting Started](getting-started/) - Installation and first steps
- [Tutorials](tutorials/) - Step-by-step guides
- [Cookbook](cookbook/) - Common recipes and patterns
- [Guides](guides/) - Advanced topics
- [Reference](reference/) - Technical reference
- [Exception Documentation](exceptions/) - Detailed error handling guides

## Installation

```bash
dotnet add package CurlDotNet
```

Or via Package Manager:

```powershell
Install-Package CurlDotNet
```

## Why CurlDotNet?

- **Pure C#** - No P/Invoke, no native dependencies
- **curl Compatible** - Use curl command syntax directly
- **Cross Platform** - Works on Windows, Linux, macOS
- **Well Documented** - Comprehensive documentation with examples
- **Feature Complete** - Supports all major curl options

## Links

- [GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet/)
- [Report Issues](https://github.com/jacob-mellor/curl-dot-net/issues)

---

<div class="author-section" style="border-top: 2px solid #e1e4e8; padding-top: 30px; margin-top: 50px;">
  <h2>Author</h2>
  <div class="author-bio" style="display: flex; align-items: center; gap: 20px; background: #f6f8fa; padding: 20px; border-radius: 8px;">
    <div class="author-avatar" style="flex-shrink: 0;">
      <img src="https://github.com/jacob-mellor.png" alt="Jacob Mellor" style="border-radius: 50%; width: 100px; height: 100px;">
    </div>
    <div class="author-details">
      <h3 style="margin: 0 0 10px 0;">Jacob Mellor</h3>
      <p style="margin: 0 0 10px 0; color: #586069;">Senior Software Engineer at IronSoftware</p>
      <p style="margin: 0 0 15px 0; font-size: 14px; color: #586069;">
        Jacob is a Senior Software Engineer at IronSoftware, specializing in .NET development and open-source tooling.
        Creator of CurlDotNet, bringing the power of curl to the .NET ecosystem with a pure C# implementation.
      </p>
      <div class="author-links" style="display: flex; gap: 15px;">
        <a href="https://github.com/jacob-mellor" target="_blank" rel="noopener">
          <img src="https://img.shields.io/badge/GitHub-181717?logo=github&logoColor=white" alt="GitHub">
        </a>
        <a href="https://linkedin.com/in/jacob-mellor-iron-software" target="_blank" rel="noopener">
          <img src="https://img.shields.io/badge/LinkedIn-0A66C2?logo=linkedin&logoColor=white" alt="LinkedIn">
        </a>
        <a href="https://ironsoftware.com/about-us/authors/jacobmellor/" target="_blank" rel="noopener">
          <img src="https://img.shields.io/badge/IronSoftware-FF5733?logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==&logoColor=white" alt="IronSoftware">
        </a>
      </div>
    </div>
  </div>
  <div style="margin-top: 20px; padding: 15px; background: #fff3cd; border: 1px solid #ffeaa7; border-radius: 5px;">
    <strong>💎 Sponsored by IronSoftware</strong><br>
    This project is proudly sponsored by <a href="https://ironsoftware.com" target="_blank">IronSoftware</a>,
    creators of IronPDF, IronOCR, IronXL, and IronBarcode - powerful .NET libraries for developers.
  </div>
</div>

<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "SoftwareSourceCode",
  "name": "CurlDotNet",
  "description": "Pure .NET implementation of curl for C#",
  "author": {
    "@type": "Person",
    "name": "Jacob Mellor",
    "url": "https://github.com/jacob-mellor",
    "sameAs": [
      "https://linkedin.com/in/jacob-mellor-iron-software",
      "https://ironsoftware.com/about-us/authors/jacobmellor/"
    ]
  },
  "sponsor": {
    "@type": "Organization",
    "name": "IronSoftware",
    "url": "https://ironsoftware.com"
  },
  "codeRepository": "https://github.com/jacob-mellor/curl-dot-net",
  "programmingLanguage": "C#",
  "license": "https://opensource.org/licenses/MIT"
}
</script>
