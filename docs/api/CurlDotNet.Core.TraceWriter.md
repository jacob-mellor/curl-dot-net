#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## TraceWriter Class

Writes a detailed, human\-readable diagnostic trace of an HTTP transfer to a
file \(or standard output\), mirroring curl's `--trace`, `--trace-ascii`
and `--trace-time` options\.

```csharp
internal sealed class TraceWriter : System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; TraceWriter

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

### Remarks

The trace records every observable phase of a request: connection and
            proxy selection, TLS certificate verification, the exact request line, headers
            and body that were sent, the response status, headers and body that were
            received, transfer timings, and - most importantly for troubleshooting - the
            precise error if a request is blocked or fails.

This is invaluable when working behind a corporate or transparent proxy,
            where it is often unclear at which stage a request is being rejected.

The output is buffered in memory and written to the target when the
            writer is disposed, so a partially completed (failed) request still produces a
            complete trace up to the point of failure.

| Fields | |
| :--- | :--- |
| [MaxDumpBytes](CurlDotNet.Core.TraceWriter.MaxDumpBytes.md 'CurlDotNet\.Core\.TraceWriter\.MaxDumpBytes') | Maximum number of payload bytes dumped for a single send/receive block\. Larger payloads are truncated with a note so trace files stay manageable\. |

| Methods | |
| :--- | :--- |
| [Error\(string\)](CurlDotNet.Core.TraceWriter.Error(string).md 'CurlDotNet\.Core\.TraceWriter\.Error\(string\)') | Records an error \- typically the precise reason a request was blocked or failed\. |
| [Info\(string\)](CurlDotNet.Core.TraceWriter.Info(string).md 'CurlDotNet\.Core\.TraceWriter\.Info\(string\)') | Records an informational milestone \(connection, proxy, TLS, timing, etc\.\)\. |
| [RecvData\(byte\[\]\)](CurlDotNet.Core.TraceWriter.RecvData.md#CurlDotNet.Core.TraceWriter.RecvData(byte[]) 'CurlDotNet\.Core\.TraceWriter\.RecvData\(byte\[\]\)') | Dumps the received response body bytes\. |
| [RecvData\(string\)](CurlDotNet.Core.TraceWriter.RecvData.md#CurlDotNet.Core.TraceWriter.RecvData(string) 'CurlDotNet\.Core\.TraceWriter\.RecvData\(string\)') | Dumps the received response body text\. |
| [RecvHeaders\(HttpResponseMessage\)](CurlDotNet.Core.TraceWriter.RecvHeaders(System.Net.Http.HttpResponseMessage).md 'CurlDotNet\.Core\.TraceWriter\.RecvHeaders\(System\.Net\.Http\.HttpResponseMessage\)') | Dumps the response status line and headers received from the server\. |
| [SendDataAsync\(HttpRequestMessage\)](CurlDotNet.Core.TraceWriter.SendDataAsync(System.Net.Http.HttpRequestMessage).md 'CurlDotNet\.Core\.TraceWriter\.SendDataAsync\(System\.Net\.Http\.HttpRequestMessage\)') | Dumps the outgoing request body, if any\. Best\-effort: bodies that cannot be re\-read for inspection are skipped without affecting the request itself\. |
| [SendHeaders\(HttpRequestMessage\)](CurlDotNet.Core.TraceWriter.SendHeaders(System.Net.Http.HttpRequestMessage).md 'CurlDotNet\.Core\.TraceWriter\.SendHeaders\(System\.Net\.Http\.HttpRequestMessage\)') | Dumps the outgoing request line and headers as they are sent to the server\. |
| [TryCreate\(CurlOptions\)](CurlDotNet.Core.TraceWriter.TryCreate(CurlDotNet.Core.CurlOptions).md 'CurlDotNet\.Core\.TraceWriter\.TryCreate\(CurlDotNet\.Core\.CurlOptions\)') | Creates a [TraceWriter](CurlDotNet.Core.TraceWriter.md 'CurlDotNet\.Core\.TraceWriter') for the supplied options, or returns `null` when neither `--trace` nor `--trace-ascii` was requested\. |
