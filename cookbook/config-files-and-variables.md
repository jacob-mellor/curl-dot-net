---
title: "curl Config Files, --variable and .netrc in C# (curl -K)"
description: "Use curl -K config files, --variable with {{name}} expansion, and .netrc credentials (-n, --netrc-file, --netrc-optional) in C# with CurlDotNet, plus why ~/.curlrc is never read."
---

# curl config files, --variable and .netrc in C# (curl -K)

curl can read options from a config file (`-K`), build values from variables (`--variable` with `--expand-...`), and take credentials from a `.netrc` file. CurlDotNet supports all three in C#, with curl's syntax, so command lines and files that work with the curl tool work unchanged.

## Read options from a file with -K

A config file holds one option per line. Leading dashes are optional, and the value may follow a space, `=` or `:`:

```text
# api.curlrc - options shared by every call
--silent
max-time = 10
header = "Accept: application/json"
user-agent: "inventory-sync/2.1"
url = "https://httpbin.org/get"
```

```csharp
using CurlDotNet;

File.WriteAllText("api.curlrc",
    "--silent\nmax-time = 10\nheader = \"Accept: application/json\"\nurl = \"https://httpbin.org/get\"\n");

var result = await Curl.ExecuteAsync("curl -K api.curlrc");
Console.WriteLine(result.StatusCode);
```

The rules follow curl's `tool_parsecfg.c`:

- Lines starting with `#` are comments.
- A value with spaces goes in double quotes; inside quotes `\t`, `\n`, `\r`, `\v`, `\"` and `\\` escapes work.
- URLs are given with `url = ...`, because a bare word is read as an option name.
- Options from the file apply as if they had been typed where `-K` appears, so options after `-K` on the command line override the file.
- A config file may include another with `-K`, up to 10 levels deep. Deeper nesting is a `CurlException` with `CurlErrorCode` 2.
- A missing or unreadable file throws `CurlReadErrorException` (exit 26).

### ~/.curlrc is never read

The curl tool reads `~/.curlrc` (or `$CURL_HOME/.curlrc`, `$XDG_CONFIG_HOME/curlrc`) automatically. CurlDotNet deliberately does not: a library must not change what an application sends because of a file in some user's home directory. Pass the file explicitly with `-K` when you want it. (This is one of the deliberate differences listed in the [test suite report](../reference/curl-test-suite.html).)

## Variables with --variable

`--variable` defines a named value; any option written as `--expand-<option>` then expands `{{name}}` in its argument:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl --variable host=httpbin.org --variable user=Ada " +
    "--expand-url 'https://{{host}}/anything/{{user}}'");
Console.WriteLine(result.StatusCode);   // GET https://httpbin.org/anything/Ada
```

Ways to define a variable:

| Form | Value |
|------|-------|
| `--variable name=text` | the text |
| `--variable name@file` | the contents of the file (`CurlReadErrorException`, exit 26, if it cannot be read) |
| `--variable %NAME` | the environment variable `NAME`; exit 2 if it is not set |
| `--variable %NAME=default` | the environment variable, or `default` when it is not set |
| `--variable name[0-99]@file` | bytes 0 to 99 of the file (also works with `=`) |

Functions after a colon transform the value, left to right:

| Function | Effect |
|----------|--------|
| `{{name:trim}}` | removes leading and trailing white space |
| `{{name:json}}` | escapes the value for use inside a JSON string |
| `{{name:url}}` | percent-encodes the value for a URL |
| `{{name:b64}}` | base64-encodes the value |
| `{{name:64dec}}` | base64-decodes the value |

They chain, for example `{{name:trim:url}}`. An unknown variable expands to nothing, an unknown function is exit 2, and `\{{` keeps a literal `{{`.

A typical use is putting a secret from the environment into a JSON body safely:

```csharp
using CurlDotNet;

Environment.SetEnvironmentVariable("API_NOTE", "He said \"hi\"\n");

var result = await Curl.ExecuteAsync(
    "curl --variable %API_NOTE " +
    "--expand-data '{\"note\":\"{{API_NOTE:trim:json}}\"}' " +
    "-H 'Content-Type: application/json' https://httpbin.org/post");
Console.WriteLine(result.StatusCode);
```

Expansion only happens in `--expand-` options. Plain `$NAME` environment variables in a command string are expanded too (outside single quotes, as a shell would), but they are not escaped, so prefer `--variable` with `:json` or `:url` for values you do not control.

## Credentials from .netrc

With `-n` / `--netrc`, CurlDotNet looks up the host in a `.netrc` file and sends its login and password, like curl:

```text
machine httpbin.org
login ada
password s3cret

default
login anonymous
password guest
```

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl --netrc-file ./build.netrc https://httpbin.org/basic-auth/ada/s3cret");
Console.WriteLine(result.StatusCode);   // 200
```

| Option | Behaviour |
|--------|-----------|
| `-n`, `--netrc` | Use the file; its password wins even when the URL has one. |
| `--netrc-optional` | Use the file only to fill in what the URL does not give. |
| `--netrc-file FILE` | Read this file instead of the default (implies `-n`). |

Without `--netrc-file`, the file is `$NETRC` when that variable is set, otherwise `~/.netrc` (on Windows `~/_netrc` when there is no `.netrc`). The format is curl's: `machine NAME` starts an entry, `default` matches any host, `macdef` blocks are skipped, and values may be quoted. A login in the URL (`https://ada@host/`) picks the entry for that login. An explicit `-u` always takes precedence over `.netrc`.

The credentials are used for the scheme you ask for (Basic by default, or `--digest`, `--ntlm`, `--anyauth`). See [curl digest, NTLM and basic auth in C#](auth-schemes.html).

## Without a command string

Every option in a config file has a `CurlOptions` property, so when the values live in your own configuration it is usually simpler to build the request directly:

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var options = new CurlOptions
{
    Url = "https://httpbin.org/get",
    MaxTime = 10,
    UserAgent = "inventory-sync/2.1"
};
options.Headers["Accept"] = "application/json";

var result = await Curl.ExecuteAsync(options);
Console.WriteLine(result.StatusCode);
```

## Related

- [curl multiple URLs and URL globbing in C#](multiple-urls-and-globbing.html)
- [curl digest, NTLM and basic auth in C#](auth-schemes.html)
- [curl CLI compatibility reference](../reference/curl-cli-compatibility.html)
- [Cookbook index](index.html)
