---
layout: default
title: Session and Cookie Management in C# with curl for .NET
description: Master cookie handling, session persistence, and authentication state management using CurlDotNet
keywords: C# cookies, session management, curl cookie jar, .NET authentication, cookie handling
---

# Session Cookies Management

## Handle Cookies and Sessions with curl for C# and .NET

Learn how to manage cookies, maintain session state, persist authentication, and handle complex cookie scenarios using CurlDotNet.

## Why Cookie Management Matters

- **Authentication**: Keep users logged in across requests
- **Session State**: Maintain shopping carts, preferences
- **CSRF Protection**: Handle anti-forgery tokens
- **Load Balancing**: Maintain server affinity
- **Personalization**: Track user preferences

## Basic Cookie Handling

```csharp
using CurlDotNet;
using System.Net;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class BasicCookieClient
{
    private readonly Curl _curl;
    private readonly CookieContainer _cookieContainer;

    public BasicCookieClient(string baseUrl)
    {
        _cookieContainer = new CookieContainer();
        _curl = new Curl(baseUrl)
            .WithCookieContainer(_cookieContainer);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        // Login request - cookies will be automatically stored
        var response = await _curl.PostAsync("/login")
            .WithFormData(new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password
            })
            .ExecuteAsync();

        if (response.StatusCode == 200)
        {
            // Session cookie is now stored in _cookieContainer
            Console.WriteLine($"Logged in. Cookies: {GetCookieCount()}");
            return true;
        }

        return false;
    }

    public async Task<string> GetProtectedDataAsync()
    {
        // Cookies are automatically sent with this request
        var response = await _curl.GetAsync("/api/protected");
        return response.Body;
    }

    public void PrintCookies()
    {
        foreach (Cookie cookie in _cookieContainer.GetCookies(new Uri(_curl.BaseUrl)))
        {
            Console.WriteLine($"{cookie.Name}={cookie.Value} " +
                $"(Expires: {cookie.Expires}, HttpOnly: {cookie.HttpOnly})");
        }
    }

    private int GetCookieCount()
    {
        return _cookieContainer.GetCookies(new Uri(_curl.BaseUrl)).Count;
    }
}
```

## Advanced Cookie Manager

```csharp
using CurlDotNet;
using System.Text.Json;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class AdvancedCookieManager
{
    private readonly Curl _curl;
    private readonly string _cookieFilePath;
    private readonly Dictionary<string, Cookie> _cookies;
    private readonly object _lock = new();

    public AdvancedCookieManager(string baseUrl, string cookieFilePath = null)
    {
        _curl = new Curl(baseUrl);
        _cookieFilePath = cookieFilePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "myapp",
            "cookies.json");
        _cookies = new Dictionary<string, Cookie>();

        LoadCookiesFromFile();
    }

    public async Task<CurlResult> ExecuteWithCookiesAsync(
        Func<Curl, Task<CurlResult>> operation)
    {
        // Add all cookies to request
        foreach (var cookie in _cookies.Values)
        {
            if (!cookie.Expired)
            {
                _curl.WithCookie(cookie.Name, cookie.Value);
            }
        }

        var result = await operation(_curl);

        // Extract and save new cookies from response
        ExtractCookiesFromResponse(result);
        SaveCookiesToFile();

        return result;
    }

    private void ExtractCookiesFromResponse(CurlResult response)
    {
        if (response.Headers?.ContainsKey("Set-Cookie") == true)
        {
            var setCookieHeaders = response.Headers["Set-Cookie"];
            var cookies = ParseSetCookieHeaders(setCookieHeaders);

            lock (_lock)
            {
                foreach (var cookie in cookies)
                {
                    _cookies[cookie.Name] = cookie;
                }
            }
        }
    }

    private List<Cookie> ParseSetCookieHeaders(string setCookieHeaders)
    {
        var cookies = new List<Cookie>();
        var cookieStrings = setCookieHeaders.Split(';');

        foreach (var cookieString in cookieStrings)
        {
            var parts = cookieString.Split('=', 2);
            if (parts.Length == 2)
            {
                var cookie = new Cookie
                {
                    Name = parts[0].Trim(),
                    Value = parts[1].Trim(),
                    Domain = new Uri(_curl.BaseUrl).Host,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(30)
                };

                // Parse additional attributes
                if (cookieString.Contains("HttpOnly", StringComparison.OrdinalIgnoreCase))
                {
                    cookie.HttpOnly = true;
                }
                if (cookieString.Contains("Secure", StringComparison.OrdinalIgnoreCase))
                {
                    cookie.Secure = true;
                }

                cookies.Add(cookie);
            }
        }

        return cookies;
    }

    private void LoadCookiesFromFile()
    {
        if (File.Exists(_cookieFilePath))
        {
            try
            {
                var json = File.ReadAllText(_cookieFilePath);
                var cookies = JsonSerializer.Deserialize<List<CookieData>>(json);

                lock (_lock)
                {
                    foreach (var cookieData in cookies)
                    {
                        var cookie = new Cookie(
                            cookieData.Name,
                            cookieData.Value,
                            cookieData.Path,
                            cookieData.Domain)
                        {
                            Expires = cookieData.Expires,
                            HttpOnly = cookieData.HttpOnly,
                            Secure = cookieData.Secure
                        };

                        _cookies[cookie.Name] = cookie;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load cookies: {ex.Message}");
            }
        }
    }

    private void SaveCookiesToFile()
    {
        try
        {
            var directory = Path.GetDirectoryName(_cookieFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            lock (_lock)
            {
                var cookieData = _cookies.Values
                    .Where(c => !c.Expired)
                    .Select(c => new CookieData
                    {
                        Name = c.Name,
                        Value = c.Value,
                        Domain = c.Domain,
                        Path = c.Path,
                        Expires = c.Expires,
                        HttpOnly = c.HttpOnly,
                        Secure = c.Secure
                    })
                    .ToList();

                var json = JsonSerializer.Serialize(cookieData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_cookieFilePath, json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save cookies: {ex.Message}");
        }
    }

    public void ClearCookies()
    {
        lock (_lock)
        {
            _cookies.Clear();
        }

        if (File.Exists(_cookieFilePath))
        {
            File.Delete(_cookieFilePath);
        }
    }

    private class CookieData
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTime Expires { get; set; }
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
    }
}
```

## Session Management with CSRF Protection

```csharp
using CurlDotNet;
using HtmlAgilityPack;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package HtmlAgilityPack
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class SessionManager
{
    private readonly Curl _curl;
    private readonly CookieContainer _cookies;
    private string _csrfToken;
    private string _sessionId;

    public SessionManager(string baseUrl)
    {
        _cookies = new CookieContainer();
        _curl = new Curl(baseUrl)
            .WithCookieContainer(_cookies)
            .WithHeader("User-Agent", "MyApp/1.0 CurlDotNet");
    }

    public async Task<bool> InitializeSessionAsync()
    {
        // Get initial page to obtain CSRF token and session cookie
        var response = await _curl.GetAsync("/");

        if (response.StatusCode != 200)
        {
            return false;
        }

        // Extract CSRF token from HTML
        _csrfToken = ExtractCsrfToken(response.Body);

        // Extract session ID from cookies
        var cookies = _cookies.GetCookies(new Uri(_curl.BaseUrl));
        var sessionCookie = cookies["session_id"] ?? cookies["PHPSESSID"] ?? cookies["ASP.NET_SessionId"];
        _sessionId = sessionCookie?.Value;

        Console.WriteLine($"Session initialized: {_sessionId}");
        Console.WriteLine($"CSRF Token: {_csrfToken}");

        return !string.IsNullOrEmpty(_sessionId);
    }

    public async Task<bool> LoginWithCsrfAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(_csrfToken))
        {
            await InitializeSessionAsync();
        }

        var response = await _curl.PostAsync("/login")
            .WithFormData(new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                ["csrf_token"] = _csrfToken,
                ["remember_me"] = "1"
            })
            .ExecuteAsync();

        if (response.StatusCode == 200 || response.StatusCode == 302)
        {
            // Update CSRF token if provided in response
            var newToken = ExtractCsrfToken(response.Body);
            if (!string.IsNullOrEmpty(newToken))
            {
                _csrfToken = newToken;
            }

            return true;
        }

        return false;
    }

    public async Task<T> PostWithCsrfAsync<T>(string endpoint, object data)
    {
        var formData = ObjectToDictionary(data);
        formData["csrf_token"] = _csrfToken;

        var response = await _curl.PostAsync(endpoint)
            .WithFormData(formData)
            .ExecuteAsync();

        // Handle CSRF token rotation
        if (response.StatusCode == 403 &&
            response.Body.Contains("csrf", StringComparison.OrdinalIgnoreCase))
        {
            // Token expired, refresh and retry
            await InitializeSessionAsync();
            formData["csrf_token"] = _csrfToken;

            response = await _curl.PostAsync(endpoint)
                .WithFormData(formData)
                .ExecuteAsync();
        }

        return JsonSerializer.Deserialize<T>(response.Body);
    }

    private string ExtractCsrfToken(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Try common CSRF token locations
        var tokenNode = doc.DocumentNode.SelectSingleNode("//meta[@name='csrf-token']");
        if (tokenNode != null)
        {
            return tokenNode.GetAttributeValue("content", "");
        }

        tokenNode = doc.DocumentNode.SelectSingleNode("//input[@name='csrf_token']");
        if (tokenNode != null)
        {
            return tokenNode.GetAttributeValue("value", "");
        }

        // Try to find in JavaScript
        var match = System.Text.RegularExpressions.Regex.Match(
            html,
            @"csrf[_-]?token['""\s]*[:=]['""\s]*([a-zA-Z0-9+/=]+)");

        return match.Success ? match.Groups[1].Value : null;
    }

    private Dictionary<string, string> ObjectToDictionary(object obj)
    {
        return obj.GetType()
            .GetProperties()
            .ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(obj)?.ToString() ?? "");
    }
}
```

## Real-World Example: E-commerce Session

```csharp
using CurlDotNet;
using System.Text.Json;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class EcommerceSessionClient
{
    private readonly Curl _curl;
    private readonly CookieContainer _cookies;
    private readonly string _baseUrl;
    private bool _isAuthenticated;
    private ShoppingCart _cart;

    public class ShoppingCart
    {
        public string CartId { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal Total { get; set; }
        public string Currency { get; set; }
    }

    public class CartItem
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public EcommerceSessionClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _cookies = new CookieContainer();
        _curl = new Curl(baseUrl)
            .WithCookieContainer(_cookies)
            .WithHeader("Accept", "application/json")
            .WithHeader("X-Requested-With", "XMLHttpRequest");
    }

    public async Task<bool> StartSessionAsync()
    {
        // Initialize anonymous session
        var response = await _curl.GetAsync("/api/session/start");

        if (response.StatusCode == 200)
        {
            var session = JsonSerializer.Deserialize<SessionInfo>(response.Body);
            _cart = new ShoppingCart { CartId = session.CartId };

            Console.WriteLine($"Session started with cart: {_cart.CartId}");
            return true;
        }

        return false;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _curl.PostAsync("/api/auth/login")
            .WithJson(new { email, password })
            .ExecuteAsync();

        if (response.StatusCode == 200)
        {
            _isAuthenticated = true;

            // Merge anonymous cart with user cart
            await MergeCartAsync();

            return true;
        }

        return false;
    }

    public async Task<bool> AddToCartAsync(string productId, int quantity = 1)
    {
        var response = await _curl.PostAsync("/api/cart/add")
            .WithJson(new
            {
                productId,
                quantity,
                cartId = _cart.CartId
            })
            .ExecuteAsync();

        if (response.StatusCode == 200)
        {
            _cart = JsonSerializer.Deserialize<ShoppingCart>(response.Body);
            Console.WriteLine($"Added to cart. Total items: {_cart.Items.Count}, " +
                $"Total: {_cart.Currency} {_cart.Total}");
            return true;
        }

        return false;
    }

    public async Task<ShoppingCart> GetCartAsync()
    {
        var response = await _curl.GetAsync($"/api/cart/{_cart.CartId}");

        if (response.StatusCode == 200)
        {
            _cart = JsonSerializer.Deserialize<ShoppingCart>(response.Body);
        }

        return _cart;
    }

    public async Task<bool> UpdateQuantityAsync(string productId, int newQuantity)
    {
        var response = await _curl.PutAsync($"/api/cart/item/{productId}")
            .WithJson(new { quantity = newQuantity })
            .ExecuteAsync();

        if (response.StatusCode == 200)
        {
            _cart = JsonSerializer.Deserialize<ShoppingCart>(response.Body);
            return true;
        }

        return false;
    }

    public async Task<CheckoutSession> StartCheckoutAsync()
    {
        if (!_isAuthenticated)
        {
            throw new InvalidOperationException("Must be logged in to checkout");
        }

        var response = await _curl.PostAsync("/api/checkout/start")
            .WithJson(new { cartId = _cart.CartId })
            .ExecuteAsync();

        if (response.StatusCode == 200)
        {
            var checkout = JsonSerializer.Deserialize<CheckoutSession>(response.Body);

            // Store checkout session cookie
            Console.WriteLine($"Checkout session: {checkout.SessionId}");
            return checkout;
        }

        throw new Exception($"Failed to start checkout: {response.Body}");
    }

    public async Task<Order> CompleteCheckoutAsync(
        CheckoutSession session,
        PaymentInfo payment,
        ShippingAddress shipping)
    {
        // Ensure we have the checkout session cookie
        var response = await _curl.PostAsync("/api/checkout/complete")
            .WithJson(new
            {
                sessionId = session.SessionId,
                payment,
                shipping,
                cartId = _cart.CartId
            })
            .ExecuteAsync();

        if (response.StatusCode == 200)
        {
            var order = JsonSerializer.Deserialize<Order>(response.Body);

            // Clear cart after successful order
            _cart = new ShoppingCart();

            return order;
        }

        throw new Exception($"Checkout failed: {response.Body}");
    }

    private async Task MergeCartAsync()
    {
        // Merge anonymous cart with user's saved cart
        var response = await _curl.PostAsync("/api/cart/merge")
            .WithJson(new { anonymousCartId = _cart.CartId })
            .ExecuteAsync();

        if (response.StatusCode == 200)
        {
            _cart = JsonSerializer.Deserialize<ShoppingCart>(response.Body);
            Console.WriteLine($"Cart merged. Total items: {_cart.Items.Count}");
        }
    }

    public void SaveSessionToDisk(string filePath)
    {
        var sessionData = new SessionData
        {
            BaseUrl = _baseUrl,
            IsAuthenticated = _isAuthenticated,
            Cart = _cart,
            Cookies = ExtractCookies()
        };

        var json = JsonSerializer.Serialize(sessionData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
    }

    public void LoadSessionFromDisk(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var sessionData = JsonSerializer.Deserialize<SessionData>(json);

            _isAuthenticated = sessionData.IsAuthenticated;
            _cart = sessionData.Cart;

            // Restore cookies
            foreach (var cookieData in sessionData.Cookies)
            {
                var cookie = new Cookie(
                    cookieData.Name,
                    cookieData.Value,
                    cookieData.Path,
                    cookieData.Domain);

                _cookies.Add(cookie);
            }
        }
    }

    private List<CookieData> ExtractCookies()
    {
        var cookies = new List<CookieData>();
        var uri = new Uri(_baseUrl);

        foreach (Cookie cookie in _cookies.GetCookies(uri))
        {
            cookies.Add(new CookieData
            {
                Name = cookie.Name,
                Value = cookie.Value,
                Domain = cookie.Domain,
                Path = cookie.Path
            });
        }

        return cookies;
    }

    // Data models
    private class SessionInfo
    {
        public string SessionId { get; set; }
        public string CartId { get; set; }
    }

    public class CheckoutSession
    {
        public string SessionId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class PaymentInfo
    {
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvv { get; set; }
    }

    public class ShippingAddress
    {
        public string FullName { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }

    private class SessionData
    {
        public string BaseUrl { get; set; }
        public bool IsAuthenticated { get; set; }
        public ShoppingCart Cart { get; set; }
        public List<CookieData> Cookies { get; set; }
    }

    private class CookieData
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
    }
}
```

