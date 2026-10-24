#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[AwsSigV4Signer](CurlDotNet.Core.AwsSigV4Signer.md 'CurlDotNet\.Core\.AwsSigV4Signer')

## AwsSigV4Signer\.SignRequest\(HttpRequestMessage, AwsSigV4Config, string, string, Nullable\<DateTime\>\) Method

Signs an [System\.Net\.Http\.HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage 'System\.Net\.Http\.HttpRequestMessage') using AWS Signature Version 4\.
Adds the required Authorization, date, and content hash headers to the request\.

```csharp
public static void SignRequest(System.Net.Http.HttpRequestMessage request, CurlDotNet.Core.AwsSigV4Config config, string accessKey, string secretKey, System.Nullable<System.DateTime> timestamp=null);
```
#### Parameters

<a name='CurlDotNet.Core.AwsSigV4Signer.SignRequest(System.Net.Http.HttpRequestMessage,CurlDotNet.Core.AwsSigV4Config,string,string,System.Nullable_System.DateTime_).request'></a>

`request` [System\.Net\.Http\.HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage 'System\.Net\.Http\.HttpRequestMessage')

The HTTP request to sign\. Will be modified in\-place with signing headers\.

<a name='CurlDotNet.Core.AwsSigV4Signer.SignRequest(System.Net.Http.HttpRequestMessage,CurlDotNet.Core.AwsSigV4Config,string,string,System.Nullable_System.DateTime_).config'></a>

`config` [AwsSigV4Config](CurlDotNet.Core.AwsSigV4Config.md 'CurlDotNet\.Core\.AwsSigV4Config')

The SigV4 configuration specifying provider, region, and service\.

<a name='CurlDotNet.Core.AwsSigV4Signer.SignRequest(System.Net.Http.HttpRequestMessage,CurlDotNet.Core.AwsSigV4Config,string,string,System.Nullable_System.DateTime_).accessKey'></a>

`accessKey` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The access key ID \(from \-\-user parameter, before the colon\)\.

<a name='CurlDotNet.Core.AwsSigV4Signer.SignRequest(System.Net.Http.HttpRequestMessage,CurlDotNet.Core.AwsSigV4Config,string,string,System.Nullable_System.DateTime_).secretKey'></a>

`secretKey` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The secret access key \(from \-\-user parameter, after the colon\)\.

<a name='CurlDotNet.Core.AwsSigV4Signer.SignRequest(System.Net.Http.HttpRequestMessage,CurlDotNet.Core.AwsSigV4Config,string,string,System.Nullable_System.DateTime_).timestamp'></a>

`timestamp` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime 'System\.DateTime')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

Optional timestamp override for deterministic testing\. Uses UTC now if null\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
Thrown when required parameters are null\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
Thrown when region or service cannot be determined\.