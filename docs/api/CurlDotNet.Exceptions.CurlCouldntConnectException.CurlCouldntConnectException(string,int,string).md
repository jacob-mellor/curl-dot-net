#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException')

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