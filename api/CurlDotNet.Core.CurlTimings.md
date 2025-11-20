#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](index.md#CurlDotNet.Core 'CurlDotNet\.Core')

## CurlTimings Class


<b>Detailed timing breakdown of the curl operation.</b>

See where time was spent (like curl -w):

```csharp
if (result.Timings.Total > 2000)
{
    Console.WriteLine("Slow request! Let's see why:");
    Console.WriteLine($"DNS: {result.Timings.NameLookup}ms");
    Console.WriteLine($"Connect: {result.Timings.Connect}ms");
    Console.WriteLine($"SSL: {result.Timings.AppConnect}ms");
    Console.WriteLine($"Wait: {result.Timings.StartTransfer}ms");
}
```

```csharp
public class CurlTimings
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlTimings
### Properties

<a name='CurlDotNet.Core.CurlTimings.AppConnect'></a>

## CurlTimings\.AppConnect Property

SSL/TLS handshake time in milliseconds

```csharp
public double AppConnect { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='CurlDotNet.Core.CurlTimings.Connect'></a>

## CurlTimings\.Connect Property

TCP connection time in milliseconds

```csharp
public double Connect { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='CurlDotNet.Core.CurlTimings.NameLookup'></a>

## CurlTimings\.NameLookup Property

DNS resolution time in milliseconds

```csharp
public double NameLookup { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='CurlDotNet.Core.CurlTimings.PreTransfer'></a>

## CurlTimings\.PreTransfer Property

Time until request was sent in milliseconds

```csharp
public double PreTransfer { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='CurlDotNet.Core.CurlTimings.Redirect'></a>

## CurlTimings\.Redirect Property

Time spent on redirects in milliseconds

```csharp
public double Redirect { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='CurlDotNet.Core.CurlTimings.StartTransfer'></a>

## CurlTimings\.StartTransfer Property

Time until first byte received in milliseconds

```csharp
public double StartTransfer { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='CurlDotNet.Core.CurlTimings.Total'></a>

## CurlTimings\.Total Property

Total time in milliseconds

```csharp
public double Total { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')