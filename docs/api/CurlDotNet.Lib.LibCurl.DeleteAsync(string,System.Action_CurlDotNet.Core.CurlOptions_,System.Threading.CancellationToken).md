#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.DeleteAsync\(string, Action\<CurlOptions\>, CancellationToken\) Method

Sends a DELETE request \(`curl -X DELETE URL`\) to remove the resource at [url](CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url 'CurlDotNet\.Lib\.LibCurl\.DeleteAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.url');
most APIs answer 200, 202 or 204\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DeleteAsync(string url, System.Action<CurlDotNet.Core.CurlOptions> configure=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL of the resource to delete\.

<a name='CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Optional callback that adjusts this request's [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions'); null for none\.

<a name='CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Token that cancels the request; `default` for none\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'); for a transfer failure `StatusCode` 0 and the error in
            [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception')\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
[cancellationToken](CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Lib\.LibCurl\.DeleteAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.cancellationToken') was cancelled\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/12\-cancellation\-tokens\.md

### Remarks

Example: `var r = await curl.DeleteAsync("https://api.example.com/users/42");` then
            `if (r.StatusCode == 204) Console.WriteLine("deleted");`. To send a body with a DELETE, set
            `opts => opts.Data = "..."` in [configure](CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md#CurlDotNet.Lib.LibCurl.DeleteAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).configure 'CurlDotNet\.Lib\.LibCurl\.DeleteAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)\.configure').

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [Delete\(string\)](CurlDotNet.Core.CurlRequestBuilder.Delete(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Delete\(string\)')