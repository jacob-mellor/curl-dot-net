#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](index.md#CurlDotNet.Middleware 'CurlDotNet\.Middleware')

## CurlContext Class

Context object containing information about the curl request\.

```csharp
public class CurlContext
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlContext
### Properties

<a name='CurlDotNet.Middleware.CurlContext.CancellationToken'></a>

## CurlContext\.CancellationToken Property

The cancellation token for the request\.

```csharp
public System.Threading.CancellationToken CancellationToken { get; set; }
```

#### Property Value
[System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

<a name='CurlDotNet.Middleware.CurlContext.Command'></a>

## CurlContext\.Command Property

The curl command being executed\.

```csharp
public string Command { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Middleware.CurlContext.Options'></a>

## CurlContext\.Options Property

The parsed curl options\.

```csharp
public CurlDotNet.Core.CurlOptions Options { get; set; }
```

#### Property Value
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

<a name='CurlDotNet.Middleware.CurlContext.Properties'></a>

## CurlContext\.Properties Property

Custom properties for passing data between middleware\.

```csharp
public System.Collections.Generic.Dictionary<string,object> Properties { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='CurlDotNet.Middleware.CurlContext.StartTime'></a>

## CurlContext\.StartTime Property

Request start time for timing\.

```csharp
public System.DateTime StartTime { get; set; }
```

#### Property Value
[System\.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime 'System\.DateTime')
### Methods

<a name='CurlDotNet.Middleware.CurlContext.GetProperty_T_(string)'></a>

## CurlContext\.GetProperty\<T\>\(string\) Method

Get a property from the context\.

```csharp
public T GetProperty<T>(string key);
```
#### Type parameters

<a name='CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).T'></a>

`T`
#### Parameters

<a name='CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[T](CurlDotNet.Middleware.CurlContext.md#CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).T 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)\.T')

<a name='CurlDotNet.Middleware.CurlContext.WithProperty(string,object)'></a>

## CurlContext\.WithProperty\(string, object\) Method

Add a property to the context \(fluent\)\.

```csharp
public CurlDotNet.Middleware.CurlContext WithProperty(string key, object value);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlContext.WithProperty(string,object).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Middleware.CurlContext.WithProperty(string,object).value'></a>

`value` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

#### Returns
[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')