#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlConnectionException Class

Thrown when a network connection cannot be established to the target host\.

```csharp
public class CurlConnectionException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlConnectionException

Derived  
&#8627; [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException')  
&#8627; [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException')

### Example

```csharp
try
{
    var result = await curl.ExecuteAsync("curl https://internal-server:8443/api");
}
catch (CurlDnsException ex)
{
    // DNS couldn't resolve the hostname
    Console.WriteLine($"DNS lookup failed for {ex.Host}");
    // Try fallback server
    result = await curl.ExecuteAsync("curl https://backup-server/api");
}
catch (CurlConnectionException ex)
{
    // Connection failed but DNS worked
    Console.WriteLine($"Cannot connect to {ex.Host}:{ex.Port}");
    Console.WriteLine("Check if the service is running and firewall rules");
}
```

### Remarks

This exception indicates network-level connection failures, not HTTP errors.

Common causes include: host unreachable, port closed, firewall blocking, network timeout.

AI-Usage: Catch this to implement retry logic or fallback servers.

AI-Pattern: Check Host and Port properties to log connection details.
### Constructors

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string)'></a>

## CurlConnectionException\(string, int, string, Nullable\<int\>, string\) Constructor

Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class with an error code\.

```csharp
protected CurlConnectionException(string message, int curlErrorCode, string host, System.Nullable<int> port=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the connection failure\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl error code\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host that could not be connected to\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).port'></a>

`port` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The port number that was attempted\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string)'></a>

## CurlConnectionException\(string, string, Nullable\<int\>, string\) Constructor

Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class\.

```csharp
public CurlConnectionException(string message, string host, System.Nullable<int> port=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the connection failure\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host that could not be connected to\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).port'></a>

`port` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The port number that was attempted\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlConnectionException\(SerializationInfo, StreamingContext\) Constructor

Initializes a new instance with serialized data\.

```csharp
protected CurlConnectionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

<a name='CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')
### Properties

<a name='CurlDotNet.Exceptions.CurlConnectionException.Host'></a>

## CurlConnectionException\.Host Property

Gets the host that could not be connected to\.

```csharp
public string Host { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The hostname or IP address that failed to connect\.

### Remarks

This may be a hostname (e.g., "api.example.com") or IP address (e.g., "192.168.1.1").

AI-Usage: Use this to implement host-specific retry or fallback logic.

<a name='CurlDotNet.Exceptions.CurlConnectionException.Port'></a>

## CurlConnectionException\.Port Property

Gets the port number that was attempted\.

```csharp
public System.Nullable<int> Port { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The TCP port number, or null if using the default port for the protocol\.

### Remarks

Default ports: HTTP=80, HTTPS=443, FTP=21, FTPS=990.

AI-Usage: Check if non-standard ports might be blocked by firewalls.
### Methods

<a name='CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlConnectionException\.GetObjectData\(SerializationInfo, StreamingContext\) Method

Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\.

```csharp
public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

The serialization information\.

<a name='CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')

The streaming context\.

Implements [GetObjectData\(SerializationInfo, StreamingContext\)](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable.getobjectdata#system-runtime-serialization-iserializable-getobjectdata(system-runtime-serialization-serializationinfo-system-runtime-serialization-streamingcontext) 'System\.Runtime\.Serialization\.ISerializable\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo,System\.Runtime\.Serialization\.StreamingContext\)')