---
title: "Paste a curl Command into PowerShell or C#"
description: "Paste a curl command from Stripe, GitHub or OpenAI-style API docs and run it unchanged in C# or PowerShell with CurlDotNet. Placeholders, keys, gotchas."
---

# How to paste a curl command from API docs into PowerShell or C#

To run a curl command from API documentation unchanged, paste it into a C# raw string literal for `Curl.ExecuteAsync`, or into a PowerShell single-quoted here-string for `[CurlDotNet.Curl]::Execute`. Replace the placeholders, put secrets in environment variables, and it runs as documented.

## GitHub-style: headers and a bearer token

GitHub's REST docs show commands like this. A public repository works without a token:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -L \
      -H "Accept: application/vnd.github+json" \
      -H "X-GitHub-Api-Version: 2022-11-28" \
      https://api.github.com/repos/jacob-mellor/curl-dot-net
    """);
Console.WriteLine(result.StatusCode);  // 200
```

GitHub requires a User-Agent. Like the curl CLI, CurlDotNet sends `User-Agent: curl/8.7.1` automatically, so the command works exactly as GitHub documents it; add `-A "my-app/1.0"` if you want to identify your app. For private data, add `-H "Authorization: Bearer $GITHUB_TOKEN"` (double quotes, so the variable is expanded).

The same request with the fluent builder, once you want to make it typed:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://api.github.com/repos/jacob-mellor/curl-dot-net")
    .FollowRedirects().WithUserAgent("my-app/1.0")  // optional
    .WithHeader("Accept", "application/vnd.github+json")
    .WithHeader("X-GitHub-Api-Version", "2022-11-28")
    .ExecuteAsync();
```

## Stripe-style: API key as user name, form fields

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl https://api.stripe.com/v1/customers \
      -u "$STRIPE_SECRET_KEY:" \
      -d name="Jenny Rosen" \
      -d email="jennyrosen@example.com"
    """);
Console.WriteLine(result.StatusCode);
```

`-u key:` sends the key as a Basic-auth user name with an empty password. Repeated `-d` values are joined with `&` and sent as `application/x-www-form-urlencoded`, as with the curl CLI. `name="Jenny Rosen"` arrives as `name=Jenny Rosen`.

## OpenAI-style: JSON body and a key from the environment

```powershell
$env:OPENAI_API_KEY = '<your key>'
$r = [CurlDotNet.Curl]::Execute(@'
curl https://api.openai.com/v1/chat/completions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $OPENAI_API_KEY" \
  -d '{ "model": "<model-id>", "messages": [{"role": "user", "content": "Hello"}] }'
'@)
$r.StatusCode
($r.Body | ConvertFrom-Json).choices[0].message.content
```

The bash-style `\` continuations and `$OPENAI_API_KEY` work unchanged: CurlDotNet joins the lines and reads the variable from the environment.

## Checklist before you run a pasted command

1. **Replace placeholders** such as `<YOUR-TOKEN>`, `OWNER/REPO` or `sk_test_...`.
2. **Secrets.** `$NAME`, `${NAME}`, `%NAME%` and `$env:NAME` are expanded from environment variables where a shell would expand them: unquoted or in double quotes, not inside single quotes. An unset variable is sent as typed, which usually shows up as a 401.
3. **User-Agent.** CurlDotNet sends `User-Agent: curl/8.7.1` like the CLI; add `-A` only to identify your own app.
4. **Add `-m`** for slow or unreliable endpoints. As with the curl CLI, there is no timeout without it; `-m` covers the whole transfer.
5. **Check for unsupported flags.** Flags such as `--retry`, `-c`, `--http2`, `-G`, `-T` and `-D` work as in curl. Options that are recognised but not implemented do not stop the request; they are listed in `Curl.Validate(cmd).ParsedOptions.UnsupportedOptions`, so check it before relying on an unusual flag. Client-certificate and CA options (`--cert`, `--key`, `--cacert`) are parsed and stored but not applied either. A misspelled flag lands in `UnknownOptions` and makes `Validate` report the command as invalid.
6. **Check the result.** A 4xx or 5xx does not throw, so look at `StatusCode` / `IsSuccess`. With `-f` / `--fail`, it throws `CurlHttpReturnedErrorException` (curl code 22) instead.

## Validate first

```csharp
using CurlDotNet;

var check = Curl.Validate("curl -H 'Accept: application/json' https://api.github.com/zen");
Console.WriteLine(check.IsValid ? $"{check.ParsedOptions.Url} OK" : check.Error);
```

## Related

- [Run curl commands from PowerShell](../run-curl-commands-powershell/README.md)
- [curl vs Invoke-WebRequest](../curl-vs-invoke-webrequest/README.md)
- [Bearer tokens in C#](../../how-to/curl-bearer-token-csharp/README.md)
- [Basic auth in C#](../../how-to/curl-basic-auth-csharp/README.md)
- [PowerShell guides](../README.md)
