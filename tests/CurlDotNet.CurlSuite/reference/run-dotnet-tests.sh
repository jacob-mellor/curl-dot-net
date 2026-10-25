#!/usr/bin/env bash
# Runs inside mcr.microsoft.com/dotnet/sdk (Linux). The repository is mounted read-only at /src;
# it is copied without bin/obj (the host's build output must not leak in) and built for net10.0.
# Usage: run-dotnet-tests.sh [suite|unit|all] [extra dotnet test args...]
set -u
what="${1:-all}"; shift || true
mkdir -p /work /out
tar -C /src --exclude='./.git' --exclude='./.claude' --exclude='bin' --exclude='obj' \
    --exclude='./tests/CurlDotNet.CurlSuite/reference/out' -cf - . | tar -C /work -xf -
cd /work
dotnet --info | head -20 > /out/dotnet-info.txt
rc=0
run() { # name project
    local name=$1 project=$2; shift 2
    dotnet build "$project" -c Release --framework net10.0 -v q -nologo > "/out/$name-build.txt" 2>&1 || { echo "$name: BUILD FAILED"; tail -20 "/out/$name-build.txt"; rc=1; return; }
    CURL_SUITE_REPORT="/out/curl-suite-report.md" dotnet test "$project" -c Release --framework net10.0 --no-build \
        --blame-hang --blame-hang-timeout 300s --blame-hang-dump-type none \
        --logger "trx;LogFileName=/out/$name.trx" --logger "console;verbosity=normal" "$@" > "/out/$name-test.txt" 2>&1
    local t=$?
    echo "$name: exit $t  $(grep -E '^(Passed|Failed)!' "/out/$name-test.txt" | tail -1)"
    [ $t -ne 0 ] && rc=1
}
case "$what" in
    suite) run suite tests/CurlDotNet.CurlSuite/CurlDotNet.CurlSuite.csproj "$@" ;;
    unit)  run unit tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj "$@" ;;
    all)   run suite tests/CurlDotNet.CurlSuite/CurlDotNet.CurlSuite.csproj "$@"; run unit tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj "$@" ;;
esac
exit $rc
