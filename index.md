---
layout: default
title: CurlDotNet - Pure .NET curl for C#
---

# CurlDotNet Documentation

![CurlDotNet - Why .NET Needs a POSIX/GNU Userland](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/1o4hlr4tbp6b8k86ew6c.jpg)

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

## 💜 Thank You .NET Foundation!

<div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; border-radius: 12px; margin: 30px 0; text-align: center; box-shadow: 0 10px 30px rgba(0,0,0,0.1);">
  <a href="https://dotnetfoundation.org" target="_blank" rel="noopener" style="text-decoration: none;">
    <img src="https://dotnetfoundation.org/img/logo_big.svg" alt=".NET Foundation" style="max-width: 300px; margin-bottom: 20px; filter: brightness(0) invert(1);">
  </a>
  <h3 style="color: white; margin: 20px 0; font-size: 24px;">We ❤️ the .NET Foundation!</h3>
  <p style="color: rgba(255,255,255,0.9); font-size: 16px; line-height: 1.6; max-width: 600px; margin: 0 auto 20px;">
    Thank you to the <a href="https://dotnetfoundation.org" style="color: #ffd700; font-weight: bold;" target="_blank">.NET Foundation</a> for their incredible work in developing and maintaining C# and the .NET ecosystem!
    From the early days of C# 1.0 (2000) evolving from Anders Hejlsberg's experience with Delphi, C++, and Java, to today's C# 13 and .NET 9, the journey has been remarkable.
    Parallel to this, we've witnessed the evolution of typed JavaScript/ECMAScript - from Macromedia's ActionScript 2.0 (2003) which pioneered optional static typing for ECMAScript, through Adobe's ActionScript 3.0 (2006) with full static typing, to Microsoft's TypeScript (2012) - also created by Anders Hejlsberg - bringing modern type safety to JavaScript.
    While these are separate language families with different roots, seeing both ecosystems mature from the late 90s/early 2000s to where they are today - with C# becoming truly cross-platform and TypeScript becoming the de facto standard for large-scale JavaScript development - represents an incredible achievement in programming language evolution.
  </p>
  <p style="color: rgba(255,255,255,0.9); font-size: 16px;">
    <strong>Thank you for making .NET open source, cross-platform, and amazing! 🚀</strong>
  </p>
  <div style="margin-top: 20px;">
    <a href="https://dotnetfoundation.org/projects" target="_blank" style="background: white; color: #667eea; padding: 12px 30px; border-radius: 25px; text-decoration: none; font-weight: bold; display: inline-block; transition: transform 0.2s;">
      Explore .NET Foundation Projects →
    </a>
  </div>
</div>

## Documentation

- [API Reference](api/) - Complete API documentation
- [Getting Started](getting-started/) - Installation and first steps
- [Tutorials](tutorials/) - Step-by-step guides
- [Cookbook](cookbook/) - Common recipes
- [Guides](guides/) - Advanced topics
- [Reference](reference/) - Technical reference
- [Exception Documentation](exceptions/) - Detailed error handling guides
- [Promotional Materials](promotional-materials) - Press kit and marketing assets

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
