#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlDnsException Class

Thrown when DNS resolution fails for a hostname\.

```csharp
public class CurlDnsException : CurlDotNet.Exceptions.CurlConnectionException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') &#129106; CurlDnsException

### Example

```csharp
try
{
    var result = await curl.ExecuteAsync("curl https://non-existent-domain.invalid");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS lookup failed for: {ex.Host}");

    // Suggest alternatives
    if (ex.Host.Contains("github"))
        Console.WriteLine("Did you mean: github.com?");

    // Or try with IP address directly
    var ipAddress = "140.82.114.3"; // github.com IP
    result = await curl.ExecuteAsync($"curl https://{ipAddress}");
}
```

### Remarks

This exception indicates the hostname could not be resolved to an IP address.

Curl error code: CURLE_COULDNT_RESOLVE_HOST (6)

AI-Usage: Catch this to handle DNS failures separately from other connection issues.

AI-Pattern: Check for typos in hostname or DNS server configuration.
### Constructors

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string)'></a>

## CurlDnsException\(string, string\) Constructor

Initializes a new instance of the [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') class\.

```csharp
public CurlDnsException(string hostname, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).hostname'></a>

`hostname` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The hostname that could not be resolved\.

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)'></a>

## CurlDnsException\(SerializationInfo, StreamingContext\) Constructor

Initializes a new instance with serialized data\.

```csharp
protected CurlDnsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).info'></a>

`info` [System\.Runtime\.Serialization\.SerializationInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.serializationinfo 'System\.Runtime\.Serialization\.SerializationInfo')

<a name='CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext).context'></a>

`context` [System\.Runtime\.Serialization\.StreamingContext](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.streamingcontext 'System\.Runtime\.Serialization\.StreamingContext')