#### [CurlDotNet](index.md 'index')
### [CurlDotNet](index.md#CurlDotNet 'CurlDotNet')

## ErgonomicExtensions Class

Ergonomic extension methods for CurlResult to improve developer experience\.
These extensions are in the CurlDotNet namespace for easy discovery\.

```csharp
public static class ErgonomicExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ErgonomicExtensions
### Methods

<a name='CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult)'></a>

## ErgonomicExtensions\.EnsureSuccessStatusCode\(this CurlResult\) Method

Throw an exception if the request was not successful\.
Similar to HttpResponseMessage\.EnsureSuccessStatusCode\(\)\.

```csharp
public static CurlDotNet.Core.CurlResult EnsureSuccessStatusCode(this CurlDotNet.Core.CurlResult result);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.EnsureSuccessStatusCode(thisCurlDotNet.Core.CurlResult).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult to check

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The same CurlResult for chaining

#### Exceptions

[CurlHttpException](CurlDotNet.Core.CurlHttpException.md 'CurlDotNet\.Core\.CurlHttpException')  
If the status code indicates failure

<a name='CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string)'></a>

## ErgonomicExtensions\.GetHeader\(this CurlResult, string\) Method

Get a specific header value from the response\.

```csharp
public static string? GetHeader(this CurlDotNet.Core.CurlResult result, string headerName);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult

<a name='CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The header name \(case\-insensitive\)

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The header value if found, null otherwise

<a name='CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string)'></a>

## ErgonomicExtensions\.HasContentType\(this CurlResult, string\) Method

Check if the response has a specific content type\.

```csharp
public static bool HasContentType(this CurlDotNet.Core.CurlResult result, string contentType);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult

<a name='CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).contentType'></a>

`contentType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The content type to check for

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if the content type matches

<a name='CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions)'></a>

## ErgonomicExtensions\.ParseJson\<T\>\(this CurlResult, JsonSerializerOptions\) Method

Parse the response body as JSON and deserialize to a strongly\-typed object\.

```csharp
public static T ParseJson<T>(this CurlDotNet.Core.CurlResult result, System.Text.Json.JsonSerializerOptions? options=null);
```
#### Type parameters

<a name='CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T'></a>

`T`

The type to deserialize to
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult to parse

<a name='CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).options'></a>

`options` [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')

Optional JSON serializer options

#### Returns
[T](CurlDotNet.ErgonomicExtensions.md#CurlDotNet.ErgonomicExtensions.ParseJson_T_(thisCurlDotNet.Core.CurlResult,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.ParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, System\.Text\.Json\.JsonSerializerOptions\)\.T')  
The deserialized object

#### Exceptions

[System\.Text\.Json\.JsonException](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonexception 'System\.Text\.Json\.JsonException')  
If the body is not valid JSON

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
If the result is not successful

<a name='CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string)'></a>

## ErgonomicExtensions\.SaveToFile\(this CurlResult, string\) Method

Save the response body to a file\.

```csharp
public static long SaveToFile(this CurlDotNet.Core.CurlResult result, string filePath);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult containing the data

<a name='CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path to save to

#### Returns
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
The number of bytes written

<a name='CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult)'></a>

## ErgonomicExtensions\.ToSimple\(this CurlResult\) Method

Convert the CurlResult to a simplified success/error tuple\.

```csharp
public static (bool Success,string? Body,string? Error) ToSimple(this CurlDotNet.Core.CurlResult result);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.ToSimple(thisCurlDotNet.Core.CurlResult).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult

#### Returns
[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')  
A tuple of \(success, body, error\)

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions)'></a>

## ErgonomicExtensions\.TryParseJson\<T\>\(this CurlResult, T, JsonSerializerOptions\) Method

Try to parse the response body as JSON\. Returns false if parsing fails\.

```csharp
public static bool TryParseJson<T>(this CurlDotNet.Core.CurlResult result, out T value, System.Text.Json.JsonSerializerOptions? options=null)
    where T : class;
```
#### Type parameters

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).T'></a>

`T`

The type to deserialize to
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The CurlResult to parse

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).value'></a>

`value` [T](CurlDotNet.ErgonomicExtensions.md#CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).T 'CurlDotNet\.ErgonomicExtensions\.TryParseJson\<T\>\(this CurlDotNet\.Core\.CurlResult, T, System\.Text\.Json\.JsonSerializerOptions\)\.T')

The deserialized object if successful

<a name='CurlDotNet.ErgonomicExtensions.TryParseJson_T_(thisCurlDotNet.Core.CurlResult,T,System.Text.Json.JsonSerializerOptions).options'></a>

`options` [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')

Optional JSON serializer options

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if parsing succeeded, false otherwise