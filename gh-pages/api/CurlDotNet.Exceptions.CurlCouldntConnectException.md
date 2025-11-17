#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlCouldntConnectException Class

CURLE\_COULDNT\_CONNECT \(7\) \- Failed to connect to host

```csharp
public class CurlCouldntConnectException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlCouldntConnectException
### Constructors

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string)'></a>

## CurlCouldntConnectException\(string, int, string\) Constructor

Initializes a new instance of the [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') class\.

```csharp
public CurlCouldntConnectException(string host, int port, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string).host'></a>

`host` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The host that could not be connected to\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string).port'></a>

`port` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The port number that was attempted\.

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.CurlCouldntConnectException(string,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing when the error occurred\.
### Properties

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.Host'></a>

## CurlCouldntConnectException\.Host Property

Gets the host that could not be connected to

```csharp
public string Host { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Exceptions.CurlCouldntConnectException.Port'></a>

## CurlCouldntConnectException\.Port Property

Gets the port number that could not be connected to

```csharp
public int Port { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')