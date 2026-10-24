#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')

## CurlTooManyRedirectsException Constructors

| Overloads | |
| :--- | :--- |
| [CurlTooManyRedirectsException\(int, int, string\)](CurlDotNet.Exceptions.CurlTooManyRedirectsException.#ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string) 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, int, string\)') | Creates the error using curl's wording `Maximum (N) redirects followed`, naming both the limit and the number of hops\. |
| [CurlTooManyRedirectsException\(int, string\)](CurlDotNet.Exceptions.CurlTooManyRedirectsException.#ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string) 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, string\)') | Creates the error \(exit code 47\) for a redirect chain of [count](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).count 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, string\)\.count') hops, with advice to look for a loop or raise the limit\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string)'></a>

## CurlTooManyRedirectsException\(int, int, string\) Constructor

Creates the error using curl's wording `Maximum (N) redirects followed`, naming both the limit and the
number of hops\.

```csharp
public CurlTooManyRedirectsException(int count, int limit, string? command);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

How many redirects were followed before the next one was refused\.

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string).limit'></a>

`limit` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The `--max-redirs` limit \(default 50\)\.

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

### Remarks

Thrown by the redirect handler when `--max-redirs` is reached.

### See Also
- [CurlTooManyRedirectsException\(int, string\)](CurlDotNet.Exceptions.CurlTooManyRedirectsException.#ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string) 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, string\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string)'></a>

## CurlTooManyRedirectsException\(int, string\) Constructor

Creates the error \(exit code 47\) for a redirect chain of [count](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).count 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, string\)\.count') hops, with advice to look for a
loop or raise the limit\.

```csharp
public CurlTooManyRedirectsException(int count, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of redirects that were followed before failing\.

<a name='CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Too many redirects: N`.

### See Also
- [CurlTooManyRedirectsException\(int, int, string\)](CurlDotNet.Exceptions.CurlTooManyRedirectsException.#ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string) 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, int, string\)')