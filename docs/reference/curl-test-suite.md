---
title: "How CurlDotNet Is Tested Against curl's Own Test Suite"
description: "How CurlDotNet replays 1092 of curl's own test files against a stub server to prove curl compatibility in C#: what the harness checks, how to run it, how to read the report, and the known-failures list."
---

# How CurlDotNet is tested against curl's own test suite

CurlDotNet identifies itself as curl 8.7.1 (`User-Agent: curl/8.7.1`) and claims to behave like curl, and that claim is tested with curl's own test suite, taken from the curl repository in October 2026. The project `tests/CurlDotNet.CurlSuite` replays 1092 test files from the curl repository against CurlDotNet: each test's command line is run through `Curl.ExecuteAsync`, and the request that reaches the wire, the exit code and the output are compared with what curl's test says curl must produce.

## Current results

| Result | Count |
|--------|-------|
| Test files replayed | 1092 |
| Runnable (not skipped) | 631 |
| Pass | 631 (100%) |
| of which byte-exact, header order included | 286 |
| Fail | 0 (`known-failures.txt` is empty) |
| Skipped (need servers or features CurlDotNet does not have) | 461 |

Source: curl/curl commit `178e5077b6`, October 2026. The figures are from a run on Linux with .NET 10 (v9.8.11). They come from `tests/CurlDotNet.CurlSuite/curl-suite-report.md`, which every run regenerates.

## What a curl test file is

curl's tests live in `tests/data/testNNNN` in the curl repository. Each file is a small XML-like document with:

- `<reply>`: the exact bytes the test server sends back (status line, headers, body), sometimes several parts for redirects or authentication rounds.
- `<client>`: the curl command line, input files the test needs, environment variables and required features.
- `<verify>`: what must happen: the request bytes curl must send (`<protocol>`), the exit code (`<errorcode>`), standard output (`<stdout>`), uploaded data, and files that must exist with given contents.

curl's own `runtests.pl` runs these against its `sws` test server. CurlDotNet's harness does the same job in C#.

## What the harness does

For every test file whose only server is `http`:

1. **Skip check.** Tests that need something CurlDotNet does not provide are skipped with a reason: another server (FTP, SMTP, SOCKS, DNS), a C test program (`libtest`, `unittest`), stdin, custom server commands, or a curl feature outside CurlDotNet's list (for example `Debug`, `ipfs`, `PSL`, `alt-svc`, `IDN`).
2. **Serve the reply.** `StubHttpServer.cs` is a from-scratch HTTP/1.x server built on `TcpListener`, so nothing sits between the test and the wire. Like curl's `sws` it records every request byte for byte, picks the reply part from the number at the end of the request path, honours `swsclose` and `swsbounce`, serves `<connectN>` replies to `CONNECT`, and moves to the next reply part for Digest, NTLM, Basic and Negotiate challenges. Like `sws`, it keeps every connection open unless the reply contains `swsclose` or is empty, and it never sends `100 Continue`.
3. **Run the command.** The test's command line, with curl's `%HOSTIP`, `%HTTPPORT`, `%LOGDIR` and similar variables substituted, is passed to `Curl.ExecuteAsync` from a per-test working directory, with `-q`, `--output`, `--include` and `--trace-ascii` added exactly as `runtests.pl` adds them, and the test's environment variables set. Each test has 25 seconds.
4. **Compare.** The exit code is `CurlErrorCode` of the thrown exception (0 for a result). The recorded request is compared with `<protocol>`, uploaded bytes with `<upload>`, standard output (`result.Body`, or `CurlException.StandardOutput` for a failure) with `<stdout>`, standard error with `<stderr>` (the harness writes curl's `curl: (N) message` line from `CurlException.CurlErrorMessage`, and curl's warnings from `Warnings`), and output files with `<data>` / `<datacheck>` and any `<file>` sections. The Perl rewrite rules in `<stripfile>` and `<strippart>` are replayed first, and `crlf="headers"` converts exactly the lines `runtests.pl`'s `subnewlines()` converts.

Requests are compared leniently first: request and status lines must match in order, and the other lines must match as a set, because `HttpClient` controls part of the header order. A test that also matches byte for byte, header order included, is counted as **byte-exact**.

No network access is needed; everything runs against `127.0.0.1`.

## How to run it

From the repository root:

