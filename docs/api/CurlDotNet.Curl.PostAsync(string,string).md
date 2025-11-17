#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.PostAsync\(string, string\) Method


<b>Quick POST request - simpler syntax for posting data.</b>

Convenient method for simple POST requests with string data.

<b>Example:</b>

```csharp
// Post form data
var response = await Curl.Post(
    "https://api.example.com/login",
    "username=john&password=secret123"
);

// Post JSON data
var json = "{\"name\":\"John\",\"age\":30}";
var result = await Curl.Post("https://api.example.com/users", json);
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostAsync(string url, string data);
```
#### Parameters

<a name='CurlDotNet.Curl.PostAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to\.

<a name='CurlDotNet.Curl.PostAsync(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The data to send in the POST body\. Can be:
- JSON string: `"{\"key\":\"value\"}"`
- Form data: `"key1=value1&key2=value2"`
- XML or any other string content

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

### Remarks

This is equivalent to: `Curl.Execute($"curl -X POST -d '{data}' {url}")`

For POST with headers, use [PostJson\(string, object\)](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)') or the full [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') method.