#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.StandardOutputDataKey Field

Key in [System\.Exception\.Data](https://learn.microsoft.com/en-us/dotnet/api/system.exception.data 'System\.Exception\.Data') under which a failed command keeps the text curl would
already have printed to standard output\.

```csharp
public const string StandardOutputDataKey = "CurlDotNet.Stdout";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Remarks

curl writes as it goes: with `-i` the headers of a `-f` failure are on screen,
            `--fail-with-body` prints the error page, `-w` still expands for a failed transfer, and
            with several URLs (`url1 url2`, globs, `--next`) the output of every transfer that ran
            before the failure is there. CurlDotNet throws instead of returning a result, so that text
            travels on the exception under this key (a [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')). It is the complete standard
            output, `-w` text included; [WriteOutDataKey](CurlDotNet.Exceptions.CurlException.WriteOutDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.WriteOutDataKey') holds only the `-w` part.

Usage: `catch (CurlException ex) { Console.Write(ex.StandardOutput); }` reproduces what
            the curl tool would have shown before it exited with [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode').

### See Also
- [StandardOutput](CurlDotNet.Exceptions.CurlException.StandardOutput.md 'CurlDotNet\.Exceptions\.CurlException\.StandardOutput')
- [WriteOutDataKey](CurlDotNet.Exceptions.CurlException.WriteOutDataKey.md 'CurlDotNet\.Exceptions\.CurlException\.WriteOutDataKey')