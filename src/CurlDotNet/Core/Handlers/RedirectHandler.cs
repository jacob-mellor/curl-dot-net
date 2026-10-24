using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core.Handlers
{
    internal class RedirectHandler
    {
        private readonly HttpClient _httpClient;

        public RedirectHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(HttpResponseMessage Response, HttpRequestMessage Request, int RedirectCount)> HandleRedirectAsync(
            HttpResponseMessage response,
            HttpRequestMessage initialRequest,
            CurlOptions options,
            CancellationToken cancellationToken,
            CurlTimings timings,
            DateTime startTime,
            StringBuilder? verboseLog,
            Func<CurlOptions, HttpRequestMessage> createRequestFunc,
            Action<StringBuilder?, HttpRequestMessage> appendVerboseRequestFunc,
            Action<StringBuilder?, HttpResponseMessage> appendVerboseResponseFunc,
            Action<HttpResponseMessage>? onRedirectResponse = null)
        {
            var redirectCount = 0;
            var currentResponse = response;
            var currentRequest = initialRequest;

            while (IsRedirect(currentResponse.StatusCode) && redirectCount < options.MaxRedirects)
            {
                // Let the caller record each intermediate response (curl -iL prints every hop's headers)
                onRedirectResponse?.Invoke(currentResponse);

                var location = currentResponse.Headers.Location;
                if (location == null)
                {
                    throw new CurlException("Redirect response missing Location header");
                }

                var newUrl = location.IsAbsoluteUri
                    ? location.ToString()
                    : new Uri(new Uri(options.Url), location).ToString();

                options.Url = newUrl;
                redirectCount++;

                currentRequest = createRequestFunc(options);
                appendVerboseRequestFunc(verboseLog, currentRequest);

                currentResponse = await _httpClient.SendAsync(currentRequest, cancellationToken);
                appendVerboseResponseFunc(verboseLog, currentResponse);

                timings.Redirect = (DateTime.UtcNow - startTime).TotalMilliseconds;
            }

            if (redirectCount >= options.MaxRedirects)
            {
                throw new CurlTooManyRedirectsException(redirectCount);
            }

            return (currentResponse, currentRequest, redirectCount);
        }

        public bool IsRedirect(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.MovedPermanently ||
                   statusCode == HttpStatusCode.Found ||
                   statusCode == HttpStatusCode.SeeOther ||
                   statusCode == HttpStatusCode.TemporaryRedirect ||
#if NETSTANDARD2_0 || NET472 || NET48
                   statusCode == (HttpStatusCode)308; // PermanentRedirect
#else
                   statusCode == HttpStatusCode.PermanentRedirect;
#endif
        }
    }
}
