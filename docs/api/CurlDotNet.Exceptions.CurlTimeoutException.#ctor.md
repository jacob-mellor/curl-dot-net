#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')

## CurlTimeoutException Constructors

| Overloads | |
| :--- | :--- |
| [CurlTimeoutException\(string, string, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(string, string, System\.Nullable\<System\.TimeSpan\>\)') | Creates a timeout error \(exit code 28\) with the limit that expired, for transfers that ran out of time\. |
| [CurlTimeoutException\(string, string, Nullable&lt;TimeSpan&gt;, int\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_,int) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(string, string, System\.Nullable\<System\.TimeSpan\>, int\)') | Creates a timeout error with a curl exit code other than 28, for timeouts curl reports separately such as [CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException') \(12, the FTP server never connected back\)\. |
| [CurlTimeoutException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception, including [Timeout](CurlDotNet.Exceptions.CurlTimeoutException.Timeout.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.Timeout'), from serialized data on \.NET Framework\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_)'></a>

## CurlTimeoutException\(string, string, Nullable\<TimeSpan\>\) Constructor

Creates a timeout error \(exit code 28\) with the limit that expired, for transfers that ran out of time\.

```csharp
public CurlTimeoutException(string message, string command=null, System.Nullable<System.TimeSpan> timeout=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What timed out, naming the host or URL; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_).timeout'></a>

`timeout` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The limit that expired, or null when none was configured\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlTimeoutException.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_).message 'CurlDotNet\.Exceptions\.CurlTimeoutException\.CurlTimeoutException\(string, string, System\.Nullable\<System\.TimeSpan\>\)\.message') has no fix, the suggestion names the limit and tells the reader to raise
            `-m/--max-time` or `--connect-timeout`; the link points at the timeout page.
            Example: `throw new CurlTimeoutException("Upload to backup.example.com timed out after 30 s.", command, TimeSpan.FromSeconds(30));`

### See Also
- [CurlOperationTimeoutException\(double, string, string\)](CurlDotNet.Exceptions.CurlOperationTimeoutException.#ctor.md#CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string,string) 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException\.CurlOperationTimeoutException\(double, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_,int)'></a>

## CurlTimeoutException\(string, string, Nullable\<TimeSpan\>, int\) Constructor

Creates a timeout error with a curl exit code other than 28, for timeouts curl reports separately such as
[CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException') \(12, the FTP server never connected back\)\.

```csharp
protected CurlTimeoutException(string message, string? command, System.Nullable<System.TimeSpan> timeout, int curlErrorCode);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What timed out and how to fix it, separated by newlines\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_,int).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_,int).timeout'></a>

`timeout` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The limit that expired, or null when none was configured\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(string,string,System.Nullable_System.TimeSpan_,int).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl exit code, for example 12 \(CURLE\_FTP\_ACCEPT\_TIMEOUT\)\.

### Remarks

Used by derived types to set [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode').

### See Also
- [CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlTimeoutException\(SerializationInfo, StreamingContext\) Constructor

Restores the exception, including [Timeout](CurlDotNet.Exceptions.CurlTimeoutException.Timeout.md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.Timeout'), from serialized data on \.NET Framework\.

```csharp
protected CurlTimeoutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialized object data written by [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')\.

<a name='CurlDotNet.Exceptions.CurlTimeoutException.CurlTimeoutException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The source of the serialized stream, supplied by the serializer\.

### Remarks

Called by the serializer; not used directly.

### See Also
- [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlTimeoutException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlTimeoutException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')