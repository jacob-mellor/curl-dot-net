#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

## CurlHttpException Class


<b>Exception for HTTP errors (4xx, 5xx status codes).</b>

Thrown by EnsureSuccess() when request fails:

```csharp
try
{
    result.EnsureSuccess();
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
    Console.WriteLine($"Response was: {ex.ResponseBody}");
}
```

```csharp
public class CurlHttpException : System.Exception
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; CurlHttpException
### Constructors

<a name='CurlDotNet.Core.CurlHttpException.CurlHttpException(string,int)'></a>

## CurlHttpException\(string, int\) Constructor

Initializes a new instance of the CurlHttpException class\.

```csharp
public CurlHttpException(string message, int statusCode);
```
#### Parameters

<a name='CurlDotNet.Core.CurlHttpException.CurlHttpException(string,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Core.CurlHttpException.CurlHttpException(string,int).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code\.
### Properties

<a name='CurlDotNet.Core.CurlHttpException.ResponseBody'></a>

## CurlHttpException\.ResponseBody Property

The response body \(may contain error details\)

```csharp
public string ResponseBody { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlHttpException.ResponseHeaders'></a>

## CurlHttpException\.ResponseHeaders Property

The response headers

```csharp
public System.Collections.Generic.Dictionary<string,string> ResponseHeaders { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Core.CurlHttpException.StatusCode'></a>

## CurlHttpException\.StatusCode Property

The HTTP status code that caused the error

```csharp
public int StatusCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')