#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlProxyException Class

Thrown when proxy connection fails

```csharp
public class CurlProxyException : CurlDotNet.Exceptions.CurlConnectionException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md 'CurlDotNet\.Exceptions\.CurlConnectionException') &#129106; CurlProxyException
### Constructors

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string)'></a>

## CurlProxyException\(string, string, Nullable\<int\>, string\) Constructor

Initializes a new instance of the [CurlProxyException](CurlDotNet.Exceptions.CurlProxyException.md 'CurlDotNet\.Exceptions\.CurlProxyException') class

```csharp
public CurlProxyException(string message, string proxyHost, System.Nullable<int> proxyPort=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message describing the proxy connection failure

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).proxyHost'></a>

`proxyHost` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The proxy hostname or IP address

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).proxyPort'></a>

`proxyPort` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The proxy port number, if applicable

<a name='CurlDotNet.Exceptions.CurlProxyException.CurlProxyException(string,string,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed
### Properties

<a name='CurlDotNet.Exceptions.CurlProxyException.ProxyHost'></a>

## CurlProxyException\.ProxyHost Property

Gets the proxy hostname or IP address that failed to connect

```csharp
public string ProxyHost { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Exceptions.CurlProxyException.ProxyPort'></a>

## CurlProxyException\.ProxyPort Property

Gets the proxy port number, if specified

```csharp
public System.Nullable<int> ProxyPort { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')