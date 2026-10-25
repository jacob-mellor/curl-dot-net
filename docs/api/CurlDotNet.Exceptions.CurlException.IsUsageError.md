#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.IsUsageError Property

True when the curl tool reports this failure as a mistake in the command line: it prints every line of
[CurlErrorMessage](CurlDotNet.Exceptions.CurlException.CurlErrorMessage.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorMessage') after `curl: `, then `curl: try 'curl --help' for more information`, and no
`curl: (2) ...` line\.

```csharp
public bool IsUsageError { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for option errors curl finds while it reads the command line, such as `-C` together with
            `--no-clobber` or an option given as the last word without its value; [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is 2 then\.
            False for failures curl reports as `curl: (N) message`, and also for option errors inside a `-K` file,
            where curl adds a `FILE:LINE config file option ...` line that CurlDotNet does not reproduce \(their
            [CurlErrorMessage](CurlDotNet.Exceptions.CurlException.CurlErrorMessage.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorMessage') is curl's first error line only\)\.

### Remarks

For `curl --no-clobber -C 1 -o out URL` the text is two lines, `--continue-at is mutually exclusive with
            --no-clobber` and `option -C: is badly used here`. The first line is an error message, which curl leaves out
            under `-s` unless `-S` is given; the last line (the option and what is wrong with it) and the
            `try 'curl --help'` hint are always printed.

A .NET tool that imitates the curl command line prints it like this: `foreach (var line in
            ex.CurlErrorMessage.Split('\n')) Console.Error.WriteLine("curl: " + line);` followed by
            `Console.Error.WriteLine("curl: try 'curl --help' for more information");`.

### See Also
- [CurlErrorMessage](CurlDotNet.Exceptions.CurlException.CurlErrorMessage.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorMessage')
- [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')