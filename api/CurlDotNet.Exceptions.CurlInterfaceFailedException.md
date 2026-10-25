#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlInterfaceFailedException Class

The local network interface or source address given with `--interface` could not be used \(exit code 45,
CURLE\_INTERFACE\_FAILED\); [InterfaceName](CurlDotNet.Exceptions.CurlInterfaceFailedException.InterfaceName.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException\.InterfaceName') names it\.

```csharp
public class CurlInterfaceFailedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlInterfaceFailedException

### Remarks

Fixes: use an interface name or IP address that exists on this machine (`ipconfig`/`ip addr`) and is
            up. Example: `catch (CurlInterfaceFailedException ex) { Console.WriteLine($"No interface {ex.InterfaceName}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md

### See Also
- [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')

| Constructors | |
| :--- | :--- |
| [CurlInterfaceFailedException\(string, string\)](CurlDotNet.Exceptions.CurlInterfaceFailedException.CurlInterfaceFailedException(string,string).md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException\.CurlInterfaceFailedException\(string, string\)') | Creates the error \(exit code 45\) for an interface that cannot be used, with advice to pick an existing one\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlInterfaceFailedException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlInterfaceFailedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [InterfaceName](CurlDotNet.Exceptions.CurlInterfaceFailedException.InterfaceName.md 'CurlDotNet\.Exceptions\.CurlInterfaceFailedException\.InterfaceName') | The interface name or local address from `--interface` that could not be bound, such as `eth1` or `10.0.0.5`\. |
