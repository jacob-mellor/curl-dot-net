# CurlDotNet Samples

This directory contains sample applications demonstrating how to use CurlDotNet.

## Available Samples

### CurlDotNet.Sample
A console application showing common usage patterns:

- Direct curl command execution
- GET and POST requests
- JSON handling
- Fluent API usage
- Error handling
- Headers and authentication

## Running the Samples

1. **Navigate to the sample directory:**
   ```bash
   cd samples/CurlDotNet.Sample
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

## Sample Output

The sample application demonstrates:

1. **Direct Curl Commands** - Paste curl commands directly into your code
2. **HTTP Methods** - GET, POST, and other HTTP operations
3. **JSON Support** - Parse and send JSON data
4. **Fluent API** - Build requests using the builder pattern
5. **Error Handling** - Proper exception handling for HTTP errors
6. **Authentication** - Bearer tokens and custom headers

## Creating Your Own Project

To use CurlDotNet in your own project:

1. **Install from NuGet:**
   ```bash
   dotnet add package CurlDotNet
   ```

2. **Basic usage:**
   ```csharp
   using CurlDotNet;

   var result = await Curl.GetAsync("https://api.example.com/data");
   Console.WriteLine(result.Body);
   ```

3. **With curl command:**
   ```csharp
   var result = await Curl.ExecuteAsync("curl -X GET https://api.example.com/data -H 'Accept: application/json'");
   ```

## Learn More

- [Main README](../README.md) - Project overview
- [Documentation](../docs/README.md) - Detailed documentation
- [API Reference](https://jacob-mellor.github.io/curl-dot-net) - Full API documentation