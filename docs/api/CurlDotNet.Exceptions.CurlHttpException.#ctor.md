#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException Constructors

| Overloads | |
| :--- | :--- |
| [CurlHttpException\(string, int, string, string, string\)](CurlDotNet.Exceptions.CurlHttpException.#ctor.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string) 'CurlDotNet\.Exceptions\.CurlHttpException\.CurlHttpException\(string, int, string, string, string\)') | Creates an HTTP error from a status code and the response, for code that turns a `CurlResult` with a 4xx/5xx status into an exception\. |
| [CurlHttpException\(string, int, string, string, string, int\)](CurlDotNet.Exceptions.CurlHttpException.#ctor.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int) 'CurlDotNet\.Exceptions\.CurlHttpException\.CurlHttpException\(string, int, string, string, string, int\)') | Creates an HTTP error that also carries a curl exit code, for HTTP failures that end a command \(such as `-f`/`--fail`, exit code 22\) rather than being returned as a result\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string)'></a>

## CurlHttpException\(string, int, string, string, string\) Constructor

Creates an HTTP error from a status code and the response, for code that turns a `CurlResult` with a
4xx/5xx status into an exception\.

```csharp
public CurlHttpException(string message, int statusCode, string statusText=null, string responseBody=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed, naming the method and URL; optionally followed by fix lines and a `See URL` line\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code of the response, such as 404\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).statusText'></a>

`statusText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The reason phrase from the status line, such as `Not Found`, or null\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).responseBody'></a>

`responseBody` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body text, or null when it was not read\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the error, or null\.

### Remarks

[CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') stays null (curl only reports 22 with `-f`). When
            [message](CurlDotNet.Exceptions.CurlHttpException.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).message 'CurlDotNet\.Exceptions\.CurlHttpException\.CurlHttpException\(string, int, string, string, string\)\.message') carries no fix, the advice for [statusCode](CurlDotNet.Exceptions.CurlHttpException.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).statusCode 'CurlDotNet\.Exceptions\.CurlHttpException\.CurlHttpException\(string, int, string, string, string\)\.statusCode') is used (401: check the
            credentials, 404: check the URL, 429: wait, 5xx: retry later) and all advice lines go into
            [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions').

Example: `throw new CurlHttpException($"POST {url} returned {r.StatusCode}", r.StatusCode, null, r.Body, r.Command);`

### See Also
- [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int)'></a>

## CurlHttpException\(string, int, string, string, string, int\) Constructor

Creates an HTTP error that also carries a curl exit code, for HTTP failures that end a command \(such as
`-f`/`--fail`, exit code 22\) rather than being returned as a result\.

```csharp
protected CurlHttpException(string message, int statusCode, string? statusText, string? responseBody, string? command, int curlErrorCode);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened and how to fix it, separated by newlines\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code of the response, such as 404\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int).statusText'></a>

`statusText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The reason phrase from the status line, or null\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int).responseBody'></a>

`responseBody` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body, when it was read; otherwise null\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the error, or null\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int).curlErrorCode'></a>

`curlErrorCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The curl exit code, for example 22 \(CURLE\_HTTP\_RETURNED\_ERROR\)\.

### Remarks

Used by [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException').

### See Also
- [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')