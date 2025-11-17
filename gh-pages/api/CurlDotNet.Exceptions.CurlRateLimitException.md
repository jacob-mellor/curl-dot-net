#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlRateLimitException Class

Thrown when rate limiting is encountered

```csharp
public class CurlRateLimitException : CurlDotNet.Exceptions.CurlHttpException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') &#129106; CurlRateLimitException
### Constructors

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string)'></a>

## CurlRateLimitException\(string, Nullable\<TimeSpan\>, Nullable\<int\>, string\) Constructor

Initializes a new instance of the [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException') class

```csharp
public CurlRateLimitException(string message, System.Nullable<System.TimeSpan> retryAfter=null, System.Nullable<int> remainingLimit=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the rate limit

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).retryAfter'></a>

`retryAfter` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The time to wait before retrying

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).remainingLimit'></a>

`remainingLimit` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The number of requests remaining

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed
### Properties

<a name='CurlDotNet.Exceptions.CurlRateLimitException.RemainingLimit'></a>

## CurlRateLimitException\.RemainingLimit Property

Gets the number of remaining requests allowed, if provided

```csharp
public System.Nullable<int> RemainingLimit { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='CurlDotNet.Exceptions.CurlRateLimitException.RetryAfter'></a>

## CurlRateLimitException\.RetryAfter Property

Gets the time to wait before retrying, if provided by the server

```csharp
public System.Nullable<System.TimeSpan> RetryAfter { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')