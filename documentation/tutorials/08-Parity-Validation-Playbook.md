# 08 – Parity Validation Playbook

> Prove that CurlDotNet behaves exactly like the native `curl` CLI across commands, platforms, and edge cases.

Parity matters for trust. This playbook documents how to compare CurlDotNet output with the canonical curl binary, interpret differences, and update both code and docs based on findings. Use it when onboarding new contributors, running release validation, or investigating bug reports that cite “works with curl, fails in CurlDotNet.”

---

## 1. Test Matrix

| Layer | Tooling | Purpose |
| --- | --- | --- |
| Unit tests | `CurlUnit*` categories | Validate parser/engine behavior in isolation |
| Synthetic tests | `Synthetic` category | Stress flags, headers, body combinations |
| Integration tests | `Integration`, `Httpbin` suites | Hit real endpoints (httpbin.org) for end-to-end coverage |
| Command-line comparison | `CommandLineComparison` category | Execute curl CLI + CurlDotNet, diff results |

Run everything before tagging a release:

```bash
dotnet test tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj --verbosity normal
dotnet test --filter "Category=CommandLineComparison"
```

> Tip: Use `dotnet test --filter "Category=CommandLineComparison&Category=RequiresNetwork"` on macOS/Linux with curl installed.

---

## 2. Command-Line Comparison Workflow

1. **Choose fixtures**
   - Start with deterministic hosts: `https://httpbin.org`, `https://ifconfig.me`, or local mock servers.
2. **Add or update tests**
   ```csharp
   [Fact]
   [Trait("Category", TestCategories.CommandLineComparison)]
   public async Task SimpleGet_CompareWithCurl()
   {
       await AssertCurlParity("curl https://httpbin.org/get");
   }
   ```
3. **Run comparison suite**
   ```bash
   dotnet test --filter "Category=CommandLineComparison"
   ```
4. **Analyze diffs**
   - Located under `tests/CurlDotNet.Tests/TestResults/<guid>/CommandLineComparison/`.
   - Files typically include `curl.stdout`, `CurlDotNet.stdout`, and JSON metadata.
5. **Log findings**
   - Update `SESSION_TRANSACTION_LOG.md` or open an issue referencing the failing fixture.

---

## 3. Investigating Mismatches

### Checklist
- ✅ Same request body? (Check `-d`, `--data-urlencode`, multipart boundaries)
- ✅ Same headers? (Look for auto headers like `User-Agent`, `Accept`, `Content-Length`)
- ✅ Same TLS/proxy settings? (`-k`, `--proxy`, `--resolve`)
- ✅ Storage differences? (`-o` paths vs `result.OutputFile`)
- ✅ Environment differences? (`HTTP_PROXY`, `NO_PROXY`, locale)

### Tools
- `result.DebugLog` (CurlDotNet) vs CLI `-v`
- Wireshark / Fiddler for raw HTTP dumps
- `CurlSettings.CaptureOptions = true` to inspect parsed `CurlOptions`
- `curl --trace-ascii` to capture CLI payload details for comparison

---

## 4. Recording Evidence

When parity fails:

1. Capture both commands’ full output.
2. Note OS, .NET SDK version, curl binary version (`curl --version`).
3. Add a snippet to `SESSION_TRANSACTION_LOG.md` referencing the test name and reproduction steps.
4. If the issue is doc-related (e.g., quoting gotcha), update the relevant manual page (`06` for strings, `07` for builders, `05` for LibCurl).

Template:

```markdown
### 2025-11-15 – Failure: Multipart boundary mismatch
- Test: `CommandParserSyntheticTests.Multipart_Upload_Parity`
- Env: macOS 14.4, .NET 8.0.100, curl 8.7.1
- Observed: CLI adds `Expect: 100-continue`, CurlDotNet does not.
- Action: Added header flag, documented in `06-Curl-String-Deep-Dive.md` (section 4.2).
```

---

## 5. Authoritative References

- `tests/CurlDotNet.Tests/CommandLineComparisonTests.cs` – Reference implementation of the parity harness.
- `CURL_SOURCE_MAPPING.md` – Map between curl option enums and managed implementation points.
- `docs/ADVANCED.md` – Hooks for plugging in middleware/logging to capture extra diagnostics during parity runs.

---

## 6. Release Checklist

Before publishing a NuGet package:

1. ✅ Run full test suite (unit, integration, comparison).
2. ✅ Capture versions (`dotnet --info`, `curl --version`).
3. ✅ Update `manual/08-Parity-Validation-Playbook.md` if new steps were added.
4. ✅ Mention parity status in release notes (“Validated against curl 8.7.1 on macOS/Linux”).
5. ✅ Cross-link relevant manual sections (06, 07) or blog posts highlighting the parity guarantee.

---

**Next:** Feed parity findings into the [Curl String Deep Dive](06-Curl-String-Deep-Dive.md) and [Fluent Builder Cookbook](07-Fluent-Builder-Cookbook.md) so end-users understand how to avoid common pitfalls. For automation ideas, see `docs/ADVANCED.md` middleware hooks.*** End Patch

