#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.Configure\(Action\<CurlOptions\>\) Method

Configure default options using an action\.

```csharp
public CurlDotNet.Lib.LibCurl Configure(System.Action<CurlDotNet.Core.CurlOptions> configure);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Configuration action

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance for method chaining

### Example

```csharp
using (var curl = new LibCurl())
{
    curl.Configure(opts => {
        opts.FollowRedirects = true;
        opts.MaxTime = 60;
        opts.Insecure = false;
    });
}
```