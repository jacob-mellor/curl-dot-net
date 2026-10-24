#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.AppendJsonData\(CurlOptions, string\) Method

Appends JSON data to the options\. Unlike [CurlDotNet\.Core\.CommandParser\.AppendData\(CurlDotNet\.Core\.CurlOptions,System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/curldotnet.core.commandparser.appenddata#curldotnet-core-commandparser-appenddata(curldotnet-core-curloptions-system-string) 'CurlDotNet\.Core\.CommandParser\.AppendData\(CurlDotNet\.Core\.CurlOptions,System\.String\)'), multiple \-\-json
values are concatenated without any separator, matching curl's behavior\.
Curl 7\.82\+ introduced \-\-json as shorthand for \-\-data with JSON headers\.

```csharp
private void AppendJsonData(CurlDotNet.Core.CurlOptions options, string value);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.AppendJsonData(CurlDotNet.Core.CurlOptions,string).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

<a name='CurlDotNet.Core.CommandParser.AppendJsonData(CurlDotNet.Core.CurlOptions,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')