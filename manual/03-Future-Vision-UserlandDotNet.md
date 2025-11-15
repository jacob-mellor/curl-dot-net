# Future Vision: UserlandDotNet

UserlandDotNet is the long-term mission to bring the best of the Unix/Linux userland into .NET and PowerShell—with first-class tooling, documentation, and IntelliSense. CurlDotNet is the first major building block.

## Why UserlandDotNet?

- **Parity with Bash** – Developers constantly find curl, grep, awk, sed, and jq snippets online. Translating those into .NET code wastes time and introduces bugs.
- **Enterprise Deployment** – Regulated environments often disallow native binaries or shell execution. Pure .NET implementations solve compliance and deployment challenges.
- **PowerShell Synergy** – PowerShell already embraces object pipelines. UserlandDotNet turns curl-style commands into strongly-typed .NET objects that can be piped, filtered, and composed.

## Roadmap & Priorities

| Phase | Tooling Focus | Description | Status |
| --- | --- | --- | --- |
| 1 | `CurlDotNet` | Pure .NET curl, fluent builders, middleware, doc coverage, parity tests | ✅ In progress |
| 2 | `Userland.Core` | Shared parser/runtime that ingests Unix-style strings → strongly typed .NET command objects (pipes, env vars, redirection) | 🛠 Design |
| 3 | `Userland.Grep` | Regex/ANSI filtering for .NET streams & PowerShell pipelines | 🔜 Next |
| 4 | `Userland.Awk` / `Userland.Sed` | Text transforms & record-based scripting with LINQ-friendly APIs | 🔜 Next |
| 5 | `Userland.Tar` / `Userland.Zip` | Archiving/compression routines with span-friendly APIs | 🔜 Next |
| 6 | `Userland.Netcat` / `Userland.SSH` | Network/socket tooling, channel bridging, remote exec with modern security | 🔜 Future |
| 7 | `Userland.Pipeline` | Composition layer: `CurlDotNet |> Userland.Grep |> Userland.Awk` inside PowerShell, C#, MAUI, etc. | 🔜 Future |

Priorities emphasize reusable parsing (Userland.Core), then high-demand CLI behaviors (grep/sed/awk) that unblock DevOps automation inside .NET and PowerShell. CurlDotNet remains the flagship proving ground for documentation, testing, and Microsoft-aligned developer experience.

## Principles

- **No Feature Bloat** – Advanced features live in dedicated namespaces (e.g., `CurlDotNet.Middleware`) so junior developers are never overwhelmed.
- **Transparency** – Each module credits the original Unix authors where applicable and explains the translation strategy from C to C#.
- **Docs Everywhere** – IntelliSense XML comments, DocFX, Markdown manuals, and DevTo-ready tutorials ensure the tooling is self-explanatory.

## How to Participate

- Watch the `UserlandDotNet` organization on GitHub.
- Follow leaders in the Microsoft ecosystem (Jeff Fritz, .NET Foundation) for best practices in OSS governance.
- Share the repo (`https://github.com/jacob-mellor/curl-dot-net`, moving to `https://github.com/UserlandDotNet/curl-dot-net`).
- File issues describing the Linux commands you want most inside .NET.
- Contribute tests first, then features—mirroring the TDD workflow documented in `CONTINUING_WORK.md`.

