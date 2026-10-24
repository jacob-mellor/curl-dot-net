#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[TraceWriter](CurlDotNet.Core.TraceWriter.md 'CurlDotNet\.Core\.TraceWriter')

## TraceWriter\.RecvData Method

| Overloads | |
| :--- | :--- |
| [RecvData\(byte\[\]\)](CurlDotNet.Core.TraceWriter.RecvData.md#CurlDotNet.Core.TraceWriter.RecvData(byte[]) 'CurlDotNet\.Core\.TraceWriter\.RecvData\(byte\[\]\)') | Dumps the received response body bytes\. |
| [RecvData\(string\)](CurlDotNet.Core.TraceWriter.RecvData.md#CurlDotNet.Core.TraceWriter.RecvData(string) 'CurlDotNet\.Core\.TraceWriter\.RecvData\(string\)') | Dumps the received response body text\. |

<a name='CurlDotNet.Core.TraceWriter.RecvData(byte[])'></a>

## TraceWriter\.RecvData\(byte\[\]\) Method

Dumps the received response body bytes\.

```csharp
public void RecvData(byte[] data);
```
#### Parameters

<a name='CurlDotNet.Core.TraceWriter.RecvData(byte[]).data'></a>

`data` [System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

The response body, or `null`/empty if none\.

<a name='CurlDotNet.Core.TraceWriter.RecvData(string)'></a>

## TraceWriter\.RecvData\(string\) Method

Dumps the received response body text\.

```csharp
public void RecvData(string text);
```
#### Parameters

<a name='CurlDotNet.Core.TraceWriter.RecvData(string).text'></a>

`text` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body text, or `null`/empty if none\.