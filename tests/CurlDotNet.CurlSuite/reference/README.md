# Linux reference: real curl as the test oracle

When CurlDotNet and one of curl's test files disagree, don't guess what curl does. Run the same test with real curl, built from the commit the tests in `../data` were taken from, using curl's own `runtests.pl` and test servers on Linux. Then compare bytes.

This folder holds two Docker setups:

| File | What it does |
|---|---|
| `Dockerfile.curl` | Builds curl from commit `178e5077` with `--enable-debug --enable-ntlm` (the features curl's test suite expects), plus curl's test servers. |
| `run-curl-tests.sh` | The image's entry point. It runs `runtests.pl` for each test number given and keeps every log in `/out/testN/`. |
| `run-dotnet-tests.sh` | Runs CurlDotNet's curl replay and unit tests on Linux .NET 10. It works on a clean copy of the tree, so the host's `bin/` and `obj/` never leak in. |

Output goes to `out/`, which git ignores.

## Build the curl image once

```bash
docker build -f tests/CurlDotNet.CurlSuite/reference/Dockerfile.curl \
  -t curldotnet/curl-reference:178e5077 tests/CurlDotNet.CurlSuite/reference
```

The build takes a few minutes.

## Ask real curl

```bash
mkdir -p tests/CurlDotNet.CurlSuite/reference/out/curl
docker run --rm -v "$PWD/tests/CurlDotNet.CurlSuite/reference/out/curl":/out \
  curldotnet/curl-reference:178e5077 1473 2051
```

`out/curl/summary.txt` says PASS or FAIL for each test. `out/curl/testN/log/` holds what curl's harness saw:

| Log file | Contents |
|---|---|
| `server.input` | The exact request bytes curl sent. |
| `curlN.out` | The output file, with `--include` headers. |
| `stdoutN`, `stderrN` | curl's standard output and standard error. |
| `traceN` | `--trace-ascii` with timestamps, including connection open and close events. |
| `http_server.log` | What the test server did, including when it kept or closed a connection. |

curl's source for that commit is inside the image, for reading the exact rule behind a behaviour:

```bash
docker run --rm --entrypoint bash -w /home/tester/curl curldotnet/curl-reference:178e5077 \
  -c 'grep -n "retry_maxtime" src/tool_operate.c'
```

## Run CurlDotNet's tests on Linux

```bash
mkdir -p tests/CurlDotNet.CurlSuite/reference/out/dotnet
docker run --rm -v "$PWD":/src:ro -v "$PWD/tests/CurlDotNet.CurlSuite/reference/out/dotnet":/out \
  mcr.microsoft.com/dotnet/sdk:10.0 bash /src/tests/CurlDotNet.CurlSuite/reference/run-dotnet-tests.sh all
```

The argument is `suite`, `unit` or `all`. The script prints one exit line per project. The suite report, test logs and TRX files land in `out/dotnet/`.

## What it settled

- All 13 tests that were on `known-failures.txt` in v9.8.2 pass in real curl on Linux. Every one was a CurlDotNet or harness gap, and all 13 now pass in CurlDotNet too.
- curl's test server keeps a connection open after every reply unless the reply contains `swsclose` or is empty, and it never sends `100 Continue`. The C# stub now does the same.
- For a redirect whose body only ends when the server closes, curl doesn't read the body. It drops that connection and follows the redirect on a new one.
- `--retry-max-time` retries only while the elapsed time is below the limit, measured with a monotonic clock.
