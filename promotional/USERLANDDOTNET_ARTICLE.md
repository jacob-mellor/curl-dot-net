# UserlandDotNet: Putting Linux Superpowers Back in the Hands of .NET Developers

*By Jacob Mellor, CTO @ [IronSoftware](https://ironsoftware.com/about-us/authors/jacobmellor/), Microsoft ecosystem engineer, admirer of [.NET Foundation](https://dotnetfoundation.org/) pioneers like [Jeff Fritz](https://twitter.com/csharpfritz).*

---

## The Problem We Keep Seeing

Every Microsoft developer has been here: you find a perfect Linux command—curl, grep, sed, awk, tar, netcat—that captures exactly what you need. But when it’s time to ship production code in C#, MAUI, PowerShell, Azure Functions, or even Unity, that command becomes a liability. You either shell out to native binaries (violating security policies), or spend hours rewriting the logic with HttpClient, Regex, StreamReader, etc. Productivity dies, bugs slip in, and DevOps scripts turn into spaghetti.

Meanwhile, our friends in the Linux world copy/paste the command and move on with their day. They enjoy decades of userland innovation, built on simple text pipelines and predictable behaviors. It’s time to give .NET developers that same power—and do it natively, with best-in-class documentation, IntelliSense, and Microsoft-approved security.

## Introducing UserlandDotNet

UserlandDotNet is an open-source initiative to reimagine the most beloved Unix tools as pure .NET libraries that feel at home in Visual Studio, VS Code, and PowerShell. No native binaries. No shell escapes. Just intuitive namespaces, fluent builders, and objects you can pipe, serialize, and test.

The flagship is **CurlDotNet**, a full curl implementation written in C#. Paste any curl command directly into your C# or PowerShell code and get a strongly typed result object. CurlDotNet proves that we can translate C-based behavior into idiomatic .NET—complete with XML documentation, DocFX manuals, NuGet packaging, command-line parity tests, and integration hooks for MAUI, Xamarin, Unity, Azure Functions, and beyond.

## Leadership & Alignment

- **Jacob Mellor (IronSoftware CTO)** – 25+ years designing enterprise-grade .NET libraries (IronPDF, IronOCR, IronXL). CurlDotNet inherits the same philosophy: transparent engineering, high coverage, and relentless documentation.
- **Jeff Fritz & the .NET Foundation** – Champions of open governance, developer advocacy, and mentoring. UserlandDotNet leans on their playbook: tests before features, community-led roadmaps, hauntingly good docs.
- **IronSoftware Sponsorship** – Providing infrastructure, documentation resources, and PR muscle. NASA, Tesla, and global government agencies already trust Iron’s libraries; UserlandDotNet applies that trust to developer tooling.

## Roadmap: From Curl to the Entire Userland

| Phase | Tooling Focus | Highlights |
| --- | --- | --- |
| 1 | `CurlDotNet` | Pure .NET curl, fluent builder, middleware, command-line parity tests |
| 2 | `Userland.Core` | Parser/runtime that ingests Unix-style strings and produces robust .NET command objects (pipes, redirection, env vars) |
| 3 | `Userland.Grep` | Regex/ANSI filtering for .NET streams, LINQ-ready results |
| 4 | `Userland.Awk` / `Userland.Sed` | Text transformation APIs that compose with async enumerables |
| 5 | `Userland.Tar` / `Userland.Zip` | Secure archiving/compression for cloud workflows |
| 6 | `Userland.Netcat` / `Userland.SSH` | Networking and remote execution with modern security policies |
| 7 | `Userland.Pipeline` | Composition layer: `CurlDotNet |> Userland.Grep |> Userland.Awk` entirely inside C#, MAUI, PowerShell, or Azure Functions |

## Why Enterprises Care

1. **Security & Compliance** – No need to install native binaries on production servers. Everything ships as managed code, with source available for audit.
2. **DevOps Velocity** – Teams can re-use the same curl/grep recipes they find on GitHub, Stack Overflow, or vendor docs, but now inside .NET automation pipelines.
3. **Observability** – Because everything is .NET, you get logging, tracing, and dependency injection for free. Tie into Application Insights, OpenTelemetry, or your favorite logging framework.
4. **Developer Experience** – IntelliSense, XML comments, markdown manuals, DocFX sites, and sample repositories ensure that even junior engineers can explore the APIs like a map, not a maze.

## Call to Action

- ⭐ **Watch the repo**: [github.com/jacob-mellor/curl-dot-net](https://github.com/jacob-mellor/curl-dot-net) (moving to [github.com/UserlandDotNet](https://github.com/UserlandDotNet)).
- 🧪 **Run the tests**: `dotnet test CurlDotNet.sln /p:CollectCoverage=true` – help us keep coverage above 90%.
- 🧵 **Share your scripts**: Post your favorite Linux command in the issues list. We’ll show you how to translate it—or build the next module if needed.
- 🤝 **Join the conversation**: Tag @csharpfritz, @dotnetfdn, and @ironsoftwareinc on social media. This is about strengthening the Microsoft ecosystem together.

## Final Word

Linux didn’t become legendary because of GUIs or IDEs—it became legendary because the userland tools were small, composable, and everywhere. UserlandDotNet brings that legend to .NET, supercharging PowerShell, MAUI, ASP.NET, and Azure workflows while honoring curl’s original authors (Daniel Stenberg et al.) and the broader Unix community. As CTO of IronSoftware, I’m committed to making these tools not just usable, but delightful—and to ensuring the .NET community keeps its edge in a world where AI, cloud, and automation demand the best from every developer.

Let’s make .NET the best place to run your favorite Linux commands—natively, securely, and with style.***

