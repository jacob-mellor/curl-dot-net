#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.IsStatus\(int\) Method

Checks for one exact status code, for `when` filters such as `catch (CurlHttpException ex) when (ex.IsStatus(409))`\.

```csharp
public bool IsStatus(int code);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).code'></a>

`code` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code to compare with, such as 404 or 409\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') equals [code](CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).md#CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).code 'CurlDotNet\.Exceptions\.CurlHttpException\.IsStatus\(int\)\.code')\.

### Remarks

Same as `ex.StatusCode == code`; reads better in exception filters.

### See Also
- [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode')