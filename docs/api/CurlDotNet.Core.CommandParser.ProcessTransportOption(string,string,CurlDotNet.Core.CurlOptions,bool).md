#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.ProcessTransportOption\(string, string, CurlOptions, bool\) Method

Handles transport\-side options\. Returns true when [option](CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).md#CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).option 'CurlDotNet\.Core\.CommandParser\.ProcessTransportOption\(string, string, CurlDotNet\.Core\.CurlOptions, bool\)\.option') was recognised\.

```csharp
private bool ProcessTransportOption(string option, string value, CurlDotNet.Core.CurlOptions options, ref bool methodSpecified);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).option'></a>

`option` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

<a name='CurlDotNet.Core.CommandParser.ProcessTransportOption(string,string,CurlDotNet.Core.CurlOptions,bool).methodSpecified'></a>

`methodSpecified` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')