---
title: "Async/Await in C# Explained for Beginners (CurlDotNet)"
description: "What async and await mean in C#, why Curl.ExecuteAsync must be awaited, and how to run several curl requests at once with Task.WhenAll."
---

# Tutorial 3: Understanding Async/Await (In Plain English)

What do `async` and `await` mean in C#, and why does every CurlDotNet request look like `await Curl.ExecuteAsync(...)`? This tutorial explains asynchronous programming with everyday analogies, shows the rules you need to follow, and covers the mistakes beginners make most often.

## What You'll Learn

- What `async` and `await` mean
- Why we use them
- How to use them correctly
- Common mistakes to avoid

## Prerequisites

- [Tutorial 1: What is .NET and C#?](01-what-is-dotnet.html)

## The Restaurant Analogy

Imagine you are at a restaurant. There are two ways to get your food:

### Method 1: Stand and Wait (Synchronous)

```text
1. You order a burger
2. You stand at the counter
3. You can't do anything else
4. You just wait... and wait...
5. Finally you get your burger
6. Now you can do the next thing
```

### Method 2: Take a Number (Asynchronous)

```text
1. You order a burger
2. They give you a number
3. You sit down and chat with friends
4. You check your phone
5. They call your number
6. You get your burger
```

**Async/await is Method 2.** Your program does not freeze while it waits.

## In Code Terms

### Without Async (Program Freezes)

```csharp
// Avoid this for web requests - the whole program stops while it waits
static void Main()
{
    Console.WriteLine("Starting download...");

    var result = FetchPage("https://httpbin.org/delay/3");  // blocks for 3 seconds

    Console.WriteLine("Finally done!");
}
```

### With Async (Program Stays Responsive)

```csharp
static async Task Main()  // Note: async Task
{
    Console.WriteLine("Starting download...");

    var result = await FetchPageAsync("https://httpbin.org/delay/3");  // does not block the thread

    Console.WriteLine("Done!");
}
```

## The Golden Rules

### Rule 1: Async Methods End with "Async"

```csharp
// Synchronous versions (avoid these for web requests)
Curl.Execute("curl https://httpbin.org/get");
File.ReadAllText("data.txt");

// Asynchronous versions (use these)
await Curl.ExecuteAsync("curl https://httpbin.org/get");
await File.ReadAllTextAsync("data.txt");
```

### Rule 2: Always Use 'await' with Async Methods

```csharp
// WRONG - missing await; result is a Task, not a CurlResult
var result = Curl.ExecuteAsync("curl https://httpbin.org/get");

// RIGHT - with await
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

### Rule 3: Methods Using 'await' Must Be 'async'

```csharp
// WRONG - method not marked async, so 'await' is a compile error
static Task<string> GetData()
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
    return result.Body;
}

// RIGHT - method marked async
static async Task<string> GetData()
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
    return result.Body;
}
```

## Real Examples with CurlDotNet

### Example 1: Simple Async Call

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()  // async Task for Main
    {
        var result = await Curl.ExecuteAsync("curl https://api.github.com");
        Console.WriteLine($"Got {result.Body.Length} characters");
    }
}
```

### Example 2: Multiple Async Calls (One at a Time)

```csharp
static async Task Main()
{
    // These happen one after another
    var result1 = await Curl.ExecuteAsync("curl https://api.github.com");
    Console.WriteLine("Got first response");

    var result2 = await Curl.ExecuteAsync("curl https://httpbin.org/get");
    Console.WriteLine("Got second response");
}
```

### Example 3: Multiple Async Calls (All at Once)

```csharp
static async Task Main()
{
    // Start both at the same time
    var task1 = Curl.ExecuteAsync("curl https://api.github.com");
    var task2 = Curl.ExecuteAsync("curl https://httpbin.org/get");

    // Wait for both to complete
    var results = await Task.WhenAll(task1, task2);

    Console.WriteLine($"Got {results.Length} responses");
}
```

CurlDotNet also has a shortcut for this: `await Curl.ExecuteManyAsync("curl https://api.github.com", "curl https://httpbin.org/get")` returns a `CurlResult[]`. See [Tutorial 13: Parallel Requests](13-parallel-requests.html).

## What's Really Happening?

When you use `await`:

1. **Request starts** - the HTTP request begins
2. **Control returns** - your program can do other things
3. **Request completes** - the server responds
4. **Code resumes** - your code continues from the `await` line

Like ordering pizza:

1. You call and order (start request)
2. You watch TV while waiting (program does other things)
3. The doorbell rings (request completes)
4. You eat pizza (code continues)

## Async vs Sync Comparison

### Synchronous (Blocking)

