#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithData\(string\) Method

Sends [data](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md#CurlDotNet.Core.CurlRequestBuilder.WithData(string).data 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)\.data') as the request body \(curl's `-d` / `--data`\) with
`Content-Type: application/x-www-form-urlencoded`, switching a GET to POST\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithData(string data);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithData(string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The body text, such as `"a=1&b=2"` or a JSON string, or `"@file"` to send a
            file's contents\. Null or empty sends no body\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The string is sent as UTF-8 exactly as given; it is not URL-encoded for you, so
            `"name=Ada Lovelace&lang=en"` must already be encoded as `"name=Ada%20Lovelace&lang=en"`
            (or use [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)'), which encodes each pair). A value starting with `@`, such as
            `"@payload.json"`, sends the bytes of that file instead, like `curl -d @payload.json`.

To send another media type, add `.WithHeader("Content-Type", "text/plain")`; for JSON
            objects prefer [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)'). A request that was GET (or had no method) becomes POST; PUT,
            PATCH and DELETE keep their method.

Example: `CurlRequestBuilder.Post("https://httpbin.org/post").WithData("key1=value1&key2=value2")`
            is `curl -X POST -d 'key1=value1&key2=value2' https://httpbin.org/post`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/11-forms-and-data.md

### See Also
- [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)')
- [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)')
- [WithBinaryData\(byte\[\]\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryData(byte[]).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryData\(byte\[\]\)')
- [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data')
- [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)')