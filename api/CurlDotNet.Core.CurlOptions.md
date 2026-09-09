#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlOptions Class

Represents parsed curl command options\.

```csharp
public class CurlOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlOptions

### Remarks

This class contains all options extracted from a curl command string.

AI-Usage: This is the intermediate representation between curl syntax and HTTP operations.

| Properties | |
| :--- | :--- |
| [BinaryData](CurlDotNet.Core.CurlOptions.BinaryData.md 'CurlDotNet\.Core\.CurlOptions\.BinaryData') | Binary data for upload\. |
| [CaCert](CurlDotNet.Core.CurlOptions.CaCert.md 'CurlDotNet\.Core\.CurlOptions\.CaCert') | Alias for CaCertFile for test compatibility\. |
| [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile') | CA certificate file \(\-\-cacert\)\. |
| [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile') | Certificate file for client authentication \(\-\-cert\)\. |
| [Compressed](CurlDotNet.Core.CurlOptions.Compressed.md 'CurlDotNet\.Core\.CurlOptions\.Compressed') | Whether to use compressed encoding \(\-\-compressed\)\. |
| [ConnectTimeout](CurlDotNet.Core.CurlOptions.ConnectTimeout.md 'CurlDotNet\.Core\.CurlOptions\.ConnectTimeout') | Connection timeout in seconds \(\-\-connect\-timeout\)\. |
| [Cookie](CurlDotNet.Core.CurlOptions.Cookie.md 'CurlDotNet\.Core\.CurlOptions\.Cookie') | Cookie string \(\-b or \-\-cookie\)\. |
| [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar') | Cookie jar file path \(\-c or \-\-cookie\-jar\)\. |
| [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs') | Whether to create missing directories for output \(\-\-create\-dirs\)\. |
| [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials') | Basic authentication \(\-u or \-\-user\)\. |
| [CustomMethod](CurlDotNet.Core.CurlOptions.CustomMethod.md 'CurlDotNet\.Core\.CurlOptions\.CustomMethod') | Request method override \(\-X flag value\)\. |
| [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data') | Request body data\. |
| [DataUrlEncode](CurlDotNet.Core.CurlOptions.DataUrlEncode.md 'CurlDotNet\.Core\.CurlOptions\.DataUrlEncode') | Whether to URL encode the data \(\-\-data\-urlencode flag\)\. |
| [DisableEprt](CurlDotNet.Core.CurlOptions.DisableEprt.md 'CurlDotNet\.Core\.CurlOptions\.DisableEprt') | Whether to disable EPRT for FTP \(\-\-disable\-eprt\)\. |
| [DisableEpsv](CurlDotNet.Core.CurlOptions.DisableEpsv.md 'CurlDotNet\.Core\.CurlOptions\.DisableEpsv') | Whether to disable EPSV for FTP \(\-\-disable\-epsv\)\. |
| [DnsServers](CurlDotNet.Core.CurlOptions.DnsServers.md 'CurlDotNet\.Core\.CurlOptions\.DnsServers') | DNS servers to use \(\-\-dns\-servers\)\. |
| [FailOnError](CurlDotNet.Core.CurlOptions.FailOnError.md 'CurlDotNet\.Core\.CurlOptions\.FailOnError') | Fail silently on HTTP errors \(\-f flag\)\. |
| [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files') | Files to upload \(for \-F flag with @file\)\. |
| [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation') | Whether to follow redirects \(\-L flag\)\. |
| [FollowRedirects](CurlDotNet.Core.CurlOptions.FollowRedirects.md 'CurlDotNet\.Core\.CurlOptions\.FollowRedirects') | Alias for FollowLocation for test compatibility\. |
| [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData') | Form data for multipart/form\-data\. |
| [FtpPassive](CurlDotNet.Core.CurlOptions.FtpPassive.md 'CurlDotNet\.Core\.CurlOptions\.FtpPassive') | Whether to use passive mode for FTP \(\-\-ftp\-pasv\)\. |
| [FtpSsl](CurlDotNet.Core.CurlOptions.FtpSsl.md 'CurlDotNet\.Core\.CurlOptions\.FtpSsl') | Whether to use SSL/TLS for FTP \(\-\-ftp\-ssl\)\. |
| [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers') | Request headers\. |
| [HeadOnly](CurlDotNet.Core.CurlOptions.HeadOnly.md 'CurlDotNet\.Core\.CurlOptions\.HeadOnly') | Whether to show only headers \(\-I or \-\-head flag\)\. |
| [HttpVersion](CurlDotNet.Core.CurlOptions.HttpVersion.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersion') | HTTP version to use \(\-\-http1\.0, \-\-http1\.1, \-\-http2\)\. |
| [IncludeHeaders](CurlDotNet.Core.CurlOptions.IncludeHeaders.md 'CurlDotNet\.Core\.CurlOptions\.IncludeHeaders') | Whether to include headers in output \(\-i flag\)\. |
| [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure') | Whether to ignore SSL errors \(\-k flag\)\. |
| [Interface](CurlDotNet.Core.CurlOptions.Interface.md 'CurlDotNet\.Core\.CurlOptions\.Interface') | Interface to use for outgoing connections \(\-\-interface\)\. |
| [KeepAliveTime](CurlDotNet.Core.CurlOptions.KeepAliveTime.md 'CurlDotNet\.Core\.CurlOptions\.KeepAliveTime') | Whether to use TCP keepalive \(\-\-keepalive\-time\)\. |
| [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile') | Certificate key file \(\-\-key\)\. |
| [LocationTrusted](CurlDotNet.Core.CurlOptions.LocationTrusted.md 'CurlDotNet\.Core\.CurlOptions\.LocationTrusted') | Location trusted for automatic authentication \(\-\-location\-trusted\)\. |
| [MaxRedirects](CurlDotNet.Core.CurlOptions.MaxRedirects.md 'CurlDotNet\.Core\.CurlOptions\.MaxRedirects') | Maximum number of redirects to follow \(\-\-max\-redirs\)\. |
| [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime') | Maximum time in seconds \(\-\-max\-time\)\. |
| [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method') | HTTP method \(GET, POST, PUT, DELETE, etc\.\)\. |
| [OriginalCommand](CurlDotNet.Core.CurlOptions.OriginalCommand.md 'CurlDotNet\.Core\.CurlOptions\.OriginalCommand') | The original command string\. |
| [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile') | Output file path \(\-o flag\)\. |
| [ProgressBar](CurlDotNet.Core.CurlOptions.ProgressBar.md 'CurlDotNet\.Core\.CurlOptions\.ProgressBar') | Whether to show progress bar \(\-\-progress\-bar\)\. |
| [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler') | Progress callback handler\. |
| [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy') | Proxy URL \(\-x or \-\-proxy\)\. |
| [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials') | Proxy authentication \(\-\-proxy\-user\)\. |
| [Quote](CurlDotNet.Core.CurlOptions.Quote.md 'CurlDotNet\.Core\.CurlOptions\.Quote') | FTP/SFTP commands to execute after transfer \(\-\-quote\)\. |
| [Range](CurlDotNet.Core.CurlOptions.Range.md 'CurlDotNet\.Core\.CurlOptions\.Range') | Range of bytes to request \(\-r or \-\-range\)\. |
| [Referer](CurlDotNet.Core.CurlOptions.Referer.md 'CurlDotNet\.Core\.CurlOptions\.Referer') | Referer header \(\-e or \-\-referer\)\. |
| [Resolve](CurlDotNet.Core.CurlOptions.Resolve.md 'CurlDotNet\.Core\.CurlOptions\.Resolve') | Resolve host to IP \(\-\-resolve\)\. |
| [ResumeFrom](CurlDotNet.Core.CurlOptions.ResumeFrom.md 'CurlDotNet\.Core\.CurlOptions\.ResumeFrom') | Resume from byte offset \(\-C or \-\-continue\-at\)\. |
| [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry') | Retry count \(\-\-retry\)\. |
| [RetryDelay](CurlDotNet.Core.CurlOptions.RetryDelay.md 'CurlDotNet\.Core\.CurlOptions\.RetryDelay') | Retry delay in seconds \(\-\-retry\-delay\)\. |
| [RetryMaxTime](CurlDotNet.Core.CurlOptions.RetryMaxTime.md 'CurlDotNet\.Core\.CurlOptions\.RetryMaxTime') | Maximum retry time \(\-\-retry\-max\-time\)\. |
| [ShowError](CurlDotNet.Core.CurlOptions.ShowError.md 'CurlDotNet\.Core\.CurlOptions\.ShowError') | Show error even in silent mode \(\-S flag\)\. |
| [Silent](CurlDotNet.Core.CurlOptions.Silent.md 'CurlDotNet\.Core\.CurlOptions\.Silent') | Silent mode \(\-s flag\)\. |
| [Socks5Proxy](CurlDotNet.Core.CurlOptions.Socks5Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Socks5Proxy') | SOCKS proxy \(\-\-socks5\)\. |
| [SpeedLimit](CurlDotNet.Core.CurlOptions.SpeedLimit.md 'CurlDotNet\.Core\.CurlOptions\.SpeedLimit') | Speed limit in bytes per second \(\-\-limit\-rate\)\. |
| [SpeedTime](CurlDotNet.Core.CurlOptions.SpeedTime.md 'CurlDotNet\.Core\.CurlOptions\.SpeedTime') | Speed time period for limit \(\-\-speed\-time\)\. |
| [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') | The target URL\. |
| [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent') | User agent string \(\-A or \-\-user\-agent\)\. |
| [UserAuth](CurlDotNet.Core.CurlOptions.UserAuth.md 'CurlDotNet\.Core\.CurlOptions\.UserAuth') | Alias for Credentials for test compatibility\. |
| [UseRemoteFileName](CurlDotNet.Core.CurlOptions.UseRemoteFileName.md 'CurlDotNet\.Core\.CurlOptions\.UseRemoteFileName') | Whether to use remote filename for output \(\-O flag\)\. |
| [Verbose](CurlDotNet.Core.CurlOptions.Verbose.md 'CurlDotNet\.Core\.CurlOptions\.Verbose') | Verbose output \(\-v flag\)\. |
| [WriteOut](CurlDotNet.Core.CurlOptions.WriteOut.md 'CurlDotNet\.Core\.CurlOptions\.WriteOut') | Write\-out format string \(\-w or \-\-write\-out\)\. |

| Methods | |
| :--- | :--- |
| [Clone\(\)](CurlDotNet.Core.CurlOptions.Clone().md 'CurlDotNet\.Core\.CurlOptions\.Clone\(\)') | Clone this options object\. |
