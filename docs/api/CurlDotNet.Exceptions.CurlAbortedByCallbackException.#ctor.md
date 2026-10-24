#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')

## CurlAbortedByCallbackException Constructors

| Overloads | |
| :--- | :--- |
| [CurlAbortedByCallbackException\(string\)](CurlDotNet.Exceptions.CurlAbortedByCallbackException.#ctor.md#CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string) 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException\.CurlAbortedByCallbackException\(string\)') | Creates the error \(exit code 42\) for a transfer cancelled through the caller's `CancellationToken`\. |
| [CurlAbortedByCallbackException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlAbortedByCallbackException.#ctor.md#CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException\.CurlAbortedByCallbackException\(string, string, System\.Exception\)') | Creates the exception for a transfer that your own callback \(for example a progress handler set with [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)')\) stopped by throwing\. |

<a name='ctor.md#CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string)'></a>

## CurlAbortedByCallbackException\(string\) Constructor

Creates the error \(exit code 42\) for a transfer cancelled through the caller's `CancellationToken`\.

```csharp
public CurlAbortedByCallbackException(string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Operation aborted by callback` with advice to check who cancelled the token. Thrown when the
            token passed to `ExecuteAsync` is cancelled.

### See Also
- [CurlAbortedByCallbackException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlAbortedByCallbackException.#ctor.md#CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException\.CurlAbortedByCallbackException\(string, string, System\.Exception\)')

<a name='ctor.md#CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string,string,System.Exception)'></a>

## CurlAbortedByCallbackException\(string, string, Exception\) Constructor

Creates the exception for a transfer that your own callback \(for example a progress handler set
with [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)')\) stopped by throwing\.

```csharp
public CurlAbortedByCallbackException(string message, string? command, System.Exception? innerException);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string,string,System.Exception).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the callback did and how to fix it, separated by a newline\.

<a name='CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string,string,System.Exception).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that was executing, or null\.

<a name='CurlDotNet.Exceptions.CurlAbortedByCallbackException.CurlAbortedByCallbackException(string,string,System.Exception).innerException'></a>

`innerException` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The exception the callback threw\.

### Remarks

curl ends a transfer with exit code 42 when its progress callback returns non-zero; CurlDotNet
            does the same when a callback throws, and keeps the callback's exception as
            [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException').

Example: `settings.WithProgress((pct, got, total) => { if (got > limit) throw new OperationCanceledException(); })`
            ends the download with this exception once `limit` bytes have arrived.

### See Also
- [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler')