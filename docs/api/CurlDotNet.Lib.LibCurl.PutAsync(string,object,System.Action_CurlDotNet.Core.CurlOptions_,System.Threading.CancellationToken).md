#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.PutAsync\(string, object, Action\<CurlOptions\>, CancellationToken\) Method

Sends a PUT request \(`curl -X PUT`\) to create or replace the resource at [url](CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url 'CurlDotNet\.Lib\.LibCurl\.PutAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.url');
the body follows the same rules as [PostAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PostAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PostAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') \(string as\-is, object as JSON\)\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PutAsync(string url, object data=null, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL of the resource\.

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The body: null, a string sent as\-is, or an object serialized to JSON\.

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional callback that adjusts this request's [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions'); null for none\.

<a name='CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Token that cancels the request; `default` for none\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'); for a transfer failure `StatusCode` 0 and the error in
            [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception')\.

#### Exceptions

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
[data](CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).data 'CurlDotNet\.Lib\.LibCurl\.PutAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.data') is an object that cannot be serialized to JSON
            \(a reference cycle, a delegate or stream member\); the parser's exception is the InnerException\. Fix: send a plain
            DTO or a JSON string\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-post\-json\-csharp/README\.md

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
[cancellationToken](CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.PutAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Lib\.LibCurl\.PutAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.cancellationToken') was cancelled\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/12\-cancellation\-tokens\.md

### Remarks

Example: `await curl.PutAsync("https://api.example.com/users/42", new { name = "Jane" });` sends
            `{"name":"Jane"}` with `Content-Type: application/json`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [Put\(string\)](CurlDotNet.Core.CurlRequestBuilder.Put(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Put\(string\)')
- [PatchAsync\(string, object, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.PatchAsync(string,object,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PatchAsync\(string, object, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')