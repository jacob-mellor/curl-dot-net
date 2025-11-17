#### [CurlDotNet](index.md 'index')
### [CurlDotNet](index.md#CurlDotNet 'CurlDotNet')

## CurlApiClient Class

Simplified API client builder for common HTTP operations\.
Provides a more ergonomic alternative to raw curl commands\.

```csharp
public class CurlApiClient
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlApiClient
### Constructors

<a name='CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_)'></a>

## CurlApiClient\(string, Nullable\<TimeSpan\>\) Constructor

Create a new API client with a base URL\.

```csharp
public CurlApiClient(string baseUrl, System.Nullable<System.TimeSpan> defaultTimeout=null);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).baseUrl'></a>

`baseUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The base URL for all requests

<a name='CurlDotNet.CurlApiClient.CurlApiClient(string,System.Nullable_System.TimeSpan_).defaultTimeout'></a>

`defaultTimeout` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

Default timeout for requests
### Methods

<a name='CurlDotNet.CurlApiClient.DeleteAsync(string)'></a>

## CurlApiClient\.DeleteAsync\(string\) Method

Perform a DELETE request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DeleteAsync(string path);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.DeleteAsync(string).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.CurlApiClient.GetAsync(string)'></a>

## CurlApiClient\.GetAsync\(string\) Method

Perform a GET request\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string path);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.GetAsync(string).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.CurlApiClient.PatchJsonAsync(string,object)'></a>

## CurlApiClient\.PatchJsonAsync\(string, object\) Method

Perform a PATCH request with JSON body\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PatchJsonAsync(string path, object data);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.CurlApiClient.PatchJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.CurlApiClient.PostJsonAsync(string,object)'></a>

## CurlApiClient\.PostJsonAsync\(string, object\) Method

Perform a POST request with JSON body\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostJsonAsync(string path, object data);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.PostJsonAsync(string,object).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.CurlApiClient.PostJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.CurlApiClient.PutJsonAsync(string,object)'></a>

## CurlApiClient\.PutJsonAsync\(string, object\) Method

Perform a PUT request with JSON body\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PutJsonAsync(string path, object data);
```
#### Parameters

<a name='CurlDotNet.CurlApiClient.PutJsonAsync(string,object).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.CurlApiClient.PutJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')