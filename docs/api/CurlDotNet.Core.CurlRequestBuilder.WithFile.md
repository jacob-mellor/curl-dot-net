#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithFile Method

| Overloads | |
| :--- | :--- |
| [WithFile\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string\)') | Upload a file \(multipart/form\-data\)\. Alias for WithUploadFile\. |
| [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)') | Upload a file\. |

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string)'></a>

## CurlRequestBuilder\.WithFile\(string\) Method

Upload a file \(multipart/form\-data\)\. Alias for WithUploadFile\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path to the file

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string)'></a>

## CurlRequestBuilder\.WithFile\(string, string\) Method

Upload a file\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithFile(string fieldName, string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).fieldName'></a>

`fieldName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')