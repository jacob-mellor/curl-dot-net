#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

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

| Constructors | |
| :--- | :--- |
| [CurlConnectionException\(string, int, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,int,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, int, string, System\.Nullable\<int\>, string\)') | Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class with an error code\. |
| [CurlConnectionException\(string, string, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(string,string,System.Nullable_int_,string) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(string, string, System\.Nullable\<int\>, string\)') | Initializes a new instance of the [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') class\. |
| [CurlConnectionException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.#ctor.md#CurlDotNet.Exceptions.CurlConnectionException.CurlConnectionException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlConnectionException\.CurlConnectionException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |

| Properties | |
| :--- | :--- |
| [Host](CurlDotNet.Exceptions.CurlConnectionException.Host.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Host') | Gets the host that could not be connected to\. |
| [Port](CurlDotNet.Exceptions.CurlConnectionException.Port.md 'CurlDotNet\.Exceptions\.CurlConnectionException\.Port') | Gets the port number that was attempted\. |

| Methods | |
| :--- | :--- |
| [GetObjectData\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlConnectionException.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).md 'CurlDotNet\.Exceptions\.CurlConnectionException\.GetObjectData\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Sets the [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo') with information about the exception\. |
