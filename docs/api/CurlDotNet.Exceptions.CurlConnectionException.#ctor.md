#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')

## CurlConnectionException Constructors

| Overloads | |
| :--- | :--- |
| [CurlConnectionException\(string, int, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, int, string, System\.Nullable\<int\>, string\)') | Creates a connection failure that carries curl's exit code, for derived types such as [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') \(6\) and [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException') \(5\)\. |
| [CurlConnectionException\(string, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)') | Creates a connection failure for [host](CurlDotNet.Exceptions.CurlConnectionException.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).host 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)\.host') \(and [port](CurlDotNet.Exceptions.CurlConnectionException.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).port 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)\.port')\) without a curl exit code, for code of your own that reports an unreachable endpoint\. |
| [CurlConnectionException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception, including [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host') and [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port'), from serialized data\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string)'></a>

## CurlConnectionException\(string, int, string, Nullable\<int\>, string\) Constructor

Creates a connection failure that carries curl's exit code, for derived types such as
[CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') \(6\) and [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException') \(5\)\.

```csharp
protected CurlConnectionException(string message, int curlErrorCode, string host, System.Nullable<int> port=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed and how to fix it, separated by newlines\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl exit code, for example 6 \(could not resolve host\)\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host name or IP address that could not be reached\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).port'></a>

`port` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The TCP port that was tried, or null for the default port\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

Used by derived types to set [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode').

### See Also
- [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string)'></a>

## CurlConnectionException\(string, string, Nullable\<int\>, string\) Constructor

Creates a connection failure for [host](CurlDotNet.Exceptions.CurlConnectionException.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).host 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)\.host') \(and [port](CurlDotNet.Exceptions.CurlConnectionException.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).port 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)\.port')\) without a curl exit
code, for code of your own that reports an unreachable endpoint\.

```csharp
public CurlConnectionException(string message, string host, System.Nullable<int> port=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed, naming the host; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host name or IP address that could not be reached\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).port'></a>

`port` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The TCP port that was tried, or null for the scheme's default port\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlConnectionException.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).message 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)\.message') has no fix, the suggestion names the host and port and the link points at
            the DNS and connection page. Example: `throw new CurlConnectionException($"Health check could not reach {host}.", host, 8443, command);`

### See Also
- [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlConnectionException\(SerializationInfo, StreamingContext\) Constructor

Restores the exception, including [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host') and [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port'), from serialized data\.

```csharp
protected CurlConnectionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialized object data written by [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlConnectionException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The source of the serialized stream, supplied by the serializer\.

### Remarks

Called by the serializer; not used directly.

### See Also
- [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlConnectionException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')