# Tutorial 1: What is .NET and C#?

## 🎯 What You'll Learn

- What .NET is (in simple terms)
- What C# is and how it relates to .NET
- How to write your first C# program
- How CurlDotNet fits into the picture

## 📚 Prerequisites

None! This is the beginning.

## 🌍 The Big Picture

Let's use a simple analogy:

- **.NET** is like a smartphone operating system (iOS or Android)
- **C#** is like the language you use to write apps
- **CurlDotNet** is like an app that helps you access the internet

### Breaking It Down

#### What is .NET?

.NET (pronounced "dot net") is a free platform made by Microsoft for building applications. Think of it as a foundation that:

- Runs your programs
- Provides built-in tools (like internet access, file handling)
- Works on Windows, Mac, and Linux
- Is completely free to use

#### What is C#?

C# (pronounced "see sharp") is a programming language. It's how you tell the computer what to do. It looks like this:

```csharp
// This is C# code
var message = "Hello, World!";
Console.WriteLine(message);
```

#### What is CurlDotNet?

CurlDotNet is a tool (called a "library" or "package") that makes it easy to:
- Download web pages
- Call APIs
- Upload files
- Interact with web services

## 💻 Your First C# Program

Let's write a simple program that uses CurlDotNet:

```csharp
using System;                // This gives us basic C# features
using System.Threading.Tasks; // This lets us use async/await
using CurlDotNet;            // This is our library!

namespace MyFirstProgram     // This is like a folder for our code
{
    class Program            // A "class" is a container for our code
    {
        static async Task Main(string[] args)  // This is where the program starts
        {
            // Let's fetch Google's homepage
            Console.WriteLine("Fetching Google...");

            var result = await Curl.ExecuteAsync("curl https://www.google.com");

            if (result.IsSuccess)
            {
                Console.WriteLine("Success! Got a response.");
                Console.WriteLine($"The page is {result.Body.Length} characters long.");
            }
            else
            {
                Console.WriteLine("Something went wrong!");
            }
        }
    }
}
```

### What's Happening Here?

1. **using statements** - Tell C# what tools we want to use
2. **namespace** - Organizes our code (like a folder)
3. **class** - A container for our code
4. **Main method** - Where the program starts running
5. **Curl.ExecuteAsync** - Fetches a webpage
6. **Console.WriteLine** - Prints to the screen

## 🏗 Setting Up Your Environment

### Step 1: Install .NET

1. Go to [dotnet.microsoft.com](https://dotnet.microsoft.com)
2. Click "Download"
3. Install it (just click Next, Next, Finish)

### Step 2: Install Visual Studio Code

1. Go to [code.visualstudio.com](https://code.visualstudio.com)
2. Download and install
3. Open it

### Step 3: Create Your First Project

Open a terminal/command prompt and type:

```bash
# Create a new folder
mkdir MyFirstCurlApp
cd MyFirstCurlApp

# Create a new C# project
dotnet new console

# Add CurlDotNet
dotnet add package CurlDotNet
```

### Step 4: Run Your Program

```bash
dotnet run
```

## 🎨 Understanding the Code Structure

Every C# program has this basic structure:

```csharp
using [Tools You Need];       // Step 1: Import tools

namespace [YourProgramName]   // Step 2: Name your program
{
    class Program              // Step 3: Create a container
    {
        static async Task Main()  // Step 4: Starting point
        {
            // Your code goes here!
        }
    }
}
```

## 🔧 C# Basics You'll See Often

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
    // Do this if successful
    Console.WriteLine("It worked!");
}
else
{
    // Do this if it failed
    Console.WriteLine("It didn't work.");
}
```

### Methods (Reusable Code)

```csharp
// A method that fetches a webpage
async Task<string> FetchWebpage(string url)
{
    var result = await Curl.ExecuteAsync($"curl {url}");
    return result.Body;
}

// Using the method
var content = await FetchWebpage("https://example.com");
```

## 🎯 Try It Yourself

### Exercise 1: Modify the Message

Change this code to print your name:

```csharp
Console.WriteLine("Fetching Google...");
// Change to: Console.WriteLine("YourName is fetching Google...");
```

### Exercise 2: Fetch a Different Website

Change the URL to fetch a different site:

```csharp
var result = await Curl.ExecuteAsync("curl https://www.github.com");
```

### Exercise 3: Print More Information

Add these lines to see more details:

```csharp
Console.WriteLine($"Status Code: {result.StatusCode}");
Console.WriteLine($"Has Headers: {result.Headers.Count > 0}");
```

## ❌ Common Mistakes

### Mistake 1: Forgetting 'await'

```csharp
// Wrong - missing await
var result = Curl.ExecuteAsync("curl https://google.com");

// Right - with await
var result = await Curl.ExecuteAsync("curl https://google.com");
```

### Mistake 2: Wrong Quotes

```csharp
// Wrong - using wrong quotes
var result = await Curl.ExecuteAsync("curl https://google.com");

// Right - using straight quotes
var result = await Curl.ExecuteAsync("curl https://google.com");
```

### Mistake 3: Missing Semicolon

```csharp
// Wrong - no semicolon
Console.WriteLine("Hello")

// Right - with semicolon
Console.WriteLine("Hello");
```

## 📊 Quick Reference

| Term | What It Means | Example |
|------|---------------|---------|
| .NET | The platform that runs your code | Like Windows or macOS |
| C# | The language you write in | Like English or Spanish |
| CurlDotNet | A tool for web requests | Like a web browser |
| `using` | Import tools | `using CurlDotNet;` |
| `var` | Create a variable | `var name = "Alice";` |
| `async/await` | Handle slow operations | `await Curl.ExecuteAsync()` |
| `Console.WriteLine` | Print to screen | `Console.WriteLine("Hi!");` |

## 🚀 Next Steps

Now that you understand the basics:

1. **Next Tutorial** → [What is curl?](02-what-is-curl.md)
2. **Try changing the example** - Make it fetch your favorite website
3. **Experiment** - What happens if you change different parts?

## 🤔 Questions You Might Have

**Q: Do I need to pay for .NET or C#?**
A: No! Everything is completely free.

**Q: Will this work on Mac/Linux?**
A: Yes! .NET works on Windows, Mac, and Linux.

**Q: Is C# hard to learn?**
A: It's one of the easier languages to learn, especially with good tutorials!

**Q: What can I build with this?**
A: Websites, desktop apps, mobile apps, games, APIs, and more!

## 📚 Summary

- **.NET** is the platform (foundation)
- **C#** is the language (how you write)
- **CurlDotNet** is a tool (for web requests)
- Programs start in the `Main` method
- Use `await` with async operations
- Everything is free to use!

---

**Ready for the next tutorial?** → [What is curl?](02-what-is-curl.md)

**Need help?** Check the [Troubleshooting Guide](../troubleshooting/common-issues.md) or ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)