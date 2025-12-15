# Recipe: POST Form Data

## 🎯 What You'll Build

Programs that submit HTML forms programmatically - login forms, search forms, contact forms, and more.

## 🥘 Ingredients

- CurlDotNet package
- An API endpoint that accepts form data
- 10 minutes

## 📖 The Recipe

### Step 1: Simple Form POST

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Submitting form...");

        // POST form data (application/x-www-form-urlencoded)
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
              -d 'username=john' \
              -d 'password=secret123'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Form submitted successfully!");
            Console.WriteLine(result.Body);
        }
    }
}
```

### Step 2: Multiple Form Fields

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class ContactForm
{
    static async Task Main()
    {
        Console.WriteLine("Submitting contact form...");

        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
              -d 'name=John Doe' \
              -d 'email=john@example.com' \
              -d 'subject=Question' \
              -d 'message=Hello, I have a question'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Contact form submitted!");
        }
    }
}
```

## 🍳 Complete Examples

### Example 1: Login Form

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class LoginForm
{
    static async Task<string> Login(string username, string password)
    {
        Console.WriteLine($"Logging in as {username}...");

        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://httpbin.org/post \
              -d 'username={username}' \
              -d 'password={password}' \
              -d 'remember=true'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Login successful!");

            // Extract token from response
            dynamic response = result.AsJsonDynamic();
            string token = response.form.token ?? "dummy-token";

            return token;
        }
        else if (result.StatusCode == 401)
        {
            Console.WriteLine("✗ Invalid credentials");
            return null;
        }
        else
        {
            Console.WriteLine($"✗ Login failed: {result.StatusCode}");
            return null;
        }
    }

    static async Task Main()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        string token = await Login(username, password);

        if (token != null)
        {
            Console.WriteLine($"Token: {token}");
        }
    }
}
```

### Example 2: Search Form

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class SearchForm
{
    static async Task<SearchResults> Search(string query, string category = "all", int page = 1)
    {
        Console.WriteLine($"Searching for '{query}'...");

        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://httpbin.org/post \
              -d 'q={Uri.EscapeDataString(query)}' \
              -d 'category={category}' \
              -d 'page={page}' \
              -d 'limit=10'
        ");

        if (result.IsSuccess)
        {
            return result.ParseJson<SearchResults>();
        }

        return null;
    }

    public class SearchResults
    {
        public int Total { get; set; }
        public SearchItem[] Items { get; set; }
    }

    public class SearchItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }

    static async Task Main()
    {
        var results = await Search("C# programming", "tutorials", 1);

        if (results != null)
        {
            Console.WriteLine($"Found {results.Total} results");
        }
    }
}
```

### Example 3: Registration Form

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class RegistrationForm
{
    public class UserRegistration
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool AcceptTerms { get; set; }
        public bool Newsletter { get; set; }
    }

    static async Task<bool> Register(UserRegistration user)
    {
        Console.WriteLine($"Registering user {user.Username}...");

        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://httpbin.org/post \
              -d 'username={user.Username}' \
              -d 'email={user.Email}' \
              -d 'password={user.Password}' \
              -d 'full_name={user.FullName}' \
              -d 'accept_terms={user.AcceptTerms.ToString().ToLower()}' \
              -d 'newsletter={user.Newsletter.ToString().ToLower()}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Registration successful!");
            return true;
        }
        else if (result.StatusCode == 409)
        {
            Console.WriteLine("✗ Username or email already exists");
            return false;
        }
        else
        {
            Console.WriteLine($"✗ Registration failed: {result.StatusCode}");
            return false;
        }
    }

    static async Task Main()
    {
        var newUser = new UserRegistration
        {
            Username = "johndoe",
            Email = "john@example.com",
            Password = "SecurePass123!",
            FullName = "John Doe",
            AcceptTerms = true,
            Newsletter = true
        };

        bool success = await Register(newUser);

        if (success)
        {
            Console.WriteLine("Welcome! Please check your email to verify your account.");
        }
    }
}
```

### Example 4: Form with Arrays/Multiple Values

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class FormWithArrays
{
    static async Task Main()
    {
        string[] interests = { "coding", "reading", "gaming" };
        string[] languages = { "C#", "JavaScript", "Python" };

        Console.WriteLine("Submitting preferences...");

        // Build form data with arrays
        string formData = "name=John";
        foreach (var interest in interests)
        {
            formData += $" -d 'interests[]={interest}'";
        }
        foreach (var lang in languages)
        {
            formData += $" -d 'languages[]={lang}'";
        }

        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://httpbin.org/post \
              -d '{formData}'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Preferences submitted!");
            dynamic response = result.AsJsonDynamic();
            Console.WriteLine($"Interests: {response.form.interests}");
            Console.WriteLine($"Languages: {response.form.languages}");
        }
    }
}
```

### Example 5: Form Data from Object

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurlDotNet;

class FormBuilder
{
    static string BuildFormData(Dictionary<string, string> fields)
    {
        return string.Join(" ", fields.Select(kvp =>
            $"-d '{kvp.Key}={Uri.EscapeDataString(kvp.Value)}'"
        ));
    }

    static async Task Main()
    {
        var formData = new Dictionary<string, string>
        {
            ["first_name"] = "John",
            ["last_name"] = "Doe",
            ["email"] = "john@example.com",
            ["phone"] = "+1-555-0123",
            ["message"] = "Hello, I'd like more information"
        };

        string dataString = BuildFormData(formData);

        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://httpbin.org/post {dataString}
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Form submitted!");
        }
    }
}
```

### Example 6: Multipart Form (Form + Files)

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class MultipartForm
{
    static async Task Main()
    {
        Console.WriteLine("Submitting multipart form...");

        // Multipart form with both fields and files
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
              -F 'name=John Doe' \
              -F 'email=john@example.com' \
              -F 'profile_picture=@photo.jpg' \
              -F 'resume=@resume.pdf' \
              -F 'cover_letter=@letter.txt'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Application submitted!");
            dynamic response = result.AsJsonDynamic();
            Console.WriteLine($"Files uploaded: {response.files}");
        }
    }
}
```

### Example 7: Form with Custom Content Type

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class CustomContentType
{
    static async Task Main()
    {
        // Explicitly set content type
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
              -H 'Content-Type: application/x-www-form-urlencoded' \
              -d 'key1=value1' \
              -d 'key2=value2'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Form submitted with custom content type!");
        }
    }
}
```

## 🎨 Variations

### URL-Encoded Form Data (Default)

```csharp
// Standard form submission
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/form \
      -d 'field1=value1' \
      -d 'field2=value2'
");
```

### Multipart Form Data

```csharp
// For file uploads and complex forms
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/form \
      -F 'field1=value1' \
      -F 'field2=value2' \
      -F 'file=@document.pdf'
");
```

### Form Data from String

```csharp
// All fields in one string
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/form \
      -d 'field1=value1&field2=value2&field3=value3'
");
```

### Form Data with Authentication

```csharp
string apiKey = Environment.GetEnvironmentVariable("API_KEY");

var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/form \
      -H 'Authorization: Bearer {apiKey}' \
      -d 'field1=value1' \
      -d 'field2=value2'
");
```

## 🐛 Troubleshooting

### Problem: Special Characters in Form Data

**Solution:**
```csharp
// URL-encode special characters
string message = "Hello & welcome to our site!";
string encoded = Uri.EscapeDataString(message);

var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/form \
      -d 'message={encoded}'
");
```

### Problem: Form Submission Returns 400 Bad Request

**Solution:**
```csharp
// Check required fields and data types
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/form \
      -d 'username=john' \
      -d 'email=john@example.com' \
      -d 'age=25'
");

if (result.StatusCode == 400)
{
    Console.WriteLine("Bad request - check required fields:");
    Console.WriteLine(result.Body); // Server usually provides details
}
```

For more details, see our [HTTP error troubleshooting guide](../../troubleshooting/common-issues.md#http-errors).

### Problem: Form Data Not Being Sent

**Solution:**
```csharp
// Ensure you're using POST method
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/form \
      -d 'field=value'
