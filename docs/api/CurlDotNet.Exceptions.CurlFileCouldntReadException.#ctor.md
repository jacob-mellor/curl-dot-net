#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException')

## CurlFileCouldntReadException Constructors

| Overloads | |
| :--- | :--- |
| [CurlFileCouldntReadException\(string, string\)](CurlDotNet.Exceptions.CurlFileCouldntReadException.#ctor.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string) 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string\)') | Creates the error \(exit code 37\) for a `file://` path that cannot be opened, with curl's wording `Couldn't open file PATH`\. |
| [CurlFileCouldntReadException\(string, string, string\)](CurlDotNet.Exceptions.CurlFileCouldntReadException.#ctor.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string) 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string, string\)') | Creates the error \(exit code 37\) for [filePath](CurlDotNet.Exceptions.CurlFileCouldntReadException.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string, string\)\.filePath') with the caller's own explanation and fix, for situations other than a `file://` URL \(such as `-d @missing.json`\)\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string)'></a>

## CurlFileCouldntReadException\(string, string\) Constructor

Creates the error \(exit code 37\) for a `file://` path that cannot be opened, with curl's wording
`Couldn't open file PATH`\.

```csharp
public CurlFileCouldntReadException(string filePath, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path that could not be read\.

<a name='CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Example: `throw new CurlFileCouldntReadException("/data/missing.json", command);`

### See Also
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

<a name='ctor.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string)'></a>

## CurlFileCouldntReadException\(string, string, string\) Constructor

Creates the error \(exit code 37\) for [filePath](CurlDotNet.Exceptions.CurlFileCouldntReadException.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string).filePath 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string, string\)\.filePath') with the caller's own explanation and fix,
for situations other than a `file://` URL \(such as `-d @missing.json`\)\.

```csharp
public CurlFileCouldntReadException(string filePath, string message, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path that could not be opened\.

<a name='CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What happened \(first line\) and how to fix it \(following lines\)\.

<a name='CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Example: `throw new CurlFileCouldntReadException(path, "Couldn't open file 'body.json' given with -d @body.json: it does not exist.\nCheck the path.", command);`

### See Also
- [CurlFileCouldntReadException\(string, string\)](CurlDotNet.Exceptions.CurlFileCouldntReadException.#ctor.md#CurlDotNet.Exceptions.CurlFileCouldntReadException.CurlFileCouldntReadException(string,string) 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException\.CurlFileCouldntReadException\(string, string\)')