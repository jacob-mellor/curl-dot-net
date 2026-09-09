#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## ICommandParser Interface

Interface for parsing curl command strings into options\.

```csharp
internal interface ICommandParser
```

Derived  
&#8627; [CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

| Methods | |
| :--- | :--- |
| [IsValid\(string\)](CurlDotNet.Core.ICommandParser.IsValid(string).md 'CurlDotNet\.Core\.ICommandParser\.IsValid\(string\)') | Validate a curl command without fully parsing\. |
| [Parse\(string\)](CurlDotNet.Core.ICommandParser.Parse(string).md 'CurlDotNet\.Core\.ICommandParser\.Parse\(string\)') | Parse a curl command string into options\. |
