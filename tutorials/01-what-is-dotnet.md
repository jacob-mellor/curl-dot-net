---
title: "What is .NET and C#? Beginner Tutorial for CurlDotNet"
description: "What .NET and C# are, how to install the .NET SDK, and how to run your first C# program that makes a web request with CurlDotNet."
---

# Tutorial 1: What is .NET and C#?

What is .NET, what is C#, and how do you write your first C# program? This tutorial answers those questions for complete beginners and shows where CurlDotNet (a pure C# library that runs curl commands) fits in.

## What You'll Learn

- What .NET is (in simple terms)
- What C# is and how it relates to .NET
- How to write your first C# program
- How CurlDotNet fits into the picture

## Prerequisites

None. This is the beginning.

## The Big Picture

A simple analogy:

- **.NET** is like a smartphone operating system (iOS or Android)
- **C#** is like the language you use to write apps
- **CurlDotNet** is like an app that helps you access the internet

### What is .NET?

.NET (pronounced "dot net") is a free platform made by Microsoft for building applications. It:

- Runs your programs
- Provides built-in tools (internet access, file handling, and more)
- Works on Windows, Mac, and Linux
- Is free to use

### What is C#?

C# (pronounced "see sharp") is a programming language. It is how you tell the computer what to do. It looks like this:

```csharp
// This is C# code
var message = "Hello, World!";
Console.WriteLine(message);
```

### What is CurlDotNet?

CurlDotNet is a library (a "package") that makes it easy to:

- Download web pages
- Call APIs
- Upload files
- Interact with web services

You give it a curl command as a string and it runs the request in pure .NET.

## Your First C# Program

A small program that uses CurlDotNet to fetch a web page:

```csharp
using System;                 // Basic C# features
using System.Threading.Tasks; // Lets us use async/await
using CurlDotNet;             // The CurlDotNet library

class Program
{
    static async Task Main()  // The program starts here
    {
        var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

        Console.WriteLine(result.IsSuccess ? "Success!" : "Something went wrong");
        Console.WriteLine($"The page is {result.Body.Length} characters long.");
    }
}
```

### What's Happening Here?

1. **using statements** tell C# which tools we want to use
2. **class** is a container for our code
3. **Main method** is where the program starts running
4. **Curl.ExecuteAsync** runs the curl command and fetches the page
5. **result.IsSuccess** is `true` when the server answered with a 2xx status
6. **Console.WriteLine** prints to the screen

## Setting Up Your Environment

### Step 1: Install .NET

1. Go to [dotnet.microsoft.com](https://dotnet.microsoft.com)
2. Click "Download" and install the .NET SDK
3. Accept the defaults in the installer

### Step 2: Install Visual Studio Code

1. Go to [code.visualstudio.com](https://code.visualstudio.com)
2. Download and install
3. Install the "C# Dev Kit" extension when prompted

### Step 3: Create Your First Project

Open a terminal and type:

```bash
# Create a new folder
mkdir MyFirstCurlApp
cd MyFirstCurlApp

# Create a new C# console project
dotnet new console

# Add CurlDotNet from NuGet
dotnet add package CurlDotNet
```

Replace the contents of `Program.cs` with the program above.

### Step 4: Run Your Program

```bash
dotnet run
```

For more detail see the [installation guide](../getting-started/installation.html).

## Understanding the Code Structure

Every C# console program has this basic shape:

```csharp
using [Tools You Need];       // Step 1: Import tools

class Program                 // Step 2: Create a container
{
    static async Task Main()  // Step 3: Starting point
    {
        // Your code goes here
    }
}
```

## C# Basics You'll See Often

### Variables (Storing Information)

```csharp
// Storing text
string name = "Alice";
var message = "Hello!";  // 'var' figures out the type automatically

// Storing numbers
int age = 25;
var price = 19.99;

// Storing yes/no values
bool isLoggedIn = true;
var isAdmin = false;
```

### If Statements (Making Decisions)

```csharp
if (result.IsSuccess)
{
    Console.WriteLine("It worked!");
}
else
{
    Console.WriteLine($"It didn't work. Status: {result.StatusCode}");
}
```

### Methods (Reusable Code)

```csharp
// A method that fetches a web page and returns its text
static async Task<string> FetchPageAsync(string url)
{
    var result = await Curl.ExecuteAsync($"curl {url}");
    return result.Body;
}

// Using the method
var content = await FetchPageAsync("https://httpbin.org/html");
```

## Try It Yourself

### Exercise 1: Print Your Name

Add a line before the request:

```csharp
Console.WriteLine("Alice is fetching a web page...");
```

### Exercise 2: Fetch a Different Website

Change the URL to fetch a different site:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

### Exercise 3: Print More Information

Add these lines to see more details:

```csharp
Console.WriteLine($"Status Code: {result.StatusCode}");
Console.WriteLine($"Header count: {result.Headers.Count}");
```

## Common Mistakes

### Mistake 1: Forgetting 'await'

```csharp
// Wrong - missing await, so 'result' is a Task, not a CurlResult
var result = Curl.ExecuteAsync("curl https://httpbin.org/get");

// Right - with await
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

### Mistake 2: Curly Quotes

Text copied from a web page or word processor sometimes contains curly quotes, which C# does not accept.

```csharp
// Wrong - curly quotes copied from a document
var result = await Curl.ExecuteAsync(“curl https://httpbin.org/get”);

// Right - straight double quotes
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

### Mistake 3: Missing Semicolon

```csharp
// Wrong - no semicolon
Console.WriteLine("Hello")

// Right - with semicolon
Console.WriteLine("Hello");
```

## Quick Reference

| Term | What It Means | Example |
|------|---------------|---------|
| .NET | The platform that runs your code | Like Windows or macOS |
| C# | The language you write in | Like English or Spanish |
| CurlDotNet | A library for web requests | `Curl.ExecuteAsync("curl https://...")` |
| `using` | Import tools | `using CurlDotNet;` |
| `var` | Create a variable | `var name = "Alice";` |
| `async/await` | Handle slow operations | `await Curl.ExecuteAsync(...)` |
| `Console.WriteLine` | Print to screen | `Console.WriteLine("Hi!");` |

## Questions You Might Have

**Q: Do I need to pay for .NET or C#?**
A: No. The .NET SDK, C#, and CurlDotNet are all free.

**Q: Will this work on Mac/Linux?**
A: Yes. .NET and CurlDotNet run on Windows, Mac, and Linux. See [framework compatibility](../framework-compatibility.html).

**Q: Is C# hard to learn?**
A: It is one of the more approachable languages, and these tutorials assume no prior experience.

**Q: What can I build with this?**
A: Websites, desktop apps, mobile apps, games, APIs, command-line tools, and more.

## Summary

- **.NET** is the platform (foundation)
- **C#** is the language (how you write)
- **CurlDotNet** is a library (for web requests)
- Programs start in the `Main` method
- Use `await` with async operations

## Next Steps

1. **Next tutorial:** [What is curl?](02-what-is-curl.html)
2. **Try changing the example** so it fetches a different site
3. **Experiment** with the pieces of the program and see what changes

**Need help?** Check the [troubleshooting guide](../troubleshooting/common-issues.html) or ask in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions).