```bash
dotnet test tests/CurlDotNet.CurlSuite/CurlDotNet.CurlSuite.csproj -c Release --framework net10.0
```

A full run takes a little over a minute. To run one curl test and see what it did:

```bash
dotnet test tests/CurlDotNet.CurlSuite/CurlDotNet.CurlSuite.csproj -c Release --framework net10.0 \
  --filter 'DisplayName~number: 1\)' --logger "console;verbosity=detailed"
```

The detailed log prints the test name, the outcome (`Pass`, `Pass (byte-exact)`, `Fail` or `Skip`) with the reason, and the command that ran. Set the environment variable `CURL_SUITE_REPORT` to a path to write the report somewhere other than the project directory.

The CI smoke workflow runs the same suite on pull requests and on pushes to `master` (on Ubuntu; the unit tests run on Linux, Windows and macOS) and uploads the report as a build artifact.

## How to read the report

`curl-suite-report.md` has three parts:

- **The summary table**, as shown above, and the pass rate over runnable tests.
- **Failures**: one row per failing test with its curl test number, name and the first differences found, for example `exit code 7, expected 0` or `protocol: missing -Proxy-Authorization: NTLM ...` (a `-` line was expected but not sent, a `+` line was sent but not expected).
- **Skipped**: the skip reasons, with how many tests each one covers and their numbers.

To look at a failing test, open `tests/CurlDotNet.CurlSuite/data/testNNNN`, then run it alone with the filter above.

## The known-failures discipline

`tests/CurlDotNet.CurlSuite/known-failures.txt` lists every test that is expected to fail, one number per line with the reason. The rules keep the list honest:

- A failing test that is **not** on the list fails the build. A change that breaks curl compatibility cannot slip in.
- A listed test that **starts passing** also fails the build, with the message "now passes: remove it from known-failures.txt". The list cannot quietly go stale.

The list is empty today. Before a test goes on it, run that test in real curl (see below) and give a reason taken from curl's logs or source.

## Checking a test against real curl

When CurlDotNet and a test disagree, the answer comes from real curl, not from memory. `tests/CurlDotNet.CurlSuite/reference` has a Docker setup that builds curl from the same commit the tests were taken from and runs curl's own `runtests.pl` and test servers on Linux:

```bash
docker build -f tests/CurlDotNet.CurlSuite/reference/Dockerfile.curl \
  -t curldotnet/curl-reference:178e5077 tests/CurlDotNet.CurlSuite/reference
docker run --rm -v "$PWD/out":/out curldotnet/curl-reference:178e5077 1473 2051
```

Each test's log folder holds the exact request bytes curl sent, its output, standard error, a timed trace and the test server's own log. The same folder runs CurlDotNet's replay and unit tests on Linux .NET 10. Its [README](https://github.com/jacob-mellor/curl-dot-net/blob/master/tests/CurlDotNet.CurlSuite/reference/README.md) has the commands.

This is how the last 49 failures were settled. All 13 tests on the list in v9.8.2 pass in real curl on Linux, so each was a CurlDotNet or harness gap. The stricter harness then exposed 36 more, and all 49 now pass.

## The unit tests

The suite above checks behaviour against curl. The unit tests in `tests/CurlDotNet.Tests` (2073 test cases) check CurlDotNet's own API: parsing, the request builder, results, exceptions, and parity tests (`Parity*.cs`) for each curl behaviour that was ported. Every one runs offline against in-process servers, so CI and `scripts/test-all-locally.csx` run the whole project with no category filter:

```bash
dotnet test tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj -c Release --framework net10.0
```

## What the suite does not cover

- Only tests served over plain HTTP are replayed. HTTPS, FTP and `file://` are covered by the unit tests only.
- The suite runs on .NET 10. On netstandard2.0 and .NET Framework, CurlDotNet relies on `HttpClient`'s parsed headers and lacks `--request-target`, removing the `Host` header, blank headers, lower-case chunk sizes, `--raw`, `-p` tunnels and a separate `--connect-timeout`; see [framework compatibility](../framework-compatibility.html).

## Related

- [curl CLI compatibility reference](curl-cli-compatibility.html)
- [curl exit codes in C#](curl-exit-codes.html)
- [How CurlDotNet works](../how-to/how-curldotnet-works/README.md)
- [Reference index](index.html)
