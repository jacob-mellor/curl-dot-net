# 06 - Curl String Playbook

The fastest way to adopt CurlDotNet is still “copy a curl command, paste it into C#.” This playbook distills best practices from the curl docs, man pages, and field experience so you can move from terminal snippets to production-quality .NET code with confidence.

---

## 1. Anatomy of a Curl Command in C#

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST \
         -H 'Accept: application/json' \
         -H 'Authorization: Bearer {TOKEN}' \
         -d '{""name"":""Ada"",""email"":""ada@example.com""}' \
         https://api.example.com/users
");
```

- `curl` prefix is optional; keep it for readability when copying docs verbatim.
- Use verbatim strings (`@"..."`) so backslashes, quotes, and line breaks match the source command.
- Every curl flag maps to the same behavior in CurlDotNet, because `CommandParser` mirrors libcurl’s CLI semantics (`src/CurlDotNet/Core/CommandParser.cs`).

---

## 2. Quick Translation Checklist

1. **Preserve quoting** – single quotes behave like bash single quotes; double quotes allow interpolation if you need it. Escape embedded quotes the same way you would in C#.
2. **Line continuations** – keep `\` at the end of each line or collapse to one line. CurlDotNet normalizes whitespace.
3. **Environment variables** – expand them in C# before passing them in, e.g. `$"curl -H 'Authorization: Bearer {token}' …"`.
4. **Files** – `-d @payload.json` still works; ensure files exist relative to the working directory of your process, not the shell you copied from.
5. **Binary data** – prefer `--data-binary` or `--upload-file` to keep newline-sensitive payloads intact.

---

## 3. Command Patterns & Recipes

### 3.1 CRUD over REST

```csharp
var list = await Curl.ExecuteAsync("curl https://api/v1/users");

var create = await Curl.ExecuteAsync(@"
    curl -X POST \
         -H 'Content-Type: application/json' \
         -d '{""name"":""Grace""}' \
         https://api/v1/users
");

var update = await Curl.ExecuteAsync(@"
    curl -X PATCH \
         -H 'Content-Type: application/json' \
         -d '{""plan"":""pro""}' \
         https://api/v1/users/42
");

var delete = await Curl.ExecuteAsync("curl -X DELETE https://api/v1/users/42");
```

### 3.2 Multipart Uploads

```csharp
var response = await Curl.ExecuteAsync(@"
    curl -X POST \
         -F 'profile=@/var/data/avatar.png' \
         -F 'metadata={""visibility"":""private""};type=application/json' \
         https://storage.local/upload
");
```

### 3.3 OAuth Token Exchanges

```csharp
var token = await Curl.ExecuteAsync(@"
    curl -u client_id:client_secret \
         -d 'grant_type=client_credentials' \
         https://login.example.com/oauth2/token
");
```

---

## 4. Validation & Debugging

- **`-v` / `--verbose`** replicates curl’s diagnostic output inside CurlDotNet. Combine with `CurlResult.DebugLog` (see XML docs) or middleware logging.
- **`-w` / `--write-out`** can be replaced with `CurlResult` properties (`StatusCode`, `TotalTime`, `Headers`, etc.).
- **`-D` / `-o`** map to in-memory headers/body by default; add `-o file.json` or use `result.SaveToFile`.
- **`--max-time`, `--connect-timeout`** behave exactly like libcurl; pair with retry middleware for resilience.

---

## 5. Security Considerations

- Keep secrets out of source control. Build strings dynamically or use `SecureString`/vault lookups before constructing the command.
- Avoid `--insecure` except during onboarding; prefer `CurlOptions.Insecure = false` globally and override only when diagnosing certificate chains.
- When copying commands from tutorials, replace sample tokens (`sk_test`, `Bearer token`) before execution to avoid leaking credentials in logs.

---

## 6. Moving from String API to LibCurl / Builder

| Need | Suggested Transition |
| --- | --- |
| Reuse headers/timeouts across many commands | Convert to `LibCurl` fluent defaults (see Chapter 05). |
| Programmatic construction with IntelliSense | Switch to `CurlRequestBuilder` so you avoid manual quoting. |
| Middleware, retries, metrics | Use `CurlMiddlewarePipeline` or dependency-injected LibCurl. |

Start with string commands for rapid prototyping, then graduate to the LibCurl or builder APIs once patterns stabilize.

---

## 7. Reference

- Curl flags follow the official curl documentation: [https://curl.se/docs/manpage.html](https://curl.se/docs/manpage.html).
- CurlDotNet command parsing tests live under `tests/CurlDotNet.Tests/CommandParserTests.cs`—skim them to see how edge cases (mixed quotes, `--data @-`, cookies, proxies) are covered.
- For advanced options (`--proto`, `--limit-rate`, etc.) check `docs/ADVANCED.md` and ensure the command parser supports the switches you need. If not, add tests + support there.

---

**Next:** Review `manual/05-LibCurl-Deep-Dive.md` for reusable client patterns or jump to the upcoming Fluent Builder recipe guide to keep everything strongly typed.


