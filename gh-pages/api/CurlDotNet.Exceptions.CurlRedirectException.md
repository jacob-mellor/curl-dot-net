#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlRedirectException Class

Thrown when redirect limit is exceeded during HTTP requests\.

```csharp
public class CurlRedirectException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlRedirectException

### Example

```csharp
try
{
    // Default limit is usually 50 redirects
    var result = await curl.ExecuteAsync("curl -L https://example.com/redirect-loop");
}
catch (CurlRedirectException ex)
{
    Console.WriteLine($"Too many redirects: {ex.RedirectCount}");
    Console.WriteLine($"Last URL attempted: {ex.LastUrl}");

    // Try again with limited redirects
    result = await curl.ExecuteAsync("curl --max-redirs 5 https://example.com/redirect-loop");
}
```

### Remarks

This exception indicates that the maximum number of HTTP redirects has been exceeded.

Curl error code: CURLE_TOO_MANY_REDIRECTS (47)

Common causes: redirect loops, misconfigured servers, or intentional redirect chains.

AI-Usage: Catch this to handle redirect issues separately from other HTTP errors.

AI-Pattern: Check RedirectCount and LastUrl to debug redirect chains.

For more information, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md
### Constructors

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string)'></a>

## CurlRedirectException\(string, int, string, string\) Constructor

Initializes a new instance of the [CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException') class\.

```csharp
public CurlRedirectException(string message, int redirectCount, string lastUrl=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the redirect limit exceeded\.

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).redirectCount'></a>

`redirectCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of redirects that were attempted\.

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).lastUrl'></a>

`lastUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The last URL in the redirect chain\.

<a name='CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing\.
### Properties

<a name='CurlDotNet.Exceptions.CurlRedirectException.LastUrl'></a>

## CurlRedirectException\.LastUrl Property

Gets the last URL that was attempted before the redirect limit was exceeded\.

```csharp
public string LastUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The final URL in the redirect chain, or null if not available\.

### Remarks

This URL can help identify where the redirect loop or chain ends.

AI-Usage: Use this to debug redirect chains and identify problematic URLs.

<a name='CurlDotNet.Exceptions.CurlRedirectException.RedirectCount'></a>

## CurlRedirectException\.RedirectCount Property

Gets the number of redirects that were followed before the limit was exceeded\.

```csharp
public int RedirectCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The count of redirects attempted\.

### Remarks

This helps identify potential redirect loops or excessive redirect chains.

AI-Usage: If count is high, suspect a redirect loop.