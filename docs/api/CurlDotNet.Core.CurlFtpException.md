#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlFtpException Class

Thrown when an FTP server answers an `ftp://` or `ftps://` transfer with an error reply
\(4xx/5xx\), such as 530 "Not logged in" or 550 "File unavailable"; [FtpStatusCode](CurlDotNet.Core.CurlFtpException.FtpStatusCode.md 'CurlDotNet\.Core\.CurlFtpException\.FtpStatusCode') holds the reply code\.

```csharp
public class CurlFtpException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFtpException

### Remarks

The message names the reply code, the server's text and the URL (without credentials), and
            says how to fix that class of reply: 530/331/332 mean the login was refused (pass or correct
            `-u user:password`; without `-u` CurlDotNet logs in as `anonymous`), 550/553 mean the
            path is missing or not permitted (FTP paths are relative to the login folder; write `%2F` for an
            absolute path), 421/425/426 mean the data connection failed (CurlDotNet uses passive mode; check that firewalls allow the
            passive data ports), 552 means the server's storage limit was hit.

[CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is 9 (`CURLE_REMOTE_ACCESS_DENIED`) for every reply code.
            A server that cannot be reached at all is a [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') (exit 7) instead.

FTP uses `FtpWebRequest`, which .NET Framework and .NET on Windows, Linux and macOS include;
            where it is missing (browser WebAssembly, some trimmed runtimes) the transfer fails with exit code 1
            before any reply.

Example: `try { await Curl.ExecuteAsync("curl -u alice:secret ftp://files.example.com/reports/q3.csv -o q3.csv"); }
            catch (CurlFtpException ex) when (ex.FtpStatusCode == 550) { Console.WriteLine("No such report yet"); }`

Reply codes and fixes: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')
- [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')
- [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')

| Constructors | |
| :--- | :--- |
| [CurlFtpException\(string, int\)](CurlDotNet.Core.CurlFtpException.#ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int) 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int\)') | Creates the exception for an FTP error reply; the message should say what failed and how to fix it\. |
| [CurlFtpException\(string, int, string\)](CurlDotNet.Core.CurlFtpException.#ctor.md#CurlDotNet.Core.CurlFtpException.CurlFtpException(string,int,string) 'CurlDotNet\.Core\.CurlFtpException\.CurlFtpException\(string, int, string\)') | Creates the exception for an FTP error reply and records the curl command that was running, so [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command') shows what to retry\. |

| Properties | |
| :--- | :--- |
| [FtpStatusCode](CurlDotNet.Core.CurlFtpException.FtpStatusCode.md 'CurlDotNet\.Core\.CurlFtpException\.FtpStatusCode') | The FTP reply code the server sent, for example 530 \(not logged in\), 550 \(file unavailable\) or 552 \(storage allocation exceeded\)\. |
