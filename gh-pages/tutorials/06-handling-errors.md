# Tutorial 06: Handling Errors Like a Pro

## 🎯 What You'll Learn

- How to detect when requests fail
- Different types of errors and what they mean
- How to handle errors gracefully
- Best practices for error recovery
- How to provide helpful error messages to users

## 📚 Prerequisites

- [Tutorial 04: Your First Request](04-your-first-request.md)
- [Tutorial 05: Understanding Results](05-understanding-results.md)
- Basic understanding of C# try-catch blocks

## 🚨 Why Error Handling Matters

Network requests can fail for many reasons:
- No internet connection
- Server is down
- Wrong URL
- Timeout
- Authentication failed
- Rate limits exceeded

**Good error handling** = A professional application that users trust
**Bad error handling** = Crashes, confusion, and angry users

## 🔍 The Three Types of Errors

### 1. HTTP Errors (Server Responded)

The server sent a response, but it indicates a problem:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        // This URL doesn't exist
        var result = await Curl.ExecuteAsync("curl https://api.github.com/this-does-not-exist");

        Console.WriteLine($"Status Code: {result.StatusCode}");  // 404
        Console.WriteLine($"Is Success: {result.IsSuccess}");     // false
        Console.WriteLine($"Response: {result.Body}");            // Error message from server
    }
}
```

**Output:**
```
Status Code: 404
Is Success: False
Response: {"message":"Not Found"}
```

### 2. Network Errors (Connection Failed)

The connection couldn't be established:

```csharp
try
{
    // Invalid domain name
    var result = await Curl.ExecuteAsync("curl https://this-domain-does-not-exist-12345.com");
}
catch (CurlException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
    // "Could not resolve host: this-domain-does-not-exist-12345.com"
}
```

### 3. Application Errors (Your Code Failed)

Your code has a bug:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// If response isn't JSON, this will throw
try
{
    var data = result.ParseJson<MyClass>();
}
catch (JsonException ex)
{
    Console.WriteLine($"JSON parsing error: {ex.Message}");
}
```

## 📊 Checking for HTTP Errors

### Method 1: Check IsSuccess

The simplest way to check if a request succeeded:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

if (result.IsSuccess)
{
    Console.WriteLine("Success! Processing data...");
    ProcessData(result.Body);
}
else
{
    Console.WriteLine($"Request failed with status {result.StatusCode}");
    HandleError(result);
}
```

### Method 2: Check Status Code Directly

For more control over different error types:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/user/123");

switch (result.StatusCode)
{
    case 200:
        Console.WriteLine("Success!");
        break;

    case 404:
        Console.WriteLine("User not found. Check the ID and try again.");
        break;

    case 401:
        Console.WriteLine("Not authenticated. Please log in first.");
        break;

    case 403:
        Console.WriteLine("You don't have permission to access this resource.");
        break;

    case 429:
        Console.WriteLine("Too many requests. Please wait and try again.");
        break;

    case >= 500:
        Console.WriteLine("Server error. Please try again later.");
        break;

    default:
        Console.WriteLine($"Unexpected status code: {result.StatusCode}");
        break;
}
```

### Method 3: Check Status Code Ranges

Group errors by category:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

if (result.StatusCode >= 200 && result.StatusCode < 300)
{
    Console.WriteLine("✅ Success!");
}
else if (result.StatusCode >= 400 && result.StatusCode < 500)
{
    Console.WriteLine("❌ Client error - something wrong with our request");
}
else if (result.StatusCode >= 500)
{
    Console.WriteLine("❌ Server error - problem on their end");
}
```

## 🛡️ Handling Network Errors

### Using Try-Catch

Wrap network calls in try-catch blocks:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Exceptions;

class Program
{
    static async Task Main()
    {
        try
        {
            var result = await Curl.ExecuteAsync("curl https://api.example.com");

            if (result.IsSuccess)
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine($"HTTP Error: {result.StatusCode}");
            }
        }
        catch (CurlDnsException ex)
        {
            Console.WriteLine($"DNS Error: Could not find server - {ex.Message}");
        }
        catch (CurlTimeoutException ex)
        {
            Console.WriteLine($"Timeout: Server took too long to respond - {ex.Message}");
        }
        catch (CurlSslException ex)
        {
            Console.WriteLine($"SSL Error: Problem with secure connection - {ex.Message}");
        }
        catch (CurlConnectionException ex)
        {
            Console.WriteLine($"Connection Error: Could not connect - {ex.Message}");
        }
        catch (CurlException ex)
        {
            Console.WriteLine($"General curl error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
```

