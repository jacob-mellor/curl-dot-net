#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

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

| Constructors | |
| :--- | :--- |
| [CurlDnsException\(string, string\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(string,string) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(string, string\)') | Initializes a new instance of the [CurlDnsException](CurlDotNet.Exceptions.CurlDnsException.md 'CurlDotNet\.Exceptions\.CurlDnsException') class\. |
| [CurlDnsException\(SerializationInfo, StreamingContext\)](CurlDotNet.Exceptions.CurlDnsException.#ctor.md#CurlDotNet.Exceptions.CurlDnsException.CurlDnsException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext) 'CurlDotNet\.Exceptions\.CurlDnsException\.CurlDnsException\(System\.Runtime\.Serialization\.SerializationInfo, System\.Runtime\.Serialization\.StreamingContext\)') | Initializes a new instance with serialized data\. |
