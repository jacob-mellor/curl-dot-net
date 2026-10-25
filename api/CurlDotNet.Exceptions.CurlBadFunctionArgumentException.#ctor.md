#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadFunctionArgumentException](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException')

## CurlBadFunctionArgumentException Constructors

| Overloads | |
| :--- | :--- |
| [CurlBadFunctionArgumentException\(string, string\)](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.#ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string) 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.CurlBadFunctionArgumentException\(string, string\)') | Creates the error \(exit code 43\) naming the invalid argument, with advice to correct or remove it\. |
| [CurlBadFunctionArgumentException\(string, string, string\)](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.#ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string,string) 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.CurlBadFunctionArgumentException\(string, string, string\)') | Creates the error for a combination of options curl refuses, explaining which options clash and what to use instead\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string)'></a>

## CurlBadFunctionArgumentException\(string, string\) Constructor

Creates the error \(exit code 43\) naming the invalid argument, with advice to correct or remove it\.

```csharp
public CurlBadFunctionArgumentException(string argumentName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string).argumentName'></a>

`argumentName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the argument or option that was invalid\.

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Bad function argument: NAME`.

### See Also
- [CurlBadFunctionArgumentException\(string, string, string\)](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.#ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string,string) 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.CurlBadFunctionArgumentException\(string, string, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string,string)'></a>

## CurlBadFunctionArgumentException\(string, string, string\) Constructor

Creates the error for a combination of options curl refuses, explaining which options clash and what to use
instead\.

```csharp
public CurlBadFunctionArgumentException(string argumentName, string detail, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string,string).argumentName'></a>

`argumentName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The option that cannot be used this way, such as `-C/--continue-at`\.

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string,string).detail'></a>

`detail` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What is wrong and how to fix it \(a fix sentence may follow on the same line\)\.

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

Example: `-C 100 -d data` gives "cannot be combined with a request body" and the advice to send the whole body again.

### See Also
- [CurlBadFunctionArgumentException\(string, string\)](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.#ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string) 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.CurlBadFunctionArgumentException\(string, string\)')