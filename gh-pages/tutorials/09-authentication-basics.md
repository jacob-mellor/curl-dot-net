# Tutorial 09: Authentication Basics

Learn how to authenticate your HTTP requests to access protected resources.

## 🔐 What is Authentication?

Authentication is how you prove who you are to a server. It's like showing your ID card to enter a building.

### Why Do APIs Need Authentication?

- **Security**: Protect sensitive data
- **Rate Limiting**: Control how much each user can access
- **Personalization**: Show user-specific content
- **Billing**: Track usage for paid services

## Types of Authentication

### 1. API Keys

The simplest form of authentication. Like a password that identifies your app.

```csharp
// Method 1: API key in header
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
    -H 'X-API-Key: your-api-key-here'
");

// Method 2: API key in query parameter
var result = await Curl.ExecuteAsync(@"
    curl 'https://api.example.com/data?api_key=your-api-key-here'
");
```

### Real-World Example: Weather API

```csharp
public class WeatherClient
{
    private readonly string _apiKey;

    public WeatherClient(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<Weather> GetWeather(string city)
    {
        var result = await Curl.ExecuteAsync($@"
            curl 'https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}'
        ");

        if (result.IsSuccess)
        {
            return result.ParseJson<Weather>();
        }

        throw new Exception($"Failed to get weather: {result.Error}");
    }
}

// Usage
var weather = new WeatherClient("your-api-key");
var forecast = await weather.GetWeather("London");
```

### 2. Bearer Tokens

Used by modern APIs. You get a token after logging in, then include it with each request.

```csharp
// Bearer token in Authorization header
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/protected \
    -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIs...'
");
```

### Getting a Bearer Token

```csharp
public class TokenAuth
{
    private string _token;
    private DateTime _expiresAt;

    public async Task<string> GetTokenAsync()
    {
        // Check if we have a valid token
        if (!string.IsNullOrEmpty(_token) && DateTime.UtcNow < _expiresAt)
        {
            return _token;
        }

        // Get new token
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://api.example.com/oauth/token \
            -d 'grant_type=client_credentials' \
            -d 'client_id=your-client-id' \
            -d 'client_secret=your-client-secret'
        ");

        if (result.IsSuccess)
        {
            dynamic tokenResponse = result.ParseJson();
            _token = tokenResponse.access_token;
            _expiresAt = DateTime.UtcNow.AddSeconds((int)tokenResponse.expires_in);
            return _token;
        }

        throw new Exception("Failed to get token");
    }

    public async Task<CurlResult> MakeAuthenticatedRequest(string endpoint)
    {
        var token = await GetTokenAsync();
        return await Curl.ExecuteAsync($@"
            curl https://api.example.com/{endpoint} \
            -H 'Authorization: Bearer {token}'
        ");
    }
}
```

### 3. Basic Authentication

Username and password sent with each request. Less secure but still common.

```csharp
// Method 1: Using -u flag
var result = await Curl.ExecuteAsync(@"
    curl -u username:password https://api.example.com/secure
");

// Method 2: Manual Basic auth header
var credentials = Convert.ToBase64String(
    Encoding.UTF8.GetBytes("username:password")
);
var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com/secure \
    -H 'Authorization: Basic {credentials}'
");
```

### Basic Auth Helper

```csharp
public class BasicAuthClient
{
    private readonly string _authHeader;

    public BasicAuthClient(string username, string password)
    {
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{username}:{password}")
        );
        _authHeader = $"Basic {credentials}";
    }

    public async Task<CurlResult> Get(string url)
    {
        return await Curl.ExecuteAsync($@"
            curl {url} \
            -H 'Authorization: {_authHeader}'
        ");
    }
}

// Usage
var client = new BasicAuthClient("john", "secret123");
var result = await client.Get("https://api.example.com/profile");
```

## OAuth 2.0 Flow

### Understanding OAuth

OAuth is like giving someone a temporary key to your house instead of your master key.

