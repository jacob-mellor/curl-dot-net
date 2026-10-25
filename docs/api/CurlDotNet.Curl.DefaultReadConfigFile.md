#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultReadConfigFile Property

Opt\-in: when true, every command first reads curl's default config file \(`.curlrc`\) from the user's home
directory exactly like the curl tool does, unless the command starts with `-q`/`--disable`\. False by default\.

```csharp
public static bool DefaultReadConfigFile { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to read the default config file like the curl tool; false \(the default\) to never read it\.

### Remarks

Why it is off: the curl tool silently applies `~/.curlrc` to every run, so a file in the user's home
            directory could add headers, a proxy, `-k` or a POST body to every request your application makes. A library
            should send what its code says, so CurlDotNet ignores the file unless you set this to true (for example in a .NET
            tool that must behave exactly like `curl` for its users). A config file named in the command with
            `-K FILE`/`--config FILE` is always read, whatever this says.

Lookup order, from curl's `src/tool_findfile.c`; the first file that can be opened wins and only that one is read:
            `$CURL_HOME/.curlrc`; `$XDG_CONFIG_HOME/curlrc` (no dot); `$HOME/.curlrc`; on Windows also
            `%USERPROFILE%\.curlrc`, `%APPDATA%\.curlrc` and `%USERPROFILE%\Application Data\.curlrc`; then, only
            when `XDG_CONFIG_HOME` is not set, `$CURL_HOME/.config/curlrc` and `$HOME/.config/curlrc`; and last, on
            Linux and macOS, `.curlrc` in the account's home directory from the user database (which matters when
            `HOME` is unset). On Windows each `.curlrc` in the variable-based places is also tried as `_curlrc`.
            Unset or empty variables are skipped. The curl tool's final Windows fallback, the folder of
            `curl.exe`, has no equivalent in a library and is not searched.

The file's options are processed before the command's own, as if typed first, with the same syntax as
            `-K` files. When one is read and the command asks for `-v`, `--trace` or `--trace-ascii` (and not
            `-s`), the note `Note: Read config file from '<path>'` is written to standard error (or the
            `--stderr` file), as curl does. A file that does not exist is not an error.

Process-wide; read at the start of every parse; reads and writes are thread-safe. It affects
            [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'), [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)'), [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') and every other
            method that parses a command string.

Example: `Curl.DefaultReadConfigFile = true;` then `await Curl.ExecuteAsync("curl https://example.com")`
            applies `~/.curlrc`, while `await Curl.ExecuteAsync("curl -q https://example.com")` does not.

### See Also
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')