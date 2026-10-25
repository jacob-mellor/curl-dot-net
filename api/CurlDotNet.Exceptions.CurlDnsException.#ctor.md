#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')

## CurlDnsException Constructors

| Overloads | |
| :--- | :--- |
| [CurlDnsException\(string, string\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(string, string\)') | Creates the exception for [hostname](CurlDotNet.Exceptions.CurlDnsException.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).hostname 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(string, string\)\.hostname'), with curl's message `Could not resolve host: NAME` and exit code 6\. |
| [CurlDnsException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Restores the exception from serialized data on \.NET Framework \(host name and exit code included\)\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string)'></a>

## CurlDnsException\(string, string\) Constructor

Creates the exception for [hostname](CurlDotNet.Exceptions.CurlDnsException.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).hostname 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(string, string\)\.hostname'), with curl's message `Could not resolve host: NAME`
and exit code 6\.

```csharp
public CurlDnsException(string hostname, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).hostname'></a>

`hostname` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host name from the URL that could not be resolved, such as `api.example.com`\.

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

The fix line tells the reader to check the spelling and DNS and shows the `--resolve` workaround for that
            name. Example: `throw new CurlDnsException("api.exmaple.com", command);`

### See Also
- [CurlCouldntResolveHostException\(string, string, string\)](CurlDotNet.Exceptions.CurlCouldntResolveHostException.#ctor.md#CurlDotNet.Exceptions.CurlCouldntResolveHostException.CurlCouldntResolveHostException(string,string,string) 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException\.CurlCouldntResolveHostException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlDnsException\(SerializationInfo, StreamingContext\) Constructor

Restores the exception from serialized data on \.NET Framework \(host name and exit code included\)\.

```csharp
protected CurlDnsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialized object data written by GetObjectData\.

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The source of the serialized stream, supplied by the serializer\.

### Remarks

Called by the serializer; not used directly.

### See Also
- [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException')