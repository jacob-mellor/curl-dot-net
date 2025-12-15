# Tutorial 9: Authentication Basics

Most APIs require authentication to verify who you are and what you're allowed to access. This tutorial covers common authentication methods with CurlDotNet.

## Types of Authentication

### 1. API Key Authentication
The simplest form - a unique key identifies your application:

```csharp
var curl = new Curl();

// API key in header
curl.Headers.Add("X-API-Key", "your-api-key-here");

// Or in query parameter
var result = await curl.GetAsync("https://api.example.com/data?api_key=your-api-key");

// Or custom header name
curl.Headers.Add("Api-Token", "your-token-here");
```

### 2. Basic Authentication
Username and password encoded in base64:

```csharp
// Method 1: Manual encoding
var credentials = Convert.ToBase64String(
    Encoding.UTF8.GetBytes($"{username}:{password}")
);
curl.Headers.Add("Authorization", $"Basic {credentials}");

// Method 2: Helper method
curl.SetBasicAuth("username", "password");

var result = await curl.GetAsync("https://api.example.com/protected");
```

### 3. Bearer Token (OAuth 2.0 / JWT)
Modern token-based authentication:

```csharp
var curl = new Curl();
curl.Headers.Add("Authorization", $"Bearer {accessToken}");

var result = await curl.GetAsync("https://api.example.com/user/profile");
```

## OAuth 2.0 Flow

### Getting an Access Token
```csharp
public async Task<string> GetOAuthToken(string clientId, string clientSecret)
{
    var curl = new Curl();

    // Request token from OAuth server
    var tokenRequest = new Dictionary<string, string>
    {
        ["grant_type"] = "client_credentials",
        ["client_id"] = clientId,
        ["client_secret"] = clientSecret,
        ["scope"] = "read:data write:data"
    };

    var result = await curl.PostFormAsync(
        "https://auth.example.com/oauth/token",
        tokenRequest
    );

    if (result.IsSuccess)
    {
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(result.Data);
        return tokenResponse.AccessToken;
    }

    throw new AuthenticationException($"Failed to get token: {result.Error}");
}

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
```

### Using and Refreshing Tokens
```csharp
public class OAuthClient
{
    private readonly Curl _curl;
    private string _accessToken;
    private string _refreshToken;
    private DateTime _tokenExpiry;

    public OAuthClient()
    {
        _curl = new Curl();
    }

    public async Task AuthenticateAsync(string clientId, string clientSecret)
    {
        var tokenData = await GetTokenAsync(clientId, clientSecret);
        SetTokenData(tokenData);
    }

    private void SetTokenData(TokenResponse tokenData)
    {
        _accessToken = tokenData.AccessToken;
        _refreshToken = tokenData.RefreshToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn - 60); // Buffer
        _curl.Headers["Authorization"] = $"Bearer {_accessToken}";
    }

    public async Task<CurlResult> GetAsync(string url)
    {
        // Check if token needs refresh
        if (DateTime.UtcNow >= _tokenExpiry && !string.IsNullOrEmpty(_refreshToken))
        {
            await RefreshTokenAsync();
        }

        return await _curl.GetAsync(url);
    }

    private async Task RefreshTokenAsync()
    {
        var refreshRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = _refreshToken
        };

        var result = await _curl.PostFormAsync(
            "https://auth.example.com/oauth/token",
            refreshRequest
        );

        if (result.IsSuccess)
        {
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(result.Data);
            SetTokenData(tokenData);
        }
        else
        {
            throw new AuthenticationException("Failed to refresh token");
        }
    }
}
```

## JWT (JSON Web Tokens)

### Decoding JWT Tokens
```csharp
public class JwtInfo
{
    public static Dictionary<string, object> DecodeJwt(string token)
    {
        // JWT has three parts: header.payload.signature
        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid JWT format");

        // Decode the payload (second part)
        var payload = parts[1];

        // Add padding if needed
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var bytes = Convert.FromBase64String(payload);
        var json = Encoding.UTF8.GetString(bytes);

        return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    }

    public static bool IsTokenExpired(string token)
    {
        var payload = DecodeJwt(token);

        if (payload.TryGetValue("exp", out var exp))
        {
            var expiry = DateTimeOffset.FromUnixTimeSeconds(
                Convert.ToInt64(exp.ToString())
            ).UtcDateTime;

            return DateTime.UtcNow > expiry;
        }

        return false; // No expiry claim
    }
}

// Usage
var jwt = "eyJhbGciOiJIUzI1NiIs...";
if (JwtInfo.IsTokenExpired(jwt))
{
    Console.WriteLine("Token expired, need to refresh");
}
```

## Session-Based Authentication

### Login and Cookie Management
```csharp
public class SessionClient
{
    private readonly Curl _curl;
    private string _sessionCookie;

    public SessionClient()
    {
        _curl = new Curl();
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var loginData = new Dictionary<string, string>
        {
            ["username"] = username,
            ["password"] = password
        };

        var result = await _curl.PostFormAsync(
            "https://example.com/api/login",
            loginData
        );

        if (result.IsSuccess)
        {
            // Extract session cookie
            if (result.Headers.TryGetValue("Set-Cookie", out string setCookie))
            {
                // Parse cookie (simplified - real parsing is more complex)
                var cookieParts = setCookie.Split(';')[0];
                _sessionCookie = cookieParts;

                // Add cookie to future requests
                _curl.Headers["Cookie"] = _sessionCookie;
                return true;
            }
        }

        return false;
    }

    public async Task<CurlResult> GetProtectedDataAsync(string url)
    {
        if (string.IsNullOrEmpty(_sessionCookie))
            throw new InvalidOperationException("Not logged in");

        return await _curl.GetAsync(url);
    }

    public async Task LogoutAsync()
    {
        await _curl.PostAsync("https://example.com/api/logout", "");
        _sessionCookie = null;
        _curl.Headers.Remove("Cookie");
    }
}
```

## API Signature Authentication

### HMAC Signing
```csharp
public class HmacAuthClient
{
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly Curl _curl;

    public HmacAuthClient(string apiKey, string apiSecret)
    {
        _apiKey = apiKey;
        _apiSecret = apiSecret;
        _curl = new Curl();
    }

    public async Task<CurlResult> SendSignedRequest(string url, string method, string body = "")
    {
        // Create signature components
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString();

        // Create string to sign
        var stringToSign = $"{method}\n{url}\n{timestamp}\n{nonce}\n{body}";

        // Generate HMAC signature
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        var signature = Convert.ToBase64String(signatureBytes);

        // Add auth headers
        _curl.Headers["X-API-Key"] = _apiKey;
        _curl.Headers["X-Timestamp"] = timestamp;
        _curl.Headers["X-Nonce"] = nonce;
        _curl.Headers["X-Signature"] = signature;

        // Send request
        if (method == "GET")
            return await _curl.GetAsync(url);
        else if (method == "POST")
            return await _curl.PostAsync(url, body);

        throw new NotSupportedException($"Method {method} not supported");
    }
}
```

## Two-Factor Authentication (2FA)

### Handling 2FA Challenges
```csharp
public class TwoFactorAuthClient
{
    private readonly Curl _curl = new Curl();

    public async Task<bool> LoginWith2FA(string username, string password)
    {
        // Step 1: Initial login
        var loginResult = await _curl.PostJsonAsync(
            "https://api.example.com/login",
            new { username, password }
        );

        if (loginResult.StatusCode == HttpStatusCode.Accepted) // 202 - needs 2FA
        {
            // Parse 2FA challenge
            var challenge = JsonSerializer.Deserialize<TwoFactorChallenge>(loginResult.Data);

            // Get 2FA code from user
            Console.Write("Enter 2FA code: ");
            var code = Console.ReadLine();

            // Step 2: Submit 2FA code
            var verifyResult = await _curl.PostJsonAsync(
                "https://api.example.com/verify-2fa",
                new
                {
                    challengeId = challenge.ChallengeId,
                    code = code
                }
            );

            if (verifyResult.IsSuccess)
            {
                var authData = JsonSerializer.Deserialize<AuthResponse>(verifyResult.Data);
                _curl.Headers["Authorization"] = $"Bearer {authData.Token}";
                return true;
            }
        }

        return false;
    }

    class TwoFactorChallenge
    {
        public string ChallengeId { get; set; }
        public string Method { get; set; } // "sms", "totp", "email"
    }

    class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
```