### Specific Exception Types

CurlDotNet provides specific exception types for different errors:

| Exception | When It Happens | What It Means |
|-----------|-----------------|---------------|
| `CurlDnsException` | Can't resolve domain name | Wrong URL or no internet |
| `CurlTimeoutException` | Request takes too long | Slow server or network |
| `CurlSslException` | SSL certificate problem | Security/certificate issue |
| `CurlConnectionException` | Can't connect to server | Server down or unreachable |
| `CurlException` | Any other curl error | Generic network problem |

### Example: Handling Specific Errors

```csharp
public async Task<string> FetchDataSafely(string url)
{
    try
    {
        var result = await Curl.ExecuteAsync($"curl {url}");

        if (!result.IsSuccess)
        {
            return $"Server returned error: {result.StatusCode}";
        }

        return result.Body;
    }
    catch (CurlDnsException)
    {
        return "Error: Could not find server. Check your internet connection.";
    }
    catch (CurlTimeoutException)
    {
        return "Error: Request timed out. The server is too slow or not responding.";
    }
    catch (CurlSslException)
    {
        return "Error: SSL certificate problem. The connection may not be secure.";
    }
    catch (CurlConnectionException)
    {
        return "Error: Could not connect to server. It may be down.";
    }
    catch (Exception ex)
    {
        return $"Unexpected error: {ex.Message}";
    }
}
```

## 🔄 Retry Logic

### Simple Retry

Try the request multiple times before giving up:

```csharp
public async Task<CurlResult> RequestWithRetry(string command, int maxAttempts = 3)
{
    CurlResult result = null;

    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            Console.WriteLine($"Attempt {attempt} of {maxAttempts}...");

            result = await Curl.ExecuteAsync(command);

            if (result.IsSuccess)
            {
                Console.WriteLine("✅ Success!");
                return result;
            }

            Console.WriteLine($"❌ Failed with status {result.StatusCode}");
        }
        catch (CurlException ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");

            if (attempt == maxAttempts)
            {
                throw; // Re-throw on last attempt
            }
        }

        if (attempt < maxAttempts)
        {
            // Wait before retrying
            int delaySeconds = attempt; // 1s, 2s, 3s...
            Console.WriteLine($"Waiting {delaySeconds} seconds before retry...");
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }

    return result;
}
```

### Usage:

```csharp
try
{
    var result = await RequestWithRetry("curl https://api.example.com/data");
    Console.WriteLine($"Got response: {result.Body}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Failed after all retries: {ex.Message}");
}
```

### Exponential Backoff

Wait longer between each retry:

```csharp
public async Task<CurlResult> RequestWithExponentialBackoff(
    string command,
    int maxAttempts = 5)
{
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            var result = await Curl.ExecuteAsync(command);

            if (result.IsSuccess || result.StatusCode == 404)
            {
                // Don't retry 404s - the resource doesn't exist
                return result;
            }

            if (result.StatusCode == 429)
            {
                // Rate limited - wait longer
                Console.WriteLine("Rate limited. Waiting before retry...");
            }
        }
        catch (CurlTimeoutException)
        {
            Console.WriteLine("Timeout. Retrying...");
        }
        catch (CurlException ex)
        {
            if (attempt == maxAttempts)
                throw;

            Console.WriteLine($"Error: {ex.Message}");
        }

        if (attempt < maxAttempts)
        {
            // Exponential backoff: 1s, 2s, 4s, 8s, 16s
            int delayMs = (int)Math.Pow(2, attempt - 1) * 1000;
            Console.WriteLine($"Waiting {delayMs}ms before retry {attempt + 1}...");
            await Task.Delay(delayMs);
        }
    }

    throw new Exception($"Failed after {maxAttempts} attempts");
}
```