## Usage Examples

### Basic Cookie Usage

```csharp
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

var client = new BasicCookieClient("https://example.com");

// Login - cookies stored automatically
await client.LoginAsync("user@example.com", "password");

// Make authenticated requests - cookies sent automatically
var data = await client.GetProtectedDataAsync();

// View current cookies
client.PrintCookies();
```

### E-commerce Session

```csharp
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

var shop = new EcommerceSessionClient("https://shop.example.com");

// Start anonymous session
await shop.StartSessionAsync();

// Add items to cart
await shop.AddToCartAsync("product-123", 2);
await shop.AddToCartAsync("product-456", 1);

// Login to continue
await shop.LoginAsync("customer@example.com", "password");

// Complete purchase
var checkout = await shop.StartCheckoutAsync();
var order = await shop.CompleteCheckoutAsync(
    checkout,
    new PaymentInfo { /* ... */ },
    new ShippingAddress { /* ... */ });

Console.WriteLine($"Order placed: {order.OrderId}");
```

### Persistent Session

```csharp
// Save session to disk
var client = new EcommerceSessionClient("https://shop.example.com");
await client.LoginAsync("user@example.com", "password");
client.SaveSessionToDisk("session.json");

// Later: restore session
var restoredClient = new EcommerceSessionClient("https://shop.example.com");
restoredClient.LoadSessionFromDisk("session.json");
// Continue where you left off
```

## Best Practices

### 1. Secure Cookie Storage
```csharp
// Encrypt cookies when storing to disk
var encryptedCookies = Encrypt(cookieData, userKey);
File.WriteAllText(path, encryptedCookies);
```

### 2. Handle Cookie Expiration
```csharp
// Check and refresh expired sessions
if (cookie.Expires < DateTime.UtcNow)
{
    await RefreshSessionAsync();
}
```

### 3. Clean Up Old Cookies
```csharp
// Remove expired cookies periodically
_cookies.RemoveAll(c => c.Expired);
```

### 4. Use Secure Flags
```csharp
// Ensure cookies are secure in production
if (IsProduction)
{
    cookie.Secure = true;
    cookie.HttpOnly = true;
    cookie.SameSite = SameSiteMode.Strict;
}
```

## Troubleshooting

### Cookies Not Being Sent
- Check domain and path match
- Verify cookies haven't expired
- Ensure secure flag matches HTTPS usage

### Session Lost Between Requests
- Verify cookie container is reused
- Check if server requires specific headers
- Ensure cookies are being saved

### CSRF Token Errors
- Token may expire between requests
- Check if token format is correct
- Verify token is sent in correct field

## Key Takeaways

- ✅ Use CookieContainer for automatic cookie management
- ✅ Persist cookies for long-lived sessions
- ✅ Handle CSRF tokens properly
- ✅ Implement session merging for anonymous to authenticated flow
- ✅ Secure cookie storage with encryption
- ✅ Clean up expired cookies regularly
- ✅ Test cookie handling across domains

## Related Examples

- [Authentication Basics](../../tutorials/authentication-basics)
- [API Client Class](./api-client-class)
- [Form Submission](../beginner/post-form)
- [Security Best Practices](../../guides/security)

---

*Part of the CurlDotNet Cookbook - Professional patterns for C# and .NET developers*