");

// Check what was sent
Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Response: {result.Body}");
```

### Problem: Authentication Required

**Solution:**
```csharp
try
{
    var result = await Curl.ExecuteAsync(@"
        curl -X POST https://api.example.com/form \
          -d 'field=value'
    ");
}
catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Authentication required. Add credentials:");
    Console.WriteLine("Example: -H 'Authorization: Bearer token'");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#authentication-errors
}
```

## 🎓 Best Practices

### 1. Validate Input Before Submitting

```csharp
bool ValidateEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}

string email = "john@example.com";

if (!ValidateEmail(email))
{
    Console.WriteLine("Invalid email address");
    return;
}

var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/form \
      -d 'email={email}'
");
```

### 2. Escape Special Characters

```csharp
// Always escape user input
string userInput = "O'Brien & Associates";
string safe = Uri.EscapeDataString(userInput);

var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/form \
      -d 'company={safe}'
");
```

### 3. Handle Empty Fields

```csharp
string optionalField = "";

var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/form \
      -d 'required_field=value' \
      {(string.IsNullOrEmpty(optionalField) ? "" : $"-d 'optional_field={optionalField}'")}
");
```

### 4. Use HTTPS for Sensitive Data

```csharp
// Always use HTTPS for passwords and sensitive data
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/login \
      -d 'username=john' \
      -d 'password=secret123'
");

// Never use HTTP for sensitive data!
// var result = await Curl.ExecuteAsync("curl -X POST http://api.example.com/login ...");
```

### 5. Check Response Status

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/form \
      -d 'field=value'
");

if (result.StatusCode == 200)
{
    Console.WriteLine("✓ Form submitted successfully");
}
else if (result.StatusCode == 400)
{
    Console.WriteLine("✗ Invalid form data");
    Console.WriteLine($"Details: {result.Body}");
}
else if (result.StatusCode == 422)
{
    Console.WriteLine("✗ Validation failed");
    Console.WriteLine($"Errors: {result.Body}");
}
else
{
    Console.WriteLine($"✗ Unexpected status: {result.StatusCode}");
}
```

## 🚀 Next Steps

Now that you can submit forms:

1. Learn to [Upload Files](upload-file.html) with multipart forms
2. Try [Send JSON](send-json.html) for API requests
3. Explore [Error Handling](handle-errors.html)
4. Build [API Client](call-api.html)

## 📚 Related Recipes

- [Send JSON](send-json.html) - Alternative to form data
- [Upload Files](upload-file.html) - Multipart form uploads
- [Simple GET](simple-get.html) - Fetching data
- [Handle Errors](handle-errors.html) - Robust error handling

## 🎓 Key Takeaways

- Use `-d` for URL-encoded form data
- Use `-F` for multipart form data (with files)
- Always escape special characters with `Uri.EscapeDataString()`
- Use HTTPS for sensitive data (passwords, etc.)
- Validate input before submitting
- Check response status codes
- Handle errors gracefully
- Multiple `-d` flags for multiple fields

## 📖 Quick Reference

```csharp
// Simple form
await Curl.ExecuteAsync(@"
    curl -X POST {url} \
      -d 'field1=value1' \
      -d 'field2=value2'
");

// All fields in one string
await Curl.ExecuteAsync(@"
    curl -X POST {url} \
      -d 'field1=value1&field2=value2'
");

// With authentication
await Curl.ExecuteAsync($@"
    curl -X POST {url} \
      -H 'Authorization: Bearer {token}' \
      -d 'field=value'
");

// Multipart form (with files)
await Curl.ExecuteAsync(@"
    curl -X POST {url} \
      -F 'field=value' \
      -F 'file=@document.pdf'
");

// With escaped special characters
string safe = Uri.EscapeDataString(userInput);
await Curl.ExecuteAsync($@"
    curl -X POST {url} \
      -d 'field={safe}'
");
```

---

**Need help?** Check [Troubleshooting](../../troubleshooting/common-issues.html) | **Have questions?** Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