## 📝 Error Message Best Practices

### Bad Error Messages

```csharp
// ❌ Too technical for users
Console.WriteLine("CurlDnsException: getaddrinfo failed (6)");

// ❌ Not helpful
Console.WriteLine("Error occurred");

// ❌ Scary
Console.WriteLine("FATAL ERROR: APPLICATION CRASHED!!!");
```

### Good Error Messages

```csharp
// ✅ Clear and actionable
Console.WriteLine("Could not connect to the server. Please check your internet connection.");

// ✅ Helpful context
Console.WriteLine("The user was not found. Please verify the username and try again.");

// ✅ Next steps provided
Console.WriteLine("Your API key is invalid. Get a new key at https://example.com/settings");
```

### User-Friendly Error Handler

```csharp
public class ErrorHandler
{
    public static string GetUserFriendlyMessage(CurlResult result, Exception exception = null)
    {
        // Network exceptions
        if (exception != null)
        {
            return exception switch
            {
                CurlDnsException =>
                    "We couldn't find that server. Please check the URL and your internet connection.",

                CurlTimeoutException =>
                    "The request took too long. The server might be slow or down. Please try again.",

                CurlSslException =>
                    "There's a problem with the secure connection. The website's certificate might be invalid.",

                CurlConnectionException =>
                    "We couldn't connect to the server. It might be down or unreachable.",

                _ =>
                    "An unexpected network error occurred. Please check your internet and try again."
            };
        }

        // HTTP status code errors
        return result.StatusCode switch
        {
            400 => "The request was invalid. Please check your input and try again.",
            401 => "You need to log in to access this resource.",
            403 => "You don't have permission to access this resource.",
            404 => "The requested resource was not found. Please check the URL.",
            408 => "The request timed out. Please try again.",
            429 => "Too many requests. Please wait a moment and try again.",
            >= 500 and < 600 => "The server is experiencing problems. Please try again later.",
            _ => $"An error occurred (Status: {result.StatusCode}). Please try again."
        };
    }
}
```

