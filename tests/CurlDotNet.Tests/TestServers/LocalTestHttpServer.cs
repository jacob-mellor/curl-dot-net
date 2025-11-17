using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CurlDotNet.Tests.TestServers
{
    /// <summary>
    /// A simple local HTTP server that mimics httpbin functionality for testing.
    /// This ensures tests are reliable and not dependent on external services.
    /// </summary>
    public class LocalTestHttpServer : IDisposable
    {
        private HttpListener _listener;
        private Task _listenerTask;
        private bool _isRunning;
        public string BaseUrl { get; private set; }
        public int Port { get; private set; }

        public LocalTestHttpServer(int? port = null)
        {
            // Find an available port if not specified
            Port = port ?? FindAvailablePort();
            BaseUrl = $"http://localhost:{Port}";
        }

        public void Start()
        {
            if (_isRunning) return;

            _listener = new HttpListener();
            _listener.Prefixes.Add($"{BaseUrl}/");
            _listener.Start();
            _isRunning = true;

            // Start listening in background
            _listenerTask = Task.Run(async () => await HandleRequestsAsync());
        }

        private async Task HandleRequestsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(async () => await HandleRequest(context));
                }
                catch (Exception) when (!_isRunning)
                {
                    // Expected when shutting down
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                var path = request.Url.AbsolutePath.ToLower();
                var method = request.HttpMethod;

                // Route to appropriate handler
                if (path == "/get")
                {
                    await HandleGet(request, response);
                }
                else if (path == "/post")
                {
                    await HandlePost(request, response);
                }
                else if (path == "/put")
                {
                    await HandlePut(request, response);
                }
                else if (path == "/headers")
                {
                    await HandleHeaders(request, response);
                }
                else if (path == "/cookies")
                {
                    await HandleCookies(request, response);
                }
                else if (path == "/anything" || path.StartsWith("/anything"))
                {
                    await HandleAnything(request, response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusDescription = "Not Found";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                var bytes = Encoding.UTF8.GetBytes($"{{\"error\":\"{ex.Message}\"}}");
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            }
            finally
            {
                response.Close();
            }
        }

        private async Task HandleGet(HttpListenerRequest request, HttpListenerResponse response)
        {
            var result = new Dictionary<string, object>
            {
                ["url"] = request.Url.ToString(),
                ["args"] = ParseQueryString(request.Url.Query),
                ["headers"] = GetHeaders(request)
            };

            await SendJsonResponse(response, result);
        }

        private async Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body = "";
            using (var reader = new StreamReader(request.InputStream))
            {
                body = await reader.ReadToEndAsync();
            }

            var result = new Dictionary<string, object>
            {
                ["url"] = request.Url.ToString(),
                ["data"] = body,
                ["headers"] = GetHeaders(request),
                ["form"] = new Dictionary<string, string>()
            };

            // If it's form data, parse it
            if (request.ContentType?.Contains("application/x-www-form-urlencoded") == true)
            {
                result["form"] = ParseQueryString(body);
                result["data"] = "";
            }

            await SendJsonResponse(response, result);
        }

        private async Task HandlePut(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body = "";
            using (var reader = new StreamReader(request.InputStream))
            {
                body = await reader.ReadToEndAsync();
            }

            var result = new Dictionary<string, object>
            {
                ["url"] = request.Url.ToString(),
                ["data"] = body,
                ["headers"] = GetHeaders(request),
                ["form"] = ParseQueryString(body)
            };

            await SendJsonResponse(response, result);
        }

        private async Task HandleHeaders(HttpListenerRequest request, HttpListenerResponse response)
        {
            var result = new Dictionary<string, object>
            {
                ["headers"] = GetHeaders(request)
            };

            await SendJsonResponse(response, result);
        }

        private async Task HandleCookies(HttpListenerRequest request, HttpListenerResponse response)
        {
            var cookies = new Dictionary<string, string>();

            // Parse cookie header
            var cookieHeader = request.Headers["Cookie"];
            if (!string.IsNullOrEmpty(cookieHeader))
            {
                var pairs = cookieHeader.Split(';');
                foreach (var pair in pairs)
                {
                    var parts = pair.Trim().Split('=');
                    if (parts.Length == 2)
                    {
                        cookies[parts[0]] = parts[1];
                    }
                }
            }

            var result = new Dictionary<string, object>
            {
                ["cookies"] = cookies
            };

            await SendJsonResponse(response, result);
        }

        private async Task HandleAnything(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body = "";
            if (request.HasEntityBody)
            {
                using (var reader = new StreamReader(request.InputStream))
                {
                    body = await reader.ReadToEndAsync();
                }
            }

            var result = new Dictionary<string, object>
            {
                ["url"] = request.Url.ToString(),
                ["method"] = request.HttpMethod,
                ["args"] = ParseQueryString(request.Url.Query),
                ["headers"] = GetHeaders(request),
                ["data"] = body
            };

            await SendJsonResponse(response, result);
        }

        private Dictionary<string, string> GetHeaders(HttpListenerRequest request)
        {
            var headers = new Dictionary<string, string>();
            foreach (string key in request.Headers.AllKeys)
            {
                headers[key] = request.Headers[key];
            }
            return headers;
        }

        private Dictionary<string, string> ParseQueryString(string query)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query)) return result;

            if (query.StartsWith("?"))
                query = query.Substring(1);

            var pairs = query.Split('&');
            foreach (var pair in pairs)
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    result[Uri.UnescapeDataString(parts[0])] = Uri.UnescapeDataString(parts[1]);
                }
            }
            return result;
        }

        private async Task SendJsonResponse(HttpListenerResponse response, object data)
        {
            response.ContentType = "application/json";
            response.StatusCode = 200;

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var bytes = Encoding.UTF8.GetBytes(json);
            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        }

        private int FindAvailablePort()
        {
            // Try to find an available port starting from 5001
            for (int port = 5001; port <= 5100; port++)
            {
                try
                {
                    var listener = new HttpListener();
                    listener.Prefixes.Add($"http://localhost:{port}/");
                    listener.Start();
                    listener.Stop();
                    return port;
                }
                catch
                {
                    // Port is in use, try next one
                }
            }
            throw new InvalidOperationException("Could not find an available port");
        }

        public void Dispose()
        {
            _isRunning = false;
            _listener?.Stop();
            _listener?.Close();
            _listenerTask?.Wait(TimeSpan.FromSeconds(2));
        }
    }

    /// <summary>
    /// Test fixture for httpbin integration tests using local server.
    /// </summary>
    public class LocalTestServerFixture : IAsyncLifetime
    {
        public LocalTestHttpServer Server { get; private set; }
        public string BaseUrl => Server?.BaseUrl ?? "";

        public async Task InitializeAsync()
        {
            Server = new LocalTestHttpServer();
            Server.Start();
            // Give server a moment to start
            await Task.Delay(100);
        }

        public async Task DisposeAsync()
        {
            Server?.Dispose();
            await Task.CompletedTask;
        }
    }
}