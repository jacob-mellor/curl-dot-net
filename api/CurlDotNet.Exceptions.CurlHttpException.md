#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlHttpException Class

Thrown for HTTP error responses when using ThrowOnError\(\)

```csharp
public class CurlHttpException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlHttpException

Derived  
&#8627; [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')  
&#8627; [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')
### Constructors

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string)'></a>

## CurlHttpException\(string, int, string, string, string\) Constructor

Initializes a new instance of the CurlHttpException class\.

```csharp
public CurlHttpException(string message, int statusCode, string statusText=null, string responseBody=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).statusText'></a>

`statusText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The HTTP status text\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).responseBody'></a>

`responseBody` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body content\.

<a name='CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the error\.
### Properties

<a name='CurlDotNet.Exceptions.CurlHttpException.IsClientError'></a>

## CurlHttpException\.IsClientError Property

Check if this is a client error \(4xx\)

```csharp
public bool IsClientError { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Exceptions.CurlHttpException.IsForbidden'></a>

## CurlHttpException\.IsForbidden Property

Check if this is forbidden \(403\)\.

```csharp
public bool IsForbidden { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Exceptions.CurlHttpException.IsNotFound'></a>

## CurlHttpException\.IsNotFound Property

Check if this is not found \(404\)\.

```csharp
public bool IsNotFound { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Exceptions.CurlHttpException.IsRateLimited'></a>

## CurlHttpException\.IsRateLimited Property

Check if this is a rate limit error \(429\)\.

```csharp
public bool IsRateLimited { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Exceptions.CurlHttpException.IsServerError'></a>

## CurlHttpException\.IsServerError Property

Check if this is a server error \(5xx\)

```csharp
public bool IsServerError { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Exceptions.CurlHttpException.IsUnauthorized'></a>

## CurlHttpException\.IsUnauthorized Property

Check if this is unauthorized \(401\)\.

```csharp
public bool IsUnauthorized { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Exceptions.CurlHttpException.ResponseBody'></a>

## CurlHttpException\.ResponseBody Property

Gets the response body content\.

```csharp
public string ResponseBody { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Exceptions.CurlHttpException.ResponseHeaders'></a>

## CurlHttpException\.ResponseHeaders Property

Gets or sets the response headers\.

```csharp
public System.Collections.Generic.Dictionary<string,string> ResponseHeaders { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Exceptions.CurlHttpException.StatusCode'></a>

## CurlHttpException\.StatusCode Property

Gets the HTTP status code of the response\.

```csharp
public int StatusCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Exceptions.CurlHttpException.StatusText'></a>

## CurlHttpException\.StatusText Property

Gets the HTTP status text of the response\.

```csharp
public string StatusText { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
### Methods

<a name='CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter()'></a>

## CurlHttpException\.GetRetryAfter\(\) Method

Get retry\-after value if present in headers\.

```csharp
public System.Nullable<System.TimeSpan> GetRetryAfter();
```

#### Returns
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Exceptions.CurlHttpException.IsRetryable()'></a>

## CurlHttpException\.IsRetryable\(\) Method

Determines whether the HTTP error is retryable\.

```csharp
public override bool IsRetryable();
```

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true if the error is retryable \(429 or 5xx status codes\); otherwise, false\.

<a name='CurlDotNet.Exceptions.CurlHttpException.IsStatus(int)'></a>

## CurlHttpException\.IsStatus\(int\) Method

Check if this is a specific status code\.

```csharp
public bool IsStatus(int code);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).code'></a>

`code` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='CurlDotNet.Exceptions.CurlHttpException.ToDetailedString()'></a>

## CurlHttpException\.ToDetailedString\(\) Method

Returns a detailed string representation of the exception including HTTP status information\.

```csharp
public override string ToDetailedString();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A detailed string representation of the exception\.

<a name='CurlDotNet.Exceptions.CurlHttpException.WithHeaders(System.Collections.Generic.Dictionary_string,string_)'></a>

## CurlHttpException\.WithHeaders\(Dictionary\<string,string\>\) Method

Add response headers \(fluent\)\.

```csharp
public CurlDotNet.Exceptions.CurlHttpException WithHeaders(System.Collections.Generic.Dictionary<string,string> headers);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.WithHeaders(System.Collections.Generic.Dictionary_string,string_).headers'></a>

`headers` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

#### Returns
[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')