```csharp
// This freezes your program for the whole duration
public void FetchFiles()
{
    var file1 = FetchFile("file1.zip");  // wait 5 seconds...
    var file2 = FetchFile("file2.zip");  // wait 5 seconds...
    var file3 = FetchFile("file3.zip");  // wait 5 seconds...
    // Total time: 15 seconds, program frozen the whole time
}
```

### Asynchronous (Non-Blocking)

```csharp
// This does not freeze, and the downloads overlap
public async Task FetchFilesAsync()
{
    var task1 = Curl.DownloadAsync("https://httpbin.org/bytes/1024", "file1.bin");
    var task2 = Curl.DownloadAsync("https://httpbin.org/bytes/1024", "file2.bin");
    var task3 = Curl.DownloadAsync("https://httpbin.org/bytes/1024", "file3.bin");

    await Task.WhenAll(task1, task2, task3);
    // Total time: roughly the slowest single download
}
```

## Common Mistakes

### Mistake 1: Forgetting await

```csharp
// WRONG - this does not wait for the result
static async Task Main()
{
    var resultTask = Curl.ExecuteAsync("curl https://httpbin.org/get");
    Console.WriteLine(resultTask.Body);  // compile error: Task has no Body
}

// RIGHT - use await to get the actual result
static async Task Main()
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
    Console.WriteLine(result.Body);
}
```

### Mistake 2: Using .Result or .Wait()

```csharp
// WRONG - can deadlock in UI and ASP.NET apps
static void Main()
{
    var result = Curl.ExecuteAsync("curl https://httpbin.org/get").Result;
}

// RIGHT - use async/await all the way
static async Task Main()
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
}
```

If you really need a synchronous call (a console script, for example), use the synchronous API instead: `var result = Curl.Execute("curl https://httpbin.org/get");`.

### Mistake 3: Not Making the Method Async

```csharp
// WRONG - method not async
static Task<string> GetData()
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");  // error
    return result.Body;
}

// RIGHT - method is async
static async Task<string> GetData()
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
    return result.Body;
}
```

## Interactive Example

Try this and compare the two timings. `https://httpbin.org/delay/1` waits one second before responding.

```csharp
using System.Diagnostics;
using CurlDotNet;

var stopwatch = Stopwatch.StartNew();
await Curl.ExecuteAsync("curl https://httpbin.org/delay/1");
await Curl.ExecuteAsync("curl https://httpbin.org/delay/1");
Console.WriteLine($"Sequential: {stopwatch.ElapsedMilliseconds}ms (about 2 seconds)");

stopwatch.Restart();
await Task.WhenAll(
    Curl.ExecuteAsync("curl https://httpbin.org/delay/1"),
    Curl.ExecuteAsync("curl https://httpbin.org/delay/1"));
Console.WriteLine($"Parallel: {stopwatch.ElapsedMilliseconds}ms (about 1 second)");
```

## Quick Reference

| Keyword | What It Does | When to Use |
|---------|--------------|-------------|
| `async` | Marks a method as asynchronous | When the method contains `await` |
| `await` | Waits for an async operation | Before any `...Async()` method |
| `Task` | Represents an ongoing operation | Return type for async methods |
| `Task<T>` | Ongoing operation with a result | When the async method returns a value |

## Simple Rules to Remember

1. **See `Async` in the method name?** Use `await`
2. **Using `await` in a method?** Mark the method `async`
3. **Method is `async`?** Return `Task` or `Task<T>`
4. **Multiple operations?** Consider `Task.WhenAll()` or `Curl.ExecuteManyAsync`

## FAQs

**Q: Why not just use synchronous methods?**
A: Your application would freeze during network requests. For a quick script that is acceptable (`Curl.Execute` exists for that), but for apps and services use `await`.

**Q: Can I use async without await?**
A: Technically yes, but you lose the benefit and the compiler warns you.

**Q: Is async/await faster?**
A: A single request takes the same time. The win is that your program stays responsive and can overlap several requests.

**Q: Do I always need async/await?**
A: For network requests, file I/O, and database calls, yes. For simple calculations, no.

## Summary

- **Async** = non-blocking (does not freeze)
- **Await** = wait for the result without blocking the thread
- **Always use together** = `await` + `...Async` methods
- **Mark methods** = `async Task` when using `await`
- **Better user experience** = responsive applications

## Next Steps

1. **Next tutorial:** [Your First Request](04-your-first-request.html)
2. **Practice:** try the interactive example above
3. **Experiment:** what happens without `await`?

**Still confused?** That is normal. Try the examples and it will click. Check [common issues](../troubleshooting/common-issues.html) if you get stuck.
