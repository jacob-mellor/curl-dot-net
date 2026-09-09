#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.Execute Method

| Overloads | |
| :--- | :--- |
| [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') |   <b>⚠️ SYNCHRONOUS - Execute curl command and WAIT for it to complete (BLOCKS thread).</b>  This method BLOCKS your thread until the HTTP request completes. Your application              will FREEZE during this time. Only use when async is not possible.  <b>When to use SYNC (this method):</b>\.\.\.  <b>Example - When sync is OK:</b>\`\.\.\.\`  <b>⚠️ WARNING:</b> This blocks your thread. The application cannot do anything else              while waiting for the HTTP response. Use ExecuteAsync instead whenever possible! |
| [Execute\(string, CurlSettings\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)') |   <b>⚠️ SYNCHRONOUS with settings - Blocks thread with advanced options.</b> |
| [Execute\(string, CancellationToken\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)') |   <b>⚠️ SYNCHRONOUS with cancellation - Blocks thread but can be cancelled.</b>  Still BLOCKS your thread, but can be cancelled. Prefer ExecuteAsync with cancellation.\`\.\.\.\` |

<a name='CurlDotNet.Curl.Execute(string)'></a>

## Curl\.Execute\(string\) Method


<b>⚠️ SYNCHRONOUS - Execute curl command and WAIT for it to complete (BLOCKS thread).</b>

This method BLOCKS your thread until the HTTP request completes. Your application
             will FREEZE during this time. Only use when async is not possible.

<b>When to use SYNC (this method):</b>
- ⚠️ Console applications with simple flow
- ⚠️ Legacy code that can't use async
- ⚠️ Unit tests (sometimes)
- ⚠️ Quick scripts or tools
- ❌ NEVER in UI applications (will freeze)
- ❌ NEVER in web applications (reduces throughput)

<b>Example - When sync is OK:</b>

```csharp
// ✅ OK - Simple console app
static void Main()
{
    var result = Curl.Execute("curl https://api.example.com");
    Console.WriteLine(result.Body);
}

// ✅ OK - Unit test
[Test]
public void TestApi()
{
    var result = Curl.Execute("curl https://api.example.com");
    Assert.AreEqual(200, result.StatusCode);
}

// ❌ BAD - Will freeze UI!
private void Button_Click(object sender, EventArgs e)
{
    var result = Curl.Execute("curl https://api.example.com"); // FREEZES UI!
    textBox.Text = result.Body;
}
```

<b>⚠️ WARNING:</b> This blocks your thread. The application cannot do anything else
             while waiting for the HTTP response. Use ExecuteAsync instead whenever possible!

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to execute

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The result \(blocks until complete\)

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings)'></a>

## Curl\.Execute\(string, CurlSettings\) Method


<b>⚠️ SYNCHRONOUS with settings - Blocks thread with advanced options.</b>

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken)'></a>

## Curl\.Execute\(string, CancellationToken\) Method


<b>⚠️ SYNCHRONOUS with cancellation - Blocks thread but can be cancelled.</b>

Still BLOCKS your thread, but can be cancelled. Prefer ExecuteAsync with cancellation.

```csharp
// Blocks thread but can timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var result = Curl.Execute("curl https://api.example.com", cts.Token);
```

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')