## Secure Storage of Credentials

### Never Hard-Code Credentials
```csharp
// BAD - Never do this!
var apiKey = "sk_live_abcd1234efgh5678";

// GOOD - Use environment variables
var apiKey = Environment.GetEnvironmentVariable("API_KEY");

// GOOD - Use configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["ApiKey"];

// GOOD - Use secure storage
var apiKey = await SecureStorage.GetAsync("api_key");
```

### Token Storage
```csharp
public interface ITokenStorage
{
    Task SaveTokenAsync(string token);
    Task<string> GetTokenAsync();
    Task DeleteTokenAsync();
}

public class SecureTokenStorage : ITokenStorage
{
    private readonly string _storageKey = "auth_token";

    public async Task SaveTokenAsync(string token)
    {
        // Platform-specific secure storage
        #if WINDOWS
        // Use Windows Credential Manager
        #elif MACOS
        // Use macOS Keychain
        #elif LINUX
        // Use Secret Service API
        #else
        // Encrypted file fallback
        #endif
    }

    public async Task<string> GetTokenAsync()
    {
        // Retrieve from secure storage
        return await Task.FromResult("stored_token");
    }

    public async Task DeleteTokenAsync()
    {
        // Remove from secure storage
    }
}
```

## Authentication Patterns

### Automatic Retry with Fresh Token
```csharp
public class AuthenticatedApiClient
{
    private readonly Curl _curl = new Curl();
    private readonly ITokenProvider _tokenProvider;

    public AuthenticatedApiClient(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public async Task<T> ExecuteWithAuth<T>(
        Func<Curl, Task<CurlResult>> request,
        Func<string, T> parser)
    {
        // Try with current token
        var token = await _tokenProvider.GetTokenAsync();
        _curl.Headers["Authorization"] = $"Bearer {token}";

        var result = await request(_curl);

        // If unauthorized, refresh and retry
        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            token = await _tokenProvider.RefreshTokenAsync();
            _curl.Headers["Authorization"] = $"Bearer {token}";

            result = await request(_curl);
        }

        if (result.IsSuccess)
        {
            return parser(result.Data);
        }

        throw new ApiException(result.Error);
    }
}
```

## Best Practices

1. **Never hard-code credentials** - Use environment variables or secure storage
2. **Use HTTPS always** - Never send credentials over HTTP
3. **Implement token refresh** - Handle expiration gracefully
4. **Store tokens securely** - Use platform-specific secure storage
5. **Validate tokens** - Check expiration before use
6. **Use appropriate auth method** - Match the API's requirements
7. **Handle 401/403 properly** - Distinguish between auth and permissions
8. **Log auth events** - Track login attempts and failures
9. **Implement rate limiting** - Prevent brute force attacks
10. **Use MFA when available** - Add extra security layer

## Summary

Authentication is crucial for API security:
- Use API keys for simple scenarios
- Implement OAuth 2.0 for modern APIs
- Handle tokens properly with refresh logic
- Store credentials securely
- Build reusable authentication patterns

## What's Next?

Learn about [working with files and downloads](10-files-and-downloads.html) in the next tutorial.

---

[← Previous: Headers Explained](08-headers-explained.html) | [Next: Files and Downloads →](10-files-and-downloads.html)