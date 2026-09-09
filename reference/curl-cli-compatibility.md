# curl CLI Compatibility (Windows, macOS, Linux)

CurlDotNet’s parser accepts raw `curl …` strings copied from documentation, terminals, or browsers. This reference explains how options differ across operating systems and clarifies why **Ubuntu-style syntax is our canonical source of truth**.

## Canonical Behavior: Ubuntu/Linux

The upstream curl project ships from Debian/Ubuntu, so its manual and examples assume:

- **Bash-style quoting** – single quotes prevent interpolation, double quotes expand variables.
- **Line continuations** – a trailing `\` escapes the newline.
- **Environment variables** – `$TOKEN` or `${TOKEN}` syntax.
- **Paths** – forward slashes, no drive letters.

CurlDotNet mirrors these rules internally. When we parse a command, we first normalize it as if it were typed on Ubuntu, then apply OS-specific conveniences.

### Example

```bash
curl -X POST https://api.example.com/users \
  -H 'Authorization: Bearer '"$API_TOKEN" \
  -d '{"name":"Ada","role":"admin"}'
```

Guidance:

- Prefer single quotes for JSON bodies.
- Use `$VAR` interpolation only when a CI runner or shell supplies the variable.
- Keep paths relative or use forward slashes; Windows paths are also supported but converted.

## Windows (CMD & PowerShell) Differences

| Concern | CMD | PowerShell | Notes |
|--------|-----|------------|-------|
| Quotes | `"` only | `'` and `"` | Wrap JSON in double quotes; CurlDotNet automatically unescapes. |
| Escape newline | `^` | `` ` `` | Parser replaces these with line breaks before processing. |
| Env vars | `%TOKEN%` | `$env:TOKEN` | We detect and expand both syntaxes. |
| Paths | `C:\path\file.txt` | same | Normalized to forward slashes internally. |

Handling tips:

```powershell
curl -H "Accept: application/json" `
     -d "{""foo"":""bar""}" `
     https://api.example.com
```

```cmd
curl -X POST ^
     -H "Content-Type: application/json" ^
     -d "{\"foo\":\"bar\"}" ^
     https://api.example.com
```

CurlDotNet strips `^`/`` ` `` continuations, unescapes nested quotes, and treats `%VAR%` exactly like `$VAR` from Ubuntu.

## macOS (zsh/bash)

Modern macOS defaults to `zsh`. Behavior matches Ubuntu except for:

- **Glob qualifiers** – `~` expands to the home directory just like Linux; CurlDotNet preserves the literal value, so expand paths before copying.
- **Unicode smart quotes** – avoid copying from rich-text editors because “smart quotes” will fail. Replace with `'` or `"`.

Example:

```zsh
curl --data-urlencode 'query=type:pull-request user:octocat' \
     https://api.github.com/search/issues
```

## Option Compatibility Checklist

| Option | Ubuntu/Linux | macOS | Windows | CurlDotNet Notes |
|--------|--------------|-------|---------|------------------|
| `-H` headers | ✅ | ✅ | ✅ | Preserve quoting exactly as Ubuntu. |
| `--data`, `-d` | ✅ | ✅ | ✅ | Windows needs escaped quotes; we normalize. |
| `--data-binary @file` | ✅ | ✅ | ✅ | Provide absolute paths on Windows; parser converts. |
| `--config file` | ✅ | ✅ | ⚠️ (paths) | Config files parsed using Ubuntu rules; prefer forward slashes. |
| `--user user:pass` | ✅ | ✅ | ✅ | Backslashes in passwords double-escape on Windows; use single quotes when possible. |
| `--form 'file=@path'` | ✅ | ✅ | ✅ | Use double quotes on Windows if single quotes unavailable. |

## Best Practices

1. **Author commands on Ubuntu (or WSL)**. This guarantees syntax that matches curl’s documentation and our parser’s baseline.
2. **Convert platform specifics only when necessary**. For example, if a CI script needs PowerShell, wrap the Ubuntu command in `@"` HERE-STRING `@"` to preserve JSON quoting.
3. **Avoid shell features that CurlDotNet cannot reproduce**:
   - Process substitution (`<(...)`)
   - Command substitution inside single quotes
   - Shell functions or aliases defined inline
4. **Test once per platform**. When supporting both Windows and Linux build agents, run a short smoke test to confirm quoting works as expected.

## Debugging Tips

- **Log the command** before execution to ensure the correct quoting survived templating.
- Use the `curl --trace-ascii -` option during development; CurlDotNet understands it and surfaces the trace in `result.Body`.
- If a command works on Ubuntu but fails after copy/paste, run it through our parser diagnostic method:
  ```csharp
  var validation = Curl.Validate(command);
  if (!validation.IsValid)
      Console.WriteLine(validation.Error);
  ```

Ubuntu remains the “source of truth,” but with these rules, you can safely translate to Windows or macOS while preserving compatibility.