### Using the Error Handler

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

    if (!result.IsSuccess)
    {
        string message = ErrorHandler.GetUserFriendlyMessage(result);
        Console.WriteLine(message);
        return;
    }

    // Process successful response
    Console.WriteLine("Data received successfully!");
}
catch (CurlException ex)
{
    string message = ErrorHandler.GetUserFriendlyMessage(null, ex);
    Console.WriteLine(message);
}
```

## 🎯 Complete Error Handling Example

Here's a production-ready example combining all concepts:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Exceptions;

public class ApiClient
{
    private readonly int maxRetries = 3;
    private readonly int timeoutSeconds = 30;

    public async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Console.WriteLine($"Fetching data (attempt {attempt}/{maxRetries})...");

                var result = await Curl.ExecuteAsync(
                    $"curl --max-time {timeoutSeconds} {url}"
                );

                // Check for HTTP errors
                if (!result.IsSuccess)
                {
                    return HandleHttpError<T>(result, attempt);
                }

                // Try to parse response
                try
                {
                    var data = result.ParseJson<T>();
                    return ApiResponse<T>.Success(data);
                }
                catch (JsonException ex)
                {
                    return ApiResponse<T>.Failure(
                        $"Invalid response format: {ex.Message}",
                        "PARSE_ERROR"
                    );
                }
            }
            catch (CurlDnsException ex)
            {
                if (attempt == maxRetries)
                {
                    return ApiResponse<T>.Failure(
                        "Could not find server. Please check your internet connection.",
                        "DNS_ERROR",
                        ex
                    );
                }
            }
            catch (CurlTimeoutException ex)
            {
                if (attempt == maxRetries)
                {
                    return ApiResponse<T>.Failure(
                        $"Request timed out after {timeoutSeconds} seconds.",
                        "TIMEOUT",
                        ex
                    );
                }
            }
            catch (CurlSslException ex)
            {
                // Don't retry SSL errors
                return ApiResponse<T>.Failure(
                    "SSL certificate error. The connection may not be secure.",
                    "SSL_ERROR",
                    ex
                );
            }
            catch (CurlException ex)
            {
                if (attempt == maxRetries)
                {
                    return ApiResponse<T>.Failure(
                        $"Network error: {ex.Message}",
                        "NETWORK_ERROR",
                        ex
                    );
                }
            }

            // Wait before retry
            if (attempt < maxRetries)
            {
                int delayMs = attempt * 1000;
                Console.WriteLine($"Waiting {delayMs}ms before retry...");
                await Task.Delay(delayMs);
            }
        }

        return ApiResponse<T>.Failure(
            $"Failed after {maxRetries} attempts",
            "MAX_RETRIES"
        );
    }

    private ApiResponse<T> HandleHttpError<T>(CurlResult result, int attempt)
    {
        string errorCode = $"HTTP_{result.StatusCode}";
        string message = result.StatusCode switch
        {
            400 => "Bad request. Please check your input.",
            401 => "Authentication required. Please log in.",
            403 => "Access denied. You don't have permission.",
            404 => "Resource not found. Please check the URL.",
            429 => "Too many requests. Please slow down.",
            >= 500 => "Server error. Please try again later.",
            _ => $"Request failed with status {result.StatusCode}"
        };

        // Don't retry client errors (except 429)
        bool shouldRetry = result.StatusCode == 429 || result.StatusCode >= 500;

        return ApiResponse<T>.Failure(message, errorCode, shouldRetry: shouldRetry);
    }
}

// Response wrapper class
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Error { get; set; }
    public string ErrorCode { get; set; }
    public Exception Exception { get; set; }
    public bool ShouldRetry { get; set; }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponse<T> Failure(
        string error,
        string errorCode,
        Exception ex = null,
        bool shouldRetry = false)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            ErrorCode = errorCode,
            Exception = ex,
            ShouldRetry = shouldRetry
        };
    }
}
```

### Using the Production Client

```csharp
public class Program
{
    static async Task Main()
    {
        var client = new ApiClient();

        var response = await client.GetAsync<GitHubUser>(
            "https://api.github.com/users/octocat"
        );

        if (response.Success)
        {
            Console.WriteLine($"✅ Got user: {response.Data.Name}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {response.Error}");
            Console.WriteLine($"   Error code: {response.ErrorCode}");

            if (response.Exception != null)
            {
                Console.WriteLine($"   Details: {response.Exception.Message}");
            }
        }
    }
}

public class GitHubUser
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
}
```

## 🎓 Key Takeaways

1. **Always check `IsSuccess`** before using response data
2. **Use try-catch** for network exceptions
3. **Provide helpful error messages** that tell users what to do
4. **Implement retry logic** for transient failures
5. **Don't retry** 404s and other permanent errors
6. **Use exponential backoff** to avoid overwhelming servers
7. **Handle specific exceptions** differently
8. **Log errors** for debugging
9. **Return error details** instead of throwing exceptions

## 🚀 Next Steps

Now that you can handle errors:

1. **Next Tutorial** → [JSON for Beginners](07-json-for-beginners.md)
2. **Practice** - Add error handling to your existing code
3. **Experiment** - Try different error scenarios
4. **Read** - Check the [Troubleshooting Guide](../troubleshooting/README.md)

## 📚 Summary

Error handling isn't optional - it's essential for professional applications. You've learned how to:
- Detect HTTP errors using status codes
- Catch network exceptions with try-catch
- Implement retry logic with exponential backoff
- Provide user-friendly error messages
- Build a production-ready API client

Good error handling makes the difference between a toy project and production-ready software.

---

**Ready to work with JSON?** → [JSON for Beginners](07-json-for-beginners.md)

**Need help?** Check the [Troubleshooting Guide](../troubleshooting/README.md) or ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