```csharp
public class OAuth2Client
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public OAuth2Client(string clientId, string clientSecret, string redirectUri)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _redirectUri = redirectUri;
    }

    // Step 1: Get authorization URL
    public string GetAuthorizationUrl()
    {
        return $"https://auth.example.com/authorize?" +
               $"client_id={_clientId}&" +
               $"redirect_uri={Uri.EscapeDataString(_redirectUri)}&" +
               $"response_type=code&" +
               $"scope=read write";
    }

    // Step 2: Exchange code for token
    public async Task<string> ExchangeCodeForToken(string code)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://auth.example.com/token \
            -d 'grant_type=authorization_code' \
            -d 'code={code}' \
            -d 'client_id={_clientId}' \
            -d 'client_secret={_clientSecret}' \
            -d 'redirect_uri={Uri.EscapeDataString(_redirectUri)}'
        ");

        if (result.IsSuccess)
        {
            dynamic response = result.ParseJson();
            return response.access_token;
        }

        throw new Exception("Failed to exchange code for token");
    }

    // Step 3: Refresh token when expired
    public async Task<string> RefreshToken(string refreshToken)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://auth.example.com/token \
            -d 'grant_type=refresh_token' \
            -d 'refresh_token={refreshToken}' \
            -d 'client_id={_clientId}' \
            -d 'client_secret={_clientSecret}'
        ");

        if (result.IsSuccess)
        {
            dynamic response = result.ParseJson();
            return response.access_token;
        }

        throw new Exception("Failed to refresh token");
    }
}
```

## Real-World Examples

### GitHub API Authentication

{% raw %}
```csharp
public class GitHubClient
{
    private readonly string _token;

    public GitHubClient(string personalAccessToken)
    {
        _token = personalAccessToken;
    }

    public async Task<List<Repository>> GetMyRepos()
    {
        var result = await Curl.ExecuteAsync($@"
            curl https://api.github.com/user/repos \
            -H 'Authorization: token {_token}' \
            -H 'Accept: application/vnd.github.v3+json'
        ");

        if (result.IsSuccess)
        {
            return result.ParseJson<List<Repository>>();
        }

        if (result.StatusCode == 401)
        {
            throw new Exception("Invalid token. Check your GitHub Personal Access Token.");
        }

        throw new Exception($"GitHub API error: {result.Error}");
    }

    public async Task CreateRepo(string name, bool isPrivate = false)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://api.github.com/user/repos \
            -H 'Authorization: token {_token}' \
            -H 'Accept: application/vnd.github.v3+json' \
            -d '{{
                ""name"": ""{name}"",
                ""private"": {isPrivate.ToString().ToLower()}
            }}'
        ");

        if (!result.IsSuccess)
        {
            throw new Exception($"Failed to create repo: {result.Content}");
        }
    }
}

public class Repository
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public bool Private { get; set; }
    public string HtmlUrl { get; set; }
}
```
{% endraw %}

### Twitter/X API Authentication

```csharp
public class TwitterClient
{
    private readonly string _bearerToken;

    public TwitterClient(string bearerToken)
    {
        _bearerToken = bearerToken;
    }

    public async Task<List<Tweet>> SearchTweets(string query)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var result = await Curl.ExecuteAsync($@"
            curl 'https://api.twitter.com/2/tweets/search/recent?query={encodedQuery}' \
            -H 'Authorization: Bearer {_bearerToken}'
        ");

        if (result.IsSuccess)
        {
            dynamic response = result.ParseJson();
            return response.data.ToObject<List<Tweet>>();
        }

        throw new Exception($"Twitter API error: {result.Content}");
    }

    public async Task<User> GetUser(string username)
    {
        var result = await Curl.ExecuteAsync($@"
            curl 'https://api.twitter.com/2/users/by/username/{username}' \
            -H 'Authorization: Bearer {_bearerToken}'
        ");

        if (result.IsSuccess)
        {
            dynamic response = result.ParseJson();
            return response.data.ToObject<User>();
        }

        throw new Exception($"Failed to get user: {result.Content}");
    }
}
```

## Secure Credential Storage

### Never Hardcode Credentials!

```csharp
// ❌ NEVER DO THIS
var result = await Curl.ExecuteAsync(@"
    curl -H 'X-API-Key: sk-1234567890abcdef' https://api.example.com
");

// ✅ Use Environment Variables
var apiKey = Environment.GetEnvironmentVariable("API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    throw new Exception("API_KEY environment variable not set");
}

var result = await Curl.ExecuteAsync($@"
    curl -H 'X-API-Key: {apiKey}' https://api.example.com
");
```

### Configuration File Approach

```csharp
public class ApiConfig
{
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public string BaseUrl { get; set; }

    public static ApiConfig Load()
    {
        // appsettings.json
        var json = File.ReadAllText("appsettings.json");
        return JsonSerializer.Deserialize<ApiConfig>(json);
    }
}

// appsettings.json (add to .gitignore!)
{
    "ApiKey": "your-key-here",
    "ApiSecret": "your-secret-here",
    "BaseUrl": "https://api.example.com"
}

// Usage
var config = ApiConfig.Load();
var result = await Curl.ExecuteAsync($@"
    curl {config.BaseUrl}/data \
    -H 'X-API-Key: {config.ApiKey}'
");
```

### Using .NET Secret Manager

```bash
# Initialize secret storage
dotnet user-secrets init

# Add secrets
dotnet user-secrets set "ApiKey" "your-api-key"
dotnet user-secrets set "ApiSecret" "your-api-secret"
```

```csharp
// In your code
var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

var configuration = builder.Build();
var apiKey = configuration["ApiKey"];

var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com \
    -H 'X-API-Key: {apiKey}'
");
```

## Handling Authentication Errors

### Common Auth Errors and Solutions

```csharp
public async Task<T> MakeAuthenticatedRequest<T>(string endpoint)
{
    int retryCount = 0;
    const int maxRetries = 2;

    while (retryCount <= maxRetries)
    {
        var token = await GetToken();
        var result = await Curl.ExecuteAsync($@"
            curl https://api.example.com/{endpoint} \
            -H 'Authorization: Bearer {token}'
        ");

        if (result.IsSuccess)
        {
            return result.ParseJson<T>();
        }

        // Handle specific auth errors
        switch (result.StatusCode)
        {
            case 401: // Unauthorized
                if (retryCount < maxRetries)
                {
                    // Token might be expired, refresh it
                    await RefreshToken();
                    retryCount++;
                    continue;
                }
                throw new Exception("Authentication failed. Check your credentials.");

            case 403: // Forbidden
                throw new Exception("You don't have permission to access this resource.");

            case 429: // Rate limited
                var retryAfter = result.Headers.GetValueOrDefault("Retry-After", "60");
                await Task.Delay(int.Parse(retryAfter) * 1000);
                retryCount++;
                continue;

            default:
                throw new Exception($"Request failed: {result.Error}");
        }
    }

    throw new Exception("Max retries exceeded");
}
```

## API Key Best Practices

### 1. Rotate Keys Regularly

```csharp
public class ApiKeyRotation
{
    private readonly List<string> _apiKeys;
    private int _currentKeyIndex = 0;

    public ApiKeyRotation(List<string> apiKeys)
    {
        _apiKeys = apiKeys;
    }

    public string GetCurrentKey()
    {
        return _apiKeys[_currentKeyIndex];
    }

    public void RotateKey()
    {
        _currentKeyIndex = (_currentKeyIndex + 1) % _apiKeys.Count;
    }

    public async Task<CurlResult> ExecuteWithRotation(string url)
    {
        var result = await Curl.ExecuteAsync($@"
            curl {url} \
            -H 'X-API-Key: {GetCurrentKey()}'
        ");

        // If rate limited, try next key
        if (result.StatusCode == 429)
        {
            RotateKey();
            return await Curl.ExecuteAsync($@"
                curl {url} \
                -H 'X-API-Key: {GetCurrentKey()}'
            ");
        }

        return result;
    }
}
```

### 2. Scope Keys Appropriately

```csharp
public class ScopedApiClient
{
    private readonly string _readOnlyKey;
    private readonly string _writeKey;
    private readonly string _adminKey;

    public async Task<CurlResult> ReadData(string endpoint)
    {
        // Use read-only key for GET requests
        return await Curl.ExecuteAsync($@"
            curl https://api.example.com/{endpoint} \
            -H 'X-API-Key: {_readOnlyKey}'
        ");
    }

    public async Task<CurlResult> WriteData(string endpoint, string data)
    {
        // Use write key for POST/PUT
        return await Curl.ExecuteAsync($@"
            curl -X POST https://api.example.com/{endpoint} \
            -H 'X-API-Key: {_writeKey}' \
            -d '{data}'
        ");
    }

    public async Task<CurlResult> DeleteData(string endpoint)
    {
        // Use admin key for DELETE
        return await Curl.ExecuteAsync($@"
            curl -X DELETE https://api.example.com/{endpoint} \
            -H 'X-API-Key: {_adminKey}'
        ");
    }
}
```

## Multi-Factor Authentication

### Adding Extra Security

```csharp
public class MfaClient
{
    public async Task<string> LoginWithMfa(string username, string password, string mfaCode)
    {
        // Step 1: Initial login
        var loginResult = await Curl.ExecuteAsync($@"
            curl -X POST https://api.example.com/login \
            -d 'username={username}' \
            -d 'password={password}'
        ");

        if (!loginResult.IsSuccess)
        {
            throw new Exception("Invalid username or password");
        }

        dynamic loginResponse = loginResult.ParseJson();
        var sessionToken = loginResponse.session_token;

        // Step 2: Verify MFA code
        var mfaResult = await Curl.ExecuteAsync($@"
            curl -X POST https://api.example.com/verify-mfa \
            -H 'X-Session-Token: {sessionToken}' \
            -d 'code={mfaCode}'
        ");

        if (!mfaResult.IsSuccess)
        {
            throw new Exception("Invalid MFA code");
        }

        dynamic mfaResponse = mfaResult.ParseJson();
        return mfaResponse.access_token;
    }
}
```

## Testing Authentication

### Mock Authentication for Testing

```csharp
public interface IAuthProvider
{
    Task<string> GetTokenAsync();
}

public class RealAuthProvider : IAuthProvider
{
    public async Task<string> GetTokenAsync()
    {
        // Real authentication logic
        var result = await Curl.ExecuteAsync("...");
        return result.ParseJson().access_token;
    }
}

public class MockAuthProvider : IAuthProvider
{
    public Task<string> GetTokenAsync()
    {
        // Return fake token for testing
        return Task.FromResult("mock-token-for-testing");
    }
}

// In your API client
public class ApiClient
{
    private readonly IAuthProvider _authProvider;

    public ApiClient(IAuthProvider authProvider)
    {
        _authProvider = authProvider;
    }

    public async Task<CurlResult> GetData()
    {
        var token = await _authProvider.GetTokenAsync();
        return await Curl.ExecuteAsync($@"
            curl https://api.example.com/data \
            -H 'Authorization: Bearer {token}'
        ");
    }
}

// Usage in tests
var mockAuth = new MockAuthProvider();
var client = new ApiClient(mockAuth);
var result = await client.GetData(); // Uses mock token
```

## 🎯 Key Takeaways

1. **Never hardcode credentials** - Use environment variables or secure storage
2. **Choose the right auth method** - API keys for simple, OAuth for complex
3. **Handle auth errors gracefully** - Refresh tokens, retry on 401
4. **Protect your keys** - Rotate regularly, use appropriate scopes
5. **Test authentication** - Use mocks for testing
6. **Monitor usage** - Track API calls and rate limits

## Common Authentication Patterns

### Pattern 1: Auto-Refreshing Token

```csharp
public class AutoRefreshClient
{
    private string _token;
    private DateTime _expiresAt;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private async Task<string> GetValidTokenAsync()
    {
        if (_token != null && DateTime.UtcNow < _expiresAt)
            return _token;

        await _refreshLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_token != null && DateTime.UtcNow < _expiresAt)
                return _token;

            // Refresh token
            await RefreshTokenAsync();
            return _token;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task RefreshTokenAsync()
    {
        // Refresh logic here
        _token = "new-token";
        _expiresAt = DateTime.UtcNow.AddHours(1);
    }
}
```

## Next Steps

Ready to work with files? Continue to [Tutorial 10: Files and Downloads](10-files-and-downloads.md)

---
*Part 9 of the CurlDotNet Tutorial Series*