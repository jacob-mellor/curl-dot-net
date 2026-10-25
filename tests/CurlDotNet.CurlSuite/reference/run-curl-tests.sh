#!/usr/bin/env bash
# Runs curl's own runtests.pl for each test number given and keeps everything curl's harness
# saw: runtests output plus the whole log directory (what curl sent, what the server sent,
# stdout, stderr, trace). Results land in /out/testN/.
set -u
cd /home/tester/curl/tests
../src/curl --version > /out/curl-version.txt
summary=/out/summary.txt
: > "$summary"
for n in "$@"; do
    dir="/out/test$n"
    rm -rf "$dir" log
    mkdir -p "$dir"
    # -a: keep going, -n: no valgrind, -am: automake-style PASS/FAIL/SKIP line; log/ is kept by default
    perl ./runtests.pl -a -n -am "$n" > "$dir/runtests.txt" 2>&1
    cp -r log "$dir/log" 2>/dev/null
    line=$(grep -m1 -E "^(PASS|FAIL|SKIP|XFAIL|ERROR)[^:]*: *$n " "$dir/runtests.txt")
    echo "${line:-$n UNKNOWN: $(tail -1 "$dir/runtests.txt")}" >> "$summary"
done
cat "$summary"
