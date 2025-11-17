# 👨‍🍳 CurlDotNet Cookbook

Welcome to the CurlDotNet Cookbook! Find ready-to-use recipes for common tasks.

## 📚 Recipe Categories

### 🆕 [Beginner Recipes](beginner/)
Perfect for getting started
- [Simple GET Request](beginner/simple-get.md)
- [POST JSON Data](beginner/send-json.md)
- [Download a File](beginner/download-file.md)
- [Upload a File](beginner/upload-file.md)
- [Submit a Form](beginner/post-form.md)
- [Call an API](beginner/call-api.md)
- [Handle Errors Gracefully](beginner/handle-errors.md)


## 🔍 Quick Recipe Finder

### By Task

**"I want to..."**

#### Download Something
- [Download a webpage](beginner/simple-get.md)
- [Download a file](beginner/download-file.md)

#### Upload Something
- [Upload a file](beginner/upload-file.md)
- [Upload with form data](beginner/post-form.md)

#### Work with APIs
- [Call a REST API](beginner/call-api.md)
- [Send JSON data](beginner/send-json.md)
- [Handle API errors](beginner/handle-errors.md)
- Check the API Guide for advanced patterns

#### Authentication
- [API keys and bearer tokens](beginner/call-api.md)

#### Error Handling
- [Basic error handling](beginner/handle-errors.md)

## 📝 Recipe Format

Each recipe includes:

1. **What You'll Build** - Clear description
2. **Ingredients** - What you need (packages, API keys, etc.)
3. **The Recipe** - Step-by-step instructions
4. **Complete Code** - Full working example
5. **Variations** - Different approaches
6. **Troubleshooting** - Common issues
7. **Next Steps** - Where to go from here

## 🎯 How to Use This Cookbook

### For Beginners
1. Start with [Simple GET Request](beginner/simple-get.md)
2. Try each beginner recipe in order
3. Move to intermediate when comfortable

### For Specific Tasks
1. Use the Quick Recipe Finder above
2. Jump directly to what you need
3. Check "Variations" for alternatives

### For Learning
1. Pick a real-world example
2. Build it step-by-step
3. Modify it for your needs

## 💡 Tips for Success

### Always Test First
```csharp
// Test with a simple request first
var test = await Curl.ExecuteAsync("curl https://httpbin.org/get");
if (test.IsSuccess)
{
    Console.WriteLine("Connection works!");
}
```

### Start Simple
```csharp
// Start with basic version
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Then add features
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com \
    -H 'Authorization: Bearer token' \
    -H 'Accept: application/json'
");
```

### Use httpbin.org for Testing
```csharp
// httpbin.org is perfect for testing
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
    -d 'test=data'
");

// It echoes back what you sent
Console.WriteLine(result.Body);
```

## 🆘 Getting Help

### Can't Find a Recipe?
- Browse [API Guide](../api-guide/README.md)
- Check [Tutorials](../tutorials/README.md)
- Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)

### Recipe Not Working?
- Check [Troubleshooting](../troubleshooting/README.md)
- Verify your CurlDotNet version
- Make sure you have all required packages

## 🤝 Contributing Recipes

Have a great recipe? We'd love to include it!

1. Follow the recipe format
2. Include complete, working code
3. Test thoroughly
4. Submit a pull request

## 📚 Related Resources

- [Tutorials](../tutorials/README.md) - Learn the basics
- [API Guide](../api-guide/README.md) - Detailed reference
- [Troubleshooting](../troubleshooting/README.md) - Fix common issues

---

**Ready to cook?** Start with → [Simple GET Request](beginner/simple-get.md)

*All recipes are tested with CurlDotNet 1.0.1+ and work on .NET Framework 4.7.2+, .NET Core 2.0+, and .NET 5-10*