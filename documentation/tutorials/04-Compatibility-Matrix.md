# Compatibility Matrix

CurlDotNet targets the platforms Microsoft developers rely on today and tomorrow.  
Use this matrix to confirm support for your runtime, UI stack, or deployment model.

| Target | Version | Status | Notes |
| --- | --- | --- | --- |
| .NET Framework | 4.7.2+ | ✅ Supported | Built from the `net472` target. Ideal for classic ASP.NET or WinForms/WPF. |
| .NET Standard | 2.0 | ✅ Supported | Enables Xamarin, MAUI, Unity, Blazor WebAssembly, and older .NET Core apps. |
| .NET | 6.0 / 7.0 | ✅ Supported | Compatible via the `netstandard2.0` build. |
| .NET | 8.0 | ✅ Native Target | Primary LTS target with best coverage and performance. |
| .NET | 10 (preview) | 🔄 Planned | Architecture already prepared; upgrade once .NET 10 ships. |
| Xamarin | iOS/Android | ✅ Supported | Use the `netstandard2.0` assembly; ensure HttpClient handlers are configured for SSL. |
| MAUI | Desktop/Mobile | ✅ Supported | Works cross-platform; pair with native handlers for best performance. |
| Unity | 2021+ | ✅ Supported | Use IL2CPP-compatible build; avoid reflection-heavy middleware. |
| Blazor WebAssembly | .NET 8 | ✅ Supported | Run curl commands in the browser using fetch-based transport. |
| Azure Functions | v4 (.NET 6/8) | ✅ Supported | Use dependency injection to register CurlDotNet services. |
| PowerShell | 7.4+ | ✅ Supported | Reference the assembly and call CurlDotNet APIs directly from scripts. |

## NuGet Target Frameworks

- `netstandard2.0`
- `net472`
- `net8.0`

The NuGet package automatically selects the correct asset based on your project’s target framework.

## Testing Strategy

- `.NET Framework` tests run on Windows agents.
- `.NET 8` and `.NET Standard` tests run cross-platform.
- Command-line parity tests execute `/usr/bin/curl` (macOS/Linux) or `curl.exe` (Windows) to guarantee behavior matches the original binary.

