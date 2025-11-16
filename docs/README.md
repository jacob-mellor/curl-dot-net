# 📚 CurlDotNet Documentation Hub

Welcome to CurlDotNet! Whether you're new to programming or an experienced developer, we've got you covered.

## 🚀 Quick Start Paths

### 🆕 Complete Beginner?
**"I'm new to .NET and programming"**
1. Start here → [What is .NET and C#?](tutorials/01-what-is-dotnet.md)
2. Then → [Your First Request](tutorials/04-your-first-request.md)
3. Next → [Understanding Results](tutorials/05-understanding-results.md)

### 💼 Experienced Developer?
**"I know .NET, just show me how to use CurlDotNet"**
1. Jump to → [Installation](getting-started/installation.md)
2. Check out → [API Guide](api-guide/README.md)
3. Browse → [Cookbook](cookbook/README.md)

### 🔄 Migrating from Another Library?
**"I'm switching from HttpClient/RestSharp/etc."**
- See → [Migration Guides](migration/README.md)
- [From HttpClient](migration/from-httpclient.md)
- [From RestSharp](migration/from-restsharp.md)

## 📖 Documentation Structure

### [🎓 Tutorials](tutorials/README.md)
**Learn the basics in plain English**
- No prior .NET knowledge required
- Step-by-step explanations
- Lots of analogies and examples
- [Start Tutorial Series →](tutorials/README.md)

### [👨‍🍳 Cookbook](cookbook/README.md)
**"How do I..." recipes**
- Task-focused solutions
- Copy-paste ready code
- Real-world scenarios
- [Browse Recipes →](cookbook/README.md)

### [📘 API Guide](api-guide/README.md)
**Complete API reference with examples**
- Every class and method documented
- Multiple examples per feature
- Best practices included
- [Explore API →](api-guide/README.md)

### [💡 Examples](examples/README.md)
**Learn by doing**
- Basic to advanced examples
- Integration with popular services
- Complete working applications
- [View Examples →](examples/README.md)

## 🎯 Common Tasks

### Basic Operations
- [Make a GET request](cookbook/beginner/simple-get.md)
- [POST JSON data](cookbook/beginner/send-json.md)
- [Download a file](cookbook/beginner/download-file.md)
- [Upload a file](cookbook/beginner/upload-file.md)
- [Handle errors](cookbook/beginner/handle-errors.md)

### Authentication
- [Basic authentication](authentication/basic-auth.md)
- [Bearer tokens](authentication/bearer-tokens.md)
- [API keys](authentication/api-keys.md)
- [OAuth flow](cookbook/real-world/oauth-flow.md)

### Working with APIs
- [Call a REST API](cookbook/beginner/call-api.md)
- [Parse JSON responses](tutorials/07-json-for-beginners.md)
- [Handle pagination](cookbook/patterns/pagination.md)
- [Retry failed requests](cookbook/intermediate/retry-logic.md)

### Advanced Features
- [Use middleware](advanced/middleware/README.md)
- [Parallel requests](tutorials/13-parallel-requests.md)
- [Progress tracking](cookbook/intermediate/progress-tracking.md)
- [Custom HTTP clients](advanced/custom-http-client.md)

## 🔍 Quick Reference

### The Three Ways to Use CurlDotNet

#### 1. String API (Simplest)
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

#### 2. Builder API (Type-Safe)
```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

#### 3. LibCurl API (Reusable)
```csharp
var curl = new LibCurl()
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(30));

var result = await curl.GetAsync("https://api.example.com");
```

## 📚 Learning Paths

### Path 1: Absolute Beginner
1. [What is .NET?](tutorials/01-what-is-dotnet.md)
2. [What is curl?](tutorials/02-what-is-curl.md)
3. [Understanding async/await](tutorials/03-what-is-async.md)
4. [Your first request](tutorials/04-your-first-request.md)
5. [Understanding results](tutorials/05-understanding-results.md)
6. [Handling errors](tutorials/06-handling-errors.md)

### Path 2: Web Developer
1. [Installation](getting-started/installation.md)
2. [HTTP methods](http-methods/README.md)
3. [Working with JSON](tutorials/07-json-for-beginners.md)
4. [Authentication](authentication/README.md)
5. [Building API clients](cookbook/intermediate/api-client-class.md)

### Path 3: Enterprise Developer
1. [Architecture overview](advanced/README.md)
2. [Middleware system](advanced/middleware/README.md)
3. [Error handling strategies](error-handling/error-handling-patterns.md)
4. [Performance optimization](advanced/performance.md)
5. [Testing strategies](advanced/testing.md)

## 🆘 Getting Help

### Can't Find Something?
- Use the [search](#) feature
- Check [Troubleshooting](troubleshooting/README.md)
- Browse [Common Issues](troubleshooting/common-issues.md)

### Still Stuck?
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/curldotnet)
- [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)

## 📊 Quick Stats

- **300+** curl options supported
- **91** exception types for precise error handling
- **100%** pure .NET - no native dependencies
- **95%** test coverage
- Works on **.NET Framework 4.7.2+**, **.NET Core 2.0+**, **.NET 5-10**

## 🎉 Welcome to CurlDotNet!

Ready to start? Choose your path above and let's begin making HTTP requests the easy way!

---
*Documentation version: 1.0.1 | Last updated: November 2024*