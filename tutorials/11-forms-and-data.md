# Tutorial 11: Forms and Data

Learn how to submit HTML forms, work with different content types, and send structured data with CurlDotNet.

## HTML Form Submission

### Simple Form POST
```csharp
var curl = new Curl();

// Create form data
var formData = new Dictionary<string, string>
{
    ["username"] = "john_doe",
    ["email"] = "john@example.com",
    ["age"] = "25",
    ["subscribe"] = "true"
};

// Submit form
var result = await curl.PostFormAsync("https://example.com/register", formData);

if (result.IsSuccess)
{
    Console.WriteLine("Form submitted successfully!");
}
```

### URL-Encoded Forms
```csharp
// Method 1: Using PostFormAsync
var loginData = new Dictionary<string, string>
{
    ["username"] = "user@example.com",
    ["password"] = "secure123",
    ["remember_me"] = "1"
};

var result = await curl.PostFormAsync("https://example.com/login", loginData);

// Method 2: Manual encoding
var encodedData = string.Join("&",
    loginData.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")
);

curl.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
var result = await curl.PostAsync("https://example.com/login", encodedData);
```

## Multipart Forms

### Text Fields with File Upload
```csharp
public async Task SubmitApplicationForm(
    string name,
    string email,
    string resumePath,
    string coverLetterPath)
{
    var curl = new Curl();
    using var form = new MultipartFormDataContent();

    // Add text fields
    form.Add(new StringContent(name), "name");
    form.Add(new StringContent(email), "email");
    form.Add(new StringContent("Software Developer"), "position");

    // Add files
    var resumeBytes = File.ReadAllBytes(resumePath);
    var resumeContent = new ByteArrayContent(resumeBytes);
    resumeContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
    form.Add(resumeContent, "resume", Path.GetFileName(resumePath));

    var coverLetterBytes = File.ReadAllBytes(coverLetterPath);
    var coverContent = new ByteArrayContent(coverLetterBytes);
    coverContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
    form.Add(coverContent, "cover_letter", Path.GetFileName(coverLetterPath));

    var result = await curl.PostAsync("https://careers.example.com/apply", form);
}
```

### Complex Multipart Data
```csharp
public async Task SubmitComplexForm()
{
    var boundary = "----WebKitFormBoundary" + Guid.NewGuid().ToString("N");
    var content = new MultipartFormDataContent(boundary);

    // Text field
    content.Add(new StringContent("John Doe"), "name");

    // JSON field
    var metadata = new { category = "photos", tags = new[] { "vacation", "2024" } };
    var jsonContent = new StringContent(
        JsonSerializer.Serialize(metadata),
        Encoding.UTF8,
        "application/json"
    );
    content.Add(jsonContent, "metadata");

    // Multiple files
    var photos = Directory.GetFiles("/path/to/photos", "*.jpg");
    foreach (var photo in photos.Take(5))
    {
        var photoBytes = File.ReadAllBytes(photo);
        var photoContent = new ByteArrayContent(photoBytes);
        photoContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(photoContent, "photos[]", Path.GetFileName(photo));
    }

    var curl = new Curl();
    var result = await curl.PostAsync("https://api.example.com/upload-batch", content);
}
```

## Working with Different Content Types

### JSON Data
```csharp
// Send JSON
var userData = new
{
    name = "Jane Smith",
    email = "jane@example.com",
    preferences = new
    {
        newsletter = true,
        notifications = false
    }
};

var result = await curl.PostJsonAsync("https://api.example.com/users", userData);
```

### XML Data
```csharp
// Send XML
var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<user>
    <name>John Doe</name>
    <email>john@example.com</email>
    <age>30</age>
</user>";

curl.Headers.Add("Content-Type", "application/xml");
var result = await curl.PostAsync("https://api.example.com/users", xmlData);
```

### Plain Text
```csharp
// Send plain text
var textData = "This is a plain text message.";

curl.Headers.Add("Content-Type", "text/plain");
var result = await curl.PostAsync("https://api.example.com/message", textData);
```

## Query Parameters

### Building URLs with Parameters
```csharp
public class QueryBuilder
{
    public static string BuildUrl(string baseUrl, Dictionary<string, string> parameters)
    {
        if (parameters == null || parameters.Count == 0)
            return baseUrl;

        var queryString = string.Join("&",
            parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")
        );

        return baseUrl.Contains("?")
            ? $"{baseUrl}&{queryString}"
            : $"{baseUrl}?{queryString}";
    }
}

// Usage
var parameters = new Dictionary<string, string>
{
    ["search"] = "curl tutorial",
    ["page"] = "1",
    ["limit"] = "20",
    ["sort"] = "relevance"
};

var url = QueryBuilder.BuildUrl("https://api.example.com/search", parameters);
// Result: https://api.example.com/search?search=curl%20tutorial&page=1&limit=20&sort=relevance

var result = await curl.GetAsync(url);
```

### Complex Query Parameters
```csharp
public class AdvancedQueryBuilder
{
    public static string BuildUrl(string baseUrl, object parameters)
    {
        var properties = parameters.GetType().GetProperties();
        var queryParts = new List<string>();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(parameters);

            if (value == null) continue;

            if (value is IEnumerable<string> list)
            {
                // Handle arrays: key[]=value1&key[]=value2
                foreach (var item in list)
                {
                    queryParts.Add($"{prop.Name}[]={Uri.EscapeDataString(item)}");
                }
            }
            else if (value is DateTime date)
            {
                // Handle dates
                queryParts.Add($"{prop.Name}={Uri.EscapeDataString(date.ToString("yyyy-MM-dd"))}");
            }
            else
            {
                // Handle simple values
                queryParts.Add($"{prop.Name}={Uri.EscapeDataString(value.ToString())}");
            }
        }

        var queryString = string.Join("&", queryParts);
        return string.IsNullOrEmpty(queryString)
            ? baseUrl
            : $"{baseUrl}?{queryString}";
    }
}

// Usage
var searchParams = new
{
    query = "CurlDotNet",
    categories = new[] { "tutorials", "documentation" },
    dateFrom = DateTime.Now.AddDays(-30),
    dateTo = DateTime.Now,
    includeArchived = false
};

var url = AdvancedQueryBuilder.BuildUrl("https://api.example.com/search", searchParams);
```

## Form Validation and Error Handling

### Client-Side Validation
```csharp
public class FormValidator
{
    public static List<string> ValidateRegistrationForm(Dictionary<string, string> formData)
    {
        var errors = new List<string>();

        // Required fields
        if (!formData.ContainsKey("email") || string.IsNullOrWhiteSpace(formData["email"]))
            errors.Add("Email is required");

        if (!formData.ContainsKey("password") || string.IsNullOrWhiteSpace(formData["password"]))
            errors.Add("Password is required");

        // Email format
        if (formData.TryGetValue("email", out var email))
        {
            if (!email.Contains("@") || !email.Contains("."))
                errors.Add("Invalid email format");
        }

        // Password strength
        if (formData.TryGetValue("password", out var password))
        {
            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters");
        }

        return errors;
    }
}

// Usage
var formData = new Dictionary<string, string>
{
    ["email"] = "user@example.com",
    ["password"] = "short"
};

var errors = FormValidator.ValidateRegistrationForm(formData);
if (errors.Any())
{
    Console.WriteLine("Form validation failed:");
    errors.ForEach(e => Console.WriteLine($"- {e}"));
}
else
{
    var result = await curl.PostFormAsync("https://example.com/register", formData);
}
```

### Handling Form Submission Errors
```csharp
public async Task<bool> SubmitFormWithErrorHandling(string url, Dictionary<string, string> formData)
{
    var curl = new Curl();
    var result = await curl.PostFormAsync(url, formData);

    if (result.IsSuccess)
    {
        // Check for application-level errors in response
        try
        {
            var response = JsonSerializer.Deserialize<FormResponse>(result.Data);

            if (response.Success)
            {
                Console.WriteLine("Form submitted successfully!");
                return true;
            }
            else
            {
                Console.WriteLine("Form submission failed:");
                foreach (var error in response.Errors ?? new List<string>())
                {
                    Console.WriteLine($"- {error}");
                }
                return false;
            }
        }
        catch (JsonException)
        {
            // Non-JSON response might indicate success
            return true;
        }
    }
    else
    {
        // HTTP-level error
        Console.WriteLine($"HTTP Error {result.StatusCode}: {result.Error}");
        return false;
    }
}

class FormResponse
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; }
    public string Message { get; set; }
}
```

## CSRF Protection

### Handling CSRF Tokens
```csharp
public class CsrfProtectedForm
{
    private readonly Curl _curl = new Curl();
    private string _csrfToken;

    public async Task<string> GetCsrfToken(string formUrl)
    {
        // GET the form page to extract CSRF token
        var result = await _curl.GetAsync(formUrl);

        if (result.IsSuccess)
        {
            // Parse HTML to find CSRF token
            // Simplified - use proper HTML parser in production
            var match = Regex.Match(result.Data,
                @"<input.*?name=""csrf_token"".*?value=""([^""]+)""");

            if (match.Success)
            {
                _csrfToken = match.Groups[1].Value;
                return _csrfToken;
            }
        }

        throw new Exception("Could not retrieve CSRF token");
    }

    public async Task<bool> SubmitProtectedForm(string url, Dictionary<string, string> formData)
    {
        // Add CSRF token to form data
        formData["csrf_token"] = _csrfToken;

        var result = await _curl.PostFormAsync(url, formData);
        return result.IsSuccess;
    }
}
```

## Form Data Serialization

### Custom Form Serializer
```csharp
public static class FormSerializer
{
    public static Dictionary<string, string> Serialize<T>(T obj)
    {
        var formData = new Dictionary<string, string>();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);

            if (value == null) continue;

            // Handle different types
            if (value is bool boolValue)
            {
                formData[prop.Name] = boolValue ? "1" : "0";
            }
            else if (value is DateTime dateValue)
            {
                formData[prop.Name] = dateValue.ToString("yyyy-MM-dd");
            }
            else if (value is IEnumerable<string> list && !(value is string))
            {
                var items = list.ToList();
                for (int i = 0; i < items.Count; i++)
                {
                    formData[$"{prop.Name}[{i}]"] = items[i];
                }
            }
            else
            {
                formData[prop.Name] = value.ToString();
            }
        }

        return formData;
    }
}

// Usage
public class UserProfile
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool Newsletter { get; set; }
    public List<string> Interests { get; set; }
}

var profile = new UserProfile
{
    Name = "Jane Doe",
    Email = "jane@example.com",
    BirthDate = new DateTime(1990, 5, 15),
    Newsletter = true,
    Interests = new List<string> { "tech", "music", "travel" }
};

var formData = FormSerializer.Serialize(profile);
var result = await curl.PostFormAsync("https://example.com/profile", formData);
```

## Streaming Form Data

### Large Form Submission
```csharp
public async Task SubmitLargeForm(string url, Stream dataStream)
{
    var curl = new Curl();

    // Set content type and length
    curl.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
    curl.Headers.Add("Content-Length", dataStream.Length.ToString());

    // Stream the content
    using var streamContent = new StreamContent(dataStream);
    var result = await curl.PostAsync(url, streamContent);

    if (result.IsSuccess)
    {
        Console.WriteLine("Large form submitted successfully");
    }
}
```

## Best Practices

1. **Validate before submission** - Check required fields client-side
2. **Encode special characters** - Use URL encoding for form data
3. **Handle CSRF tokens** - Include security tokens when required
4. **Set correct Content-Type** - Match the server's expectations
5. **Limit file sizes** - Check before uploading large files
6. **Use HTTPS** - Never send sensitive form data over HTTP
7. **Handle errors gracefully** - Provide user-friendly error messages
8. **Implement retry logic** - Handle temporary network failures
9. **Clean user input** - Sanitize data before submission
10. **Log submissions** - Track form submission attempts

## Summary

Working with forms and data in CurlDotNet:
- Simple methods for common form submissions
- Support for various content types
- Multipart forms for file uploads
- Query parameter building
- Form validation and error handling

## What's Next?

Learn about [cancellation tokens](12-cancellation-tokens.html) for managing long-running requests.

---

[← Previous: Files and Downloads](10-files-and-downloads.html) | [Next: Cancellation Tokens →](12-cancellation-tokens.html)