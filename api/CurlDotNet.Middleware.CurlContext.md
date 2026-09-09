#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## CurlContext Class

Context object containing information about the curl request\.

```csharp
public class CurlContext
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlContext

| Properties | |
| :--- | :--- |
| [CancellationToken](CurlDotNet.Middleware.CurlContext.CancellationToken.md 'CurlDotNet\.Middleware\.CurlContext\.CancellationToken') | The cancellation token for the request\. |
| [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') | The curl command being executed\. |
| [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options') | The parsed curl options\. |
| [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') | Custom properties for passing data between middleware\. |
| [StartTime](CurlDotNet.Middleware.CurlContext.StartTime.md 'CurlDotNet\.Middleware\.CurlContext\.StartTime') | Request start time for timing\. |

| Methods | |
| :--- | :--- |
| [GetProperty&lt;T&gt;\(string\)](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)') | Get a property from the context\. |
| [WithProperty\(string, object\)](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)') | Add a property to the context \(fluent\)\. |
