# Tutorial 2: What is curl?

## 🎯 What You'll Learn

- What curl is and why it's everywhere
- How curl relates to everyday web browsing
- Why developers love curl
- How CurlDotNet brings curl to C#

## 📚 Prerequisites

- [Tutorial 1: What is .NET and C#?](01-what-is-dotnet.html)

## 🌍 The Big Picture

### What is curl?

**curl** (pronounced "curl" or "see-url") is a command-line tool that developers use to transfer data to and from servers. Think of it as a web browser without the graphics - it's the tool that powers billions of API calls every day.

### Real-World Analogy

Imagine you want to order a pizza:

- **Web Browser** = Going to the pizza shop, looking at their menu, placing your order, and seeing everything happen visually
- **curl** = Calling the pizza shop on the phone with exact instructions: "One large pepperoni, thin crust, deliver to 123 Main St"

curl is:
- Faster (no graphics to load)
- More precise (you say exactly what you want)
- Scriptable (you can automate it)
- Universal (works everywhere)

## 🔍 Why curl is Everywhere

### 1. Every API Documentation Uses curl

When you look at API documentation from GitHub, Stripe, Twitter, or any major service, you'll see curl commands:

```bash
# GitHub API
curl https://api.github.com/users/octocat

# Stripe API
curl https://api.stripe.com/v1/charges \
  -u sk_test_KEY: \
  -d amount=2000

# Twitter API
curl -X POST https://api.twitter.com/2/tweets \
  -H "Authorization: Bearer TOKEN"
```

### 2. Developers Use It Every Day

- Testing APIs while building them
- Debugging network issues
- Quick checks of website status
- Automating tasks
- CI/CD pipelines

### 3. It's on Every Computer

curl comes pre-installed on:
- All Linux systems
- All Mac computers
- Windows 10 and newer
- Most cloud servers
- Docker containers

## 📖 Understanding curl Commands

Let's break down a typical curl command:

```bash
curl -X POST https://api.example.com/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer token123" \
  -d '{"name":"Alice","email":"alice@example.com"}'
```

### Breaking It Down

| Part | What It Does |
|------|--------------|
| `curl` | The command name |
| `-X POST` | Use POST method (like submitting a form) |
| `https://api.example.com/users` | Where to send the request |
| `-H "..."` | Add a header (extra information) |
| `-d '...'` | The data to send |

### curl Options Explained

Here are the most common curl options you'll see:

| Option | What It Does | Example |
|--------|--------------|---------|
| `-X` | HTTP method | `-X POST`, `-X PUT` |
| `-H` | Add header | `-H "Authorization: Bearer token"` |
| `-d` | Send data | `-d '{"key":"value"}'` |
| `-o` | Save output to file | `-o result.json` |
| `-u` | Username/password | `-u user:pass` |
| `-L` | Follow redirects | `-L` |
| `-v` | Verbose (show details) | `-v` |
| `-k` | Skip SSL verification | `-k` (only for testing!) |
| `-i` | Include headers in output | `-i` |

## 💡 Common curl Examples

### Example 1: Get a Webpage

```bash
curl https://example.com
```

This fetches the HTML of example.com - like viewing the page source in your browser.

### Example 2: Get JSON Data

```bash
curl https://api.github.com/users/octocat
```

This gets user data in JSON format - exactly what mobile apps do behind the scenes.

### Example 3: Send Data to an API

```bash
curl -X POST https://httpbin.org/post \
  -H "Content-Type: application/json" \
  -d '{"message":"Hello World"}'
```

This sends data to a server - like submitting a form but faster and more precise.

### Example 4: Download a File

```bash
curl -o image.jpg https://example.com/image.jpg
```

This downloads a file - like right-clicking and "Save As" in a browser.

### Example 5: Authentication

```bash
curl -u username:password https://api.example.com/private
```

This authenticates with username and password - like logging into a website.

## 🎨 Why Developers Love curl

### 1. It's Fast

No waiting for browsers to load graphics, JavaScript, or CSS. Just get the data you need instantly.

### 2. It's Precise

You control exactly what gets sent:
- Which HTTP method
- Which headers
- What data
- How to handle responses

### 3. It's Reproducible

Copy a curl command, send it to a colleague, and they get the exact same result. No "works on my machine" problems.

### 4. It's Automatable

Put curl commands in scripts, run them on schedules, chain them together - endless possibilities.

### 5. It's Universal

The same curl command works on Mac, Windows, Linux, in Docker, in CI/CD, everywhere.

## 🚀 From curl to CurlDotNet

### The Problem

You find a perfect curl command in API documentation:

```bash
curl -X POST https://api.example.com/users \
  -H "Authorization: Bearer token123" \
  -d '{"name":"Alice"}'
```

But you're writing a C# application. Traditionally, you'd have to:

1. Understand what the curl command does
2. Translate it to C# HttpClient syntax
3. Handle headers, data, authentication separately
4. Test that your translation works the same way

This is time-consuming and error-prone.

### The Solution: CurlDotNet

With CurlDotNet, you paste the curl command directly into C#:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
      -H 'Authorization: Bearer token123' \
      -d '{""name"":""Alice""}'
");
```

**That's it!** No translation. No interpretation. Just paste and run.

## 🔧 How CurlDotNet Works

### Not a Wrapper

CurlDotNet is **not** a wrapper around the curl binary. It doesn't execute curl.exe behind the scenes.

### Pure .NET Implementation

CurlDotNet is a complete C# implementation that:
1. Parses curl command syntax
2. Translates curl options to HTTP operations
3. Executes requests using native .NET
4. Returns results in a C#-friendly format

### Same Behavior, Better Integration

You get:
- Exact same behavior as native curl
- Full async/await support
- Strong typing and IntelliSense
- Exception handling
- No external dependencies
- Cross-platform compatibility

## 📊 curl vs CurlDotNet Comparison

| Feature | curl (command-line) | CurlDotNet |
|---------|---------------------|------------|
| Parse responses | Manual | Automatic (JSON, XML) |
| Error handling | Exit codes | C# exceptions |
| Type safety | None | Full C# types |
| IntelliSense | None | Full support |
| Debugging | Limited | Full Visual Studio support |
| Integration | Shell scripts only | Full .NET integration |
| Async support | No | Yes (async/await) |
| Object mapping | Manual parsing | Automatic deserialization |

## 🎯 Real-World Scenarios

### Scenario 1: Testing an API

**With curl:**
```bash
curl -X POST https://api.example.com/test \
  -H "Content-Type: application/json" \
  -d '{"test":true}'
```

**With CurlDotNet in C#:**
```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/test \
      -H 'Content-Type: application/json' \
      -d '{""test"":true}'
");

// Now you can easily check the result
Assert.Equal(200, result.StatusCode);
var data = result.ParseJson<TestResponse>();
Assert.True(data.Success);
```

### Scenario 2: Building an Integration

**Found in API docs (Stripe example):**
```bash
curl https://api.stripe.com/v1/charges \
  -u sk_test_KEY: \
  -d amount=2000 \
  -d currency=usd
```

**Use directly in your C# app:**
```csharp
var charge = await Curl.ExecuteAsync($@"
    curl https://api.stripe.com/v1/charges \
      -u {apiKey}: \
      -d amount={amount} \
      -d currency={currency}
");

if (charge.IsSuccess)
{
    var chargeData = charge.ParseJson<StripeCharge>();
    // Process the charge...
}
```

### Scenario 3: Migrating Bash Scripts

**Old bash script:**
```bash
#!/bin/bash
for user in $(cat users.txt); do
  curl -X POST https://api.example.com/notify \
    -d "user=$user"
done
```

**New C# version:**
```csharp
var users = await File.ReadAllLinesAsync("users.txt");

foreach (var user in users)
{
    await Curl.ExecuteAsync($@"
        curl -X POST https://api.example.com/notify \
          -d 'user={user}'
    ");
}
```

## 🔍 Understanding curl Options in Depth

### HTTP Methods

```bash
curl -X GET https://api.example.com/users      # Read data
curl -X POST https://api.example.com/users     # Create new
curl -X PUT https://api.example.com/users/1    # Update entire
curl -X PATCH https://api.example.com/users/1  # Update partial
curl -X DELETE https://api.example.com/users/1 # Delete
```

### Headers

Headers are like the envelope of a letter - they contain metadata about your request:

```bash
# Tell server what format you want back
curl -H "Accept: application/json" https://api.example.com

# Tell server what format you're sending
curl -H "Content-Type: application/json" \
  -d '{"key":"value"}' https://api.example.com

# Authentication
curl -H "Authorization: Bearer token123" https://api.example.com

# Custom headers
curl -H "X-API-Key: key123" \
  -H "X-Request-ID: unique-id" \
  https://api.example.com
```

### Data Sending

```bash
# JSON data
curl -d '{"name":"Alice"}' https://api.example.com

# Form data
curl -d "name=Alice&age=30" https://api.example.com

# File upload
curl -F "file=@document.pdf" https://api.example.com

# Data from file
curl -d @data.json https://api.example.com
```

## 💻 Try It Yourself

### Exercise 1: Understand This curl Command

Look at this command and identify each part:

```bash
curl -X POST https://api.example.com/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer abc123" \
  -d '{"name":"John","email":"john@example.com"}'
```

<details>
<summary>Click for Answer</summary>

- `-X POST` = Use POST method (creating something new)
- URL = Send to https://api.example.com/users
- First `-H` = Tell server we're sending JSON
- Second `-H` = Provide authentication token
- `-d` = Send user data (name and email)

</details>

### Exercise 2: Translate to CurlDotNet

Convert the above curl command to CurlDotNet C# code.

<details>
<summary>Click for Answer</summary>

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer abc123' \
      -d '{""name"":""John"",""email"":""john@example.com""}'
");
```

Note: Change double quotes inside the string to `""` to escape them in C#.

</details>

### Exercise 3: Real API Test

Try this with a real API that's made for testing:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        // httpbin.org is a free API for testing
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
              -H 'Content-Type: application/json' \
              -d '{""message"":""Hello from CurlDotNet!""}'
        ");

        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine($"Response: {result.Body}");
    }
}
```

## ❌ Common Mistakes

### Mistake 1: Forgetting to Escape Quotes

```csharp
// Wrong - will cause syntax error
var result = await Curl.ExecuteAsync("curl -d '{"name":"Alice"}'");

// Right - escape inner quotes
var result = await Curl.ExecuteAsync(@"curl -d '{""name"":""Alice""}'");
```

### Mistake 2: Using curl Binary Instead of CurlDotNet

```csharp
// Wrong - this runs curl.exe if installed
Process.Start("curl", "https://api.example.com");

// Right - uses CurlDotNet
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

### Mistake 3: Forgetting async/await

```csharp
// Wrong - missing await
var result = Curl.ExecuteAsync("curl https://api.example.com");

// Right - with await
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

## 📚 curl Resources

Want to learn more about curl itself?

- [Official curl Website](https://curl.se/) - The source of all curl knowledge
- [curl Documentation](https://curl.se/docs/) - Complete reference
- [curl Tutorial](https://curl.se/docs/tutorial.html) - Learn curl basics
- [curl Man Page](https://curl.se/docs/manpage.html) - All options explained

For CurlDotNet-specific information, see our [troubleshooting guide](../troubleshooting/common-issues.html) for links to documentation with detailed error explanations.

## 🎓 Key Takeaways

- curl is a command-line tool for transferring data
- It's used in billions of API calls every day
- Every API documentation includes curl examples
- curl is fast, precise, reproducible, and universal
- CurlDotNet lets you use curl commands directly in C#
- No translation needed - paste and run
- Get all the benefits of curl with C# integration

## 🚀 Next Steps

Now that you understand curl:

1. **Next Tutorial** → [Understanding Async/Await](03-what-is-async.html)
2. **Try curl yourself** - Open a terminal and run `curl https://api.github.com`
3. **Look at API docs** - Notice how they all use curl
4. **Experiment** - Try different curl options

## 🤔 Questions You Might Have

**Q: Do I need to install curl to use CurlDotNet?**
A: No! CurlDotNet is a pure .NET implementation. You don't need curl installed.

**Q: Can I use curl commands I find online directly?**
A: Yes! That's the whole point of CurlDotNet. Copy, paste, and run.

**Q: What if I don't know curl syntax?**
A: That's okay! CurlDotNet also has a fluent API for building requests programmatically. But knowing curl is useful since most API docs use it.

**Q: Is CurlDotNet as powerful as native curl?**
A: Yes! CurlDotNet supports 300+ curl options and aims for complete compatibility.

**Q: Why not just use HttpClient?**
A: You can! But CurlDotNet is easier when you have curl commands from API documentation. No translation needed.

## 📚 Summary

curl is a universal tool that developers use to interact with web services. CurlDotNet brings curl's simplicity and universality to C#, letting you paste curl commands directly into your code without translation. This eliminates the friction between reading API documentation and implementing it in your applications.

---

**Ready for the next tutorial?** → [Understanding Async/Await](03-what-is-async.html)

**Need help?** Check the [Troubleshooting Guide](../troubleshooting/common-issues.html) or ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
