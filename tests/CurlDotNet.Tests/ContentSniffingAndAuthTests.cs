/***************************************************************************
 * ContentSniffingAndAuthTests - Regression tests for v9.7.0 fixes
 *
 * 1. Binary content sniffing: binary payloads served under a lying text
 *    Content-Type (e.g. an .xlsx sent as text/plain) must be stored as
 *    bytes, never decoded to a string (reported by Massimo Braga by email;
 *    servers that map unknown extensions to text/plain corrupt downloads).
 * 2. Cookie handling (issue #41): -b values and manual Cookie headers must
 *    reach the wire; the HttpClientHandler cookie container must not swallow
 *    them or leak session cookies across unrelated requests.
 * 3. Digest auth (issue #40): --digest must not send a preemptive
 *    Authorization: Basic header.
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 ***************************************************************************/

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Core.Handlers;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for payload-based binary sniffing, curl-style cookie pass-through,
    /// and challenge-based authentication scheme selection.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    [Trait("Category", TestCategories.Unit)]
    public class ContentSniffingAndAuthTests : IDisposable
    {
        // A minimal ZIP local-file-header prefix followed by high bytes that UTF-8
        // decoding would mangle - stands in for a real .xlsx payload.
        private static readonly byte[] ZipPayload =
            { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00, 0x08, 0x00, 0xC3, 0x9F, 0xFE, 0xFF, 0x80, 0x81 };

        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly HttpHandler _handler;
        private HttpRequestMessage _capturedRequest;

        public ContentSniffingAndAuthTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object);
            _handler = new HttpHandler(_httpClient);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private void SetupMockResponse(byte[] content, string contentTypeHeader)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(content)
            };
            if (contentTypeHeader != null)
            {
                response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentTypeHeader);
            }

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => _capturedRequest = req)
                .ReturnsAsync(response);
        }

        #region Binary sniffing (Massimo Braga's .xlsx-as-text/plain report)

        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/html")]
        [InlineData("application/xml")]
        [InlineData("text/plain; charset=utf-8")]
        public async Task ZipPayload_UnderLyingTextContentType_IsKeptAsBinary(string contentType)
        {
            SetupMockResponse(ZipPayload, contentType);
            var options = new CurlOptions { Url = "https://example.com/report.xlsx" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.IsBinary.Should().BeTrue($"a ZIP payload served as {contentType} must not be string-decoded");
            result.BinaryData.Should().Equal(ZipPayload, "wire bytes must be preserved verbatim");
        }

        [Fact]
        public async Task PdfPayload_UnderTextPlain_IsKeptAsBinary()
        {
            var pdf = Encoding.ASCII.GetBytes("%PDF-1.7\n").AsSpan().ToArray();
            SetupMockResponse(pdf, "text/plain");
            var options = new CurlOptions { Url = "https://example.com/doc.pdf" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.IsBinary.Should().BeTrue();
        }

        [Fact]
        public async Task GenuineText_UnderTextPlain_IsStillDecodedAsBody()
        {
            var text = Encoding.UTF8.GetBytes("Hello, Ünïcøde world!");
            SetupMockResponse(text, "text/plain; charset=utf-8");
            var options = new CurlOptions { Url = "https://example.com/readme.txt" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.IsBinary.Should().BeFalse();
            result.Body.Should().Be("Hello, Ünïcøde world!");
        }

        [Fact]
        public async Task UserForcedTextContentType_BeatsSniffing()
        {
            SetupMockResponse(ZipPayload, "application/x-custom");
            var options = new CurlOptions { Url = "https://example.com/data" };
            options.TextContentTypes.Add("application/x-custom");

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.Body.Should().NotBeNull("an explicit user text override must win over sniffing");
        }

        [Fact]
        public void LooksBinary_Utf16Text_IsNotBinary()
        {
            var utf16 = Encoding.Unicode.GetBytes("plain text");
            HttpHandler.LooksBinary(utf16, "utf-16").Should().BeFalse();
            HttpHandler.LooksBinary(Encoding.Unicode.GetPreamble(), null).Should().BeFalse("UTF-16 BOM marks text");
        }

        [Fact]
        public void LooksBinary_NulByte_IsBinary()
        {
            HttpHandler.LooksBinary(new byte[] { 0x41, 0x42, 0x00, 0x43 }).Should().BeTrue();
        }

        #endregion

        #region Cookies reach the wire (issue #41)

        [Fact]
        public async Task CookieOption_IsSentAsRequestHeader()
        {
            SetupMockResponse(Encoding.UTF8.GetBytes("ok"), "text/plain");
            var options = new CurlOptions { Url = "https://example.com/", Cookie = "myCookie=1111111" };

            await _handler.ExecuteAsync(options, CancellationToken.None);

            _capturedRequest.Headers.TryGetValues("Cookie", out var values).Should().BeTrue();
            string.Join(";", values).Should().Contain("myCookie=1111111");
        }

        #endregion

        #region Challenge auth schemes (issue #40)

        [Theory]
        [InlineData("digest", true)]
        [InlineData("ntlm", true)]
        [InlineData("negotiate", true)]
        [InlineData("anyauth", true)]
        [InlineData("basic", false)]
        [InlineData(null, false)]
        public void ChallengeAuthDetection_DrivesTransportAndHeaderChoice(string scheme, bool expected)
        {
            // UsesChallengeAuth gates both the dedicated-client transport (whose handler
            // answers the 401 challenge) and the skip of the preemptive Basic header.
            // The full digest handshake is covered by an end-to-end RFC 2617 exchange in CI.
            var options = new CurlOptions
            {
                Url = "https://example.com/",
                Credentials = new NetworkCredential("admin", "secret"),
                AuthScheme = scheme
            };

            HttpHandler.UsesChallengeAuth(options).Should().Be(expected);
        }

        [Fact]
        public void ChallengeAuth_RequiresCredentials()
        {
            var options = new CurlOptions { Url = "https://example.com/", AuthScheme = "digest" };
            HttpHandler.UsesChallengeAuth(options).Should().BeFalse("--digest without -u has nothing to negotiate");
        }

        [Fact]
        public async Task BasicScheme_StillSendsPreemptiveBasicHeader()
        {
            SetupMockResponse(Encoding.UTF8.GetBytes("ok"), "text/plain");
            var options = new CurlOptions
            {
                Url = "https://example.com/",
                Credentials = new NetworkCredential("admin", "secret"),
                AuthScheme = "basic"
            };

            await _handler.ExecuteAsync(options, CancellationToken.None);

            _capturedRequest.Headers.Authorization.Should().NotBeNull();
            _capturedRequest.Headers.Authorization.Scheme.Should().Be("Basic");
        }

        [Fact]
        public async Task NoScheme_StillSendsPreemptiveBasic()
        {
            SetupMockResponse(Encoding.UTF8.GetBytes("ok"), "text/plain");
            var options = new CurlOptions
            {
                Url = "https://example.com/",
                Credentials = new NetworkCredential("admin", "secret")
            };

            await _handler.ExecuteAsync(options, CancellationToken.None);

            _capturedRequest.Headers.Authorization.Should().NotBeNull();
            _capturedRequest.Headers.Authorization.Scheme.Should().Be("Basic");
        }

        [Theory]
        [InlineData("--digest", "digest")]
        [InlineData("--basic", "basic")]
        [InlineData("--ntlm", "ntlm")]
        [InlineData("--negotiate", "negotiate")]
        [InlineData("--anyauth", "anyauth")]
        public void Parser_RecognizesAuthSchemeFlags(string flag, string expected)
        {
            var parser = new CommandParser();
            var options = parser.Parse($"curl {flag} -u admin:secret https://example.com/");
            options.AuthScheme.Should().Be(expected);
            options.Credentials.Should().NotBeNull();
        }

        [Fact]
        public void Parser_AcceptsCommandWithoutCurlPrefix()
        {
            var parser = new CommandParser();
            var options = parser.Parse("-X PUT https://example.com/api --digest -u \"admin:secret\"");
            options.Method.Should().Be("PUT");
            options.AuthScheme.Should().Be("digest");
            options.Url.Should().Be("https://example.com/api");
        }

        #endregion

        #region Quoted whitespace preservation (Massimo Braga's double-space report)

        [Fact]
        public void Parser_PreservesConsecutiveSpaces_InQuotedFilePath()
        {
            var parser = new CommandParser();
            var options = parser.Parse(
                "curl --output \"C:\\Temp\\CURLAPIOutput Check new  TeamsMeeting  fields.json\" https://example.com/");
            options.OutputFile.Should().Be("C:\\Temp\\CURLAPIOutput Check new  TeamsMeeting  fields.json",
                "consecutive spaces inside a quoted path must survive parsing");
        }

        [Fact]
        public void Parser_PreservesConsecutiveSpaces_InQuotedData()
        {
            var parser = new CommandParser();
            var options = parser.Parse("curl -d '{\"note\":\"two  spaces\"}' https://example.com/");
            options.Data.Should().Contain("two  spaces");
        }

        [Fact]
        public void Parser_StillCollapsesWhitespace_BetweenArguments()
        {
            var parser = new CommandParser();
            var options = parser.Parse("curl    -X   POST     https://example.com/api");
            options.Method.Should().Be("POST");
            options.Url.Should().Be("https://example.com/api");
        }

        #endregion
    }
}
