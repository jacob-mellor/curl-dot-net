#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlOperationTimeoutException Class

CURLE\_OPERATION\_TIMEDOUT \(28\) \- Operation timeout

```csharp
public class CurlOperationTimeoutException : CurlDotNet.Exceptions.CurlTimeoutException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') &#129106; CurlOperationTimeoutException
### Constructors

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string)'></a>

## CurlOperationTimeoutException\(double, string\) Constructor

Initializes a new instance of the [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') class\.

```csharp
public CurlOperationTimeoutException(double timeoutSeconds, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).timeoutSeconds'></a>

`timeoutSeconds` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The timeout value in seconds that was exceeded\.

<a name='CurlDotNet.Exceptions.CurlOperationTimeoutException.CurlOperationTimeoutException(double,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.