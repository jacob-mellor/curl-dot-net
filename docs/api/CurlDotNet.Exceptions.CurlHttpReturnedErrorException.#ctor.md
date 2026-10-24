#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')

## CurlHttpReturnedErrorException Constructors

| Overloads | |
| :--- | :--- |
| [CurlHttpReturnedErrorException\(int, string, string, string\)](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.#ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string) 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException\.CurlHttpReturnedErrorException\(int, string, string, string\)') | Creates the error \(exit code 22\) from a status code, reason phrase and body, with status\-specific advice in the message\. |
| [CurlHttpReturnedErrorException\(int, string, string, string, bool, string\)](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.#ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string) 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException\.CurlHttpReturnedErrorException\(int, string, string, string, bool, string\)') | Creates the error for `-f`/`--fail` or `--fail-with-body` with curl's wording `The requested URL returned error: 404` and the URL \(without credentials or query string\)\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string)'></a>

## CurlHttpReturnedErrorException\(int, string, string, string\) Constructor

Creates the error \(exit code 22\) from a status code, reason phrase and body, with status\-specific advice in the
message\.

```csharp
public CurlHttpReturnedErrorException(int statusCode, string statusText, string body, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code of the response, 400 or above\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).statusText'></a>

`statusText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The reason phrase from the status line, such as `Not Found`, or null\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).body'></a>

`body` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body, or null when it was not read\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

Message: `HTTP error 404: Not Found`, the advice for that status, and the HTTP errors page.
            Example: `throw new CurlHttpReturnedErrorException(503, "Service Unavailable", body, command);`

### See Also
- [CurlHttpReturnedErrorException\(int, string, string, string, bool, string\)](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.#ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string) 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException\.CurlHttpReturnedErrorException\(int, string, string, string, bool, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string)'></a>

## CurlHttpReturnedErrorException\(int, string, string, string, bool, string\) Constructor

Creates the error for `-f`/`--fail` or `--fail-with-body` with curl's wording
`The requested URL returned error: 404` and the URL \(without credentials or query string\)\.

```csharp
public CurlHttpReturnedErrorException(int statusCode, string? statusText, string? body, string url, bool bodyKept, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code \(400 or above\)\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string).statusText'></a>

`statusText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The reason phrase from the status line, or null\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string).body'></a>

`body` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body text, if any, otherwise null\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL that returned the error; printed without user:password and query string\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string).bodyKept'></a>

`bodyKept` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True for `--fail-with-body`, false for `-f`\.

<a name='CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed, or null\.

### Remarks

With `-f` the body was not written anywhere; with `--fail-with-body` it was written to the output
            as usual and is also available in [ResponseBody](CurlDotNet.Exceptions.CurlHttpException.ResponseBody.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseBody').
            [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is 22 either way. The second line explains what `-f` did; the
            status-specific advice follows in [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions').

### See Also
- [CurlHttpReturnedErrorException\(int, string, string, string\)](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.#ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string) 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException\.CurlHttpReturnedErrorException\(int, string, string, string\)')