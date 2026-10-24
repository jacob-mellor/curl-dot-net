#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithUploadFile\(string, string\) Method

Upload a file \(multipart/form\-data\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithUploadFile(string parameterName, string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).parameterName'></a>

`parameterName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The form field name

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to the file

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')