#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException Constructors

| Overloads | |
| :--- | :--- |
| [CurlException\(string, int, string\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string\)') | Creates an exception that carries curl's exit code, so callers can switch on [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') exactly like a shell script switches on `$?`\. |
| [CurlException\(string, int, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string, System\.Exception\)') | Creates an exception that carries curl's exit code and wraps the \.NET exception that caused it \(for example an [System\.IO\.IOException](https://learn.microsoft.com/en-us/dotnet/api/system.io.ioexception 'System\.IO\.IOException') while writing `-o` output\)\. |
| [CurlException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, string, System\.Exception\)') | Creates an exception with a message and, optionally, the command that failed and the exception that caused it\. |
| [CurlException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores an exception \(its message, [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')\) from serialized data; used by remoting and binary serialization on \.NET Framework\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string)'></a>

## CurlException\(string, int, string\) Constructor

Creates an exception that carries curl's exit code, so callers can switch on
[CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') exactly like a shell script switches on `$?`\.

```csharp
public CurlException(string message, int curlErrorCode, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened, how to fix it and \(optionally\) a `See URL` line, separated by
            newlines\. When no link is given, the page for [curlErrorCode](CurlDotNet.Exceptions.CurlException.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string).curlErrorCode 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string\)\.curlErrorCode') is appended\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl exit code, 1 to 100 \(see `man curl`, EXIT CODES\)\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

Exit codes are the `CURLE_*` values from `curl.h`: 6 = could not resolve host,
            7 = could not connect, 22 = HTTP error with `-f`, 23 = write error, 28 = timeout, 60 = certificate
            not trusted. The full table is at
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

Example: `throw new CurlException("Upload of 'big.iso' was refused by the server.\nSplit the file or raise the server limit.", 25, command);`
            produces a message ending with the documentation link for exit code 25.

### See Also
- [CurlException\(string, int, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string, System\.Exception\)')
- [GetCurlErrorName\(\)](CurlDotNet.Exceptions.CurlException.GetCurlErrorName().md 'CurlDotNet\.Exceptions\.CurlException\.GetCurlErrorName\(\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception)'></a>

## CurlException\(string, int, string, Exception\) Constructor

Creates an exception that carries curl's exit code and wraps the \.NET exception that caused it
\(for example an [System\.IO\.IOException](https://learn.microsoft.com/en-us/dotnet/api/system.io.ioexception 'System\.IO\.IOException') while writing `-o` output\)\.

```csharp
public CurlException(string message, int curlErrorCode, string command, System.Exception innerException);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened \(including the inner cause in words\), how to fix it, and
            optionally a `See URL` line, separated by newlines\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl exit code, 1 to 100\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The exception that caused this one, or null\.

### Remarks

The wrapped exception stays available as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException'); the message
            should still say in words what the inner exception means, because many loggers only print the
            outer message.

Example: `catch (IOException io) { throw new CurlException($"Could not write 'out.json': {io.Message}\nCheck that the folder exists and is writable.", 23, command, io); }`

### See Also
- [CurlException\(string, int, string\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string\)')
- [CurlException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, string, System\.Exception\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception)'></a>

## CurlException\(string, string, Exception\) Constructor

Creates an exception with a message and, optionally, the command that failed and the exception
that caused it\.

```csharp
public CurlException(string message, string command=null, System.Exception innerException=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened, how to fix it, and a `See URL` line, separated by newlines\.
            A plain one\-line message is also accepted and returned unchanged by [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command \(or a description of the API call\) that failed, for
            [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command'); null when there was none\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The \.NET exception that caused this one, kept as
            [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException'); null when there is none\.

### Remarks

Write [message](CurlDotNet.Exceptions.CurlException.md#CurlDotNet.Exceptions.CurlException.CurlException(string,string,System.Exception).message 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, string, System\.Exception\)\.message') as three parts: what happened on the first line (with the
            concrete URL, file or option), how to fix it on the following line(s), and
            `See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/...` on the last line. The fix
            lines become [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') and the link becomes [DocumentationUrl](CurlDotNet.Exceptions.CurlException.DocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlException\.DocumentationUrl').

Example: `throw new CurlException("Config file 'app.curlrc' is empty.\nAdd at least one URL line.\nSee https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md", command);`

This overload leaves [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') null. Use
            [CurlException\(string, int, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string, System\.Exception\)') when you know the curl exit code.

### See Also
- [CurlException\(string, int, string, Exception\)](CurlDotNet.Exceptions.CurlException.#ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(string,int,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlException\.CurlException\(string, int, string, System\.Exception\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlException\(SerializationInfo, StreamingContext\) Constructor

Restores an exception \(its message, [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')\) from serialized
data; used by remoting and binary serialization on \.NET Framework\.

```csharp
protected CurlException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialized object data written by [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')\.

<a name='CurlDotNet.Exceptions.CurlException.CurlException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The source of the serialized stream, supplied by the serializer\.

### Remarks

You do not call this yourself. Derived types that add state must call it and restore their own fields.
            Binary serialization is obsolete on .NET 8 and later; prefer [ToJson\(\)](CurlDotNet.Exceptions.CurlException.ToJson().md 'CurlDotNet\.Exceptions\.CurlException\.ToJson\(\)') to move errors between processes.

### See Also
- [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)')