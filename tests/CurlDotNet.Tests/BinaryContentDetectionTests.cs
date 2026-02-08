/***************************************************************************
 * BinaryContentDetectionTests - Tests for binary vs text content detection
 *
 * Verifies that the IsTextContent method correctly identifies binary
 * content types, especially Office Open XML formats (.xlsx, .docx, .pptx)
 * that contain "xml" in their MIME type string but are binary data.
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for binary content type detection in HttpHandler.
    /// Ensures that Office documents, PDFs, archives, and other binary formats
    /// are correctly detected and not corrupted by text encoding.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    [Trait("Category", TestCategories.Unit)]
    public class BinaryContentDetectionTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly HttpHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryContentDetectionTests"/> class.
        /// </summary>
        public BinaryContentDetectionTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object);
            _handler = new HttpHandler(_httpClient);
        }

        /// <summary>
        /// Disposes the HTTP client and handler.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private void SetupMockResponse(byte[] content, string mediaType)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(content)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        private void SetupMockResponseNoContentType(byte[] content)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(content)
            };
            response.Content.Headers.ContentType = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        #region Office Open XML Formats (THE BUG FIX)

        /// <summary>
        /// Regression test: .xlsx files must be treated as binary.
        /// The MIME type contains "xml" in "openxmlformats" which previously
        /// caused false positive text detection.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_XlsxContentType_TreatedAsBinary()
        {
            // Arrange - Fake .xlsx data (in real life this would be a ZIP file)
            var binaryData = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0xFF, 0xFE, 0x00, 0x01 };
            SetupMockResponse(binaryData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var options = new CurlOptions { Url = "https://example.com/report.xlsx" };

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.BinaryData.Should().NotBeNull("xlsx files must be treated as binary");
            result.BinaryData.Should().BeEquivalentTo(binaryData, "binary data must not be corrupted");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Regression test: .docx files must be treated as binary.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_DocxContentType_TreatedAsBinary()
        {
            var binaryData = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0xAB, 0xCD };
            SetupMockResponse(binaryData, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            var options = new CurlOptions { Url = "https://example.com/document.docx" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("docx files must be treated as binary");
            result.BinaryData.Should().BeEquivalentTo(binaryData);
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Regression test: .pptx files must be treated as binary.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_PptxContentType_TreatedAsBinary()
        {
            var binaryData = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x12, 0x34 };
            SetupMockResponse(binaryData, "application/vnd.openxmlformats-officedocument.presentationml.presentation");

            var options = new CurlOptions { Url = "https://example.com/slides.pptx" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("pptx files must be treated as binary");
            result.BinaryData.Should().BeEquivalentTo(binaryData);
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Regression test: .xltx template files must be treated as binary.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_XltxContentType_TreatedAsBinary()
        {
            var binaryData = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x56, 0x78 };
            SetupMockResponse(binaryData, "application/vnd.openxmlformats-officedocument.spreadsheetml.template");

            var options = new CurlOptions { Url = "https://example.com/template.xltx" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("xltx files must be treated as binary");
            result.IsBinary.Should().BeTrue();
        }

        #endregion

        #region Text Content Types (Must Still Work)

        /// <summary>
        /// Ensures genuine XML content types are still treated as text.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_ApplicationXml_TreatedAsText()
        {
            var textContent = "<root><element>value</element></root>";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(textContent, System.Text.Encoding.UTF8, "application/xml")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions { Url = "https://example.com/data.xml" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.Body.Should().Contain("value", "genuine XML should be treated as text");
            result.BinaryData.Should().BeNull();
        }

        /// <summary>
        /// Ensures JSON content is treated as text.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_ApplicationJson_TreatedAsText()
        {
            var jsonContent = "{\"key\":\"value\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions { Url = "https://example.com/api/data" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.Body.Should().Contain("value");
            result.BinaryData.Should().BeNull();
        }

        /// <summary>
        /// Ensures text/html is treated as text.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_TextHtml_TreatedAsText()
        {
            var htmlContent = "<html><body>Hello</body></html>";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(htmlContent, System.Text.Encoding.UTF8, "text/html")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions { Url = "https://example.com/page.html" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.Body.Should().Contain("Hello");
            result.BinaryData.Should().BeNull();
        }

        /// <summary>
        /// Ensures SOAP+XML (a genuine XML variant) is treated as text.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_SoapPlusXml_TreatedAsText()
        {
            var soapContent = "<soap:Envelope></soap:Envelope>";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(soapContent, System.Text.Encoding.UTF8, "application/soap+xml")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions { Url = "https://example.com/soap" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.Body.Should().Contain("Envelope", "SOAP+XML is genuine text XML");
            result.BinaryData.Should().BeNull();
        }

        /// <summary>
        /// Ensures application/vnd.api+json (JSON API) is treated as text.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_JsonApiContentType_TreatedAsText()
        {
            var jsonContent = "{\"data\":[]}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/vnd.api+json")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions { Url = "https://example.com/api" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.Body.Should().Contain("data", "JSON API type should be text");
            result.BinaryData.Should().BeNull();
        }

        #endregion

        #region Common Binary Types

        /// <summary>
        /// PDF files must be treated as binary.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_PdfContentType_TreatedAsBinary()
        {
            var binaryData = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // %PDF-
            SetupMockResponse(binaryData, "application/pdf");

            var options = new CurlOptions { Url = "https://example.com/doc.pdf" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("PDF files must be binary");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// ZIP archives must be treated as binary.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_ZipContentType_TreatedAsBinary()
        {
            var binaryData = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // PK header
            SetupMockResponse(binaryData, "application/zip");

            var options = new CurlOptions { Url = "https://example.com/archive.zip" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("ZIP files must be binary");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// application/octet-stream must be treated as binary.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_OctetStream_TreatedAsBinary()
        {
            var binaryData = new byte[] { 0x00, 0x01, 0x02, 0xFF };
            SetupMockResponse(binaryData, "application/octet-stream");

            var options = new CurlOptions { Url = "https://example.com/file.bin" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("octet-stream must be binary");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Image types must be treated as binary.
        /// </summary>
        [Theory]
        [InlineData("image/jpeg")]
        [InlineData("image/png")]
        [InlineData("image/gif")]
        [InlineData("image/webp")]
        [InlineData("image/bmp")]
        public async Task ExecuteAsync_ImageContentTypes_TreatedAsBinary(string contentType)
        {
            var binaryData = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            SetupMockResponse(binaryData, contentType);

            var options = new CurlOptions { Url = "https://example.com/image" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull($"{contentType} must be binary");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Legacy Office formats must be treated as binary.
        /// </summary>
        [Theory]
        [InlineData("application/vnd.ms-excel")]
        [InlineData("application/msword")]
        [InlineData("application/vnd.ms-powerpoint")]
        public async Task ExecuteAsync_LegacyOfficeFormats_TreatedAsBinary(string contentType)
        {
            var binaryData = new byte[] { 0xD0, 0xCF, 0x11, 0xE0 }; // OLE header
            SetupMockResponse(binaryData, contentType);

            var options = new CurlOptions { Url = "https://example.com/document" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull($"{contentType} must be binary");
            result.IsBinary.Should().BeTrue();
        }

        #endregion

        #region Null Content-Type Handling

        /// <summary>
        /// When Content-Type is null/missing, default to binary (safe default).
        /// Binary read as text = corrupted. Text read as binary = lossless.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_NullContentType_DefaultsToBinary()
        {
            var data = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
            SetupMockResponseNoContentType(data);

            var options = new CurlOptions { Url = "https://example.com/unknown" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("null content type should default to binary");
            result.IsBinary.Should().BeTrue();
        }

        #endregion

        #region ForceBinary Override

        /// <summary>
        /// ForceBinary should override text detection and treat everything as binary.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_ForceBinary_OverridesTextDetection()
        {
            var textContent = "This is plain text";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(textContent, System.Text.Encoding.UTF8, "text/plain")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions
            {
                Url = "https://example.com/data",
                ForceBinary = true
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("ForceBinary should override text content type");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// ForceBinary should override even for application/json.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_ForceBinary_OverridesJsonDetection()
        {
            var jsonContent = "{\"key\":\"value\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions
            {
                Url = "https://example.com/api/data",
                ForceBinary = true
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("ForceBinary should force even JSON to binary");
            result.IsBinary.Should().BeTrue();
        }

        #endregion

        #region Custom Content Type Overrides

        /// <summary>
        /// Custom binary content types should override the default text detection.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_CustomBinaryContentType_TreatedAsBinary()
        {
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            // Use a content type that would normally be unknown (and thus binary)
            // but here we explicitly register it as binary
            SetupMockResponse(data, "application/x-custom-binary");

            var options = new CurlOptions
            {
                Url = "https://example.com/custom",
                BinaryContentTypes = new List<string> { "application/x-custom-binary" }
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("custom binary content types should be binary");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Custom text content types should override the default binary detection.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_CustomTextContentType_TreatedAsText()
        {
            var textContent = "custom text format data";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(textContent)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-custom-text");

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var options = new CurlOptions
            {
                Url = "https://example.com/custom",
                TextContentTypes = new List<string> { "application/x-custom-text" }
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.Body.Should().Contain("custom text", "custom text content types should be text");
            result.BinaryData.Should().BeNull();
        }

        /// <summary>
        /// BinaryContentTypes should take priority over TextContentTypes for the same MIME type.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_BinaryOverridesTakesPriorityOverText()
        {
            var data = new byte[] { 0x01, 0x02, 0x03 };
            SetupMockResponse(data, "application/x-ambiguous");

            var options = new CurlOptions
            {
                Url = "https://example.com/ambiguous",
                BinaryContentTypes = new List<string> { "application/x-ambiguous" },
                TextContentTypes = new List<string> { "application/x-ambiguous" }
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull("binary override takes priority over text override");
            result.IsBinary.Should().BeTrue();
        }

        #endregion

        #region CurlOptions Clone Tests

        /// <summary>
        /// Clone must preserve ForceBinary setting.
        /// </summary>
        [Fact]
        public void Clone_PreservesForceBinary()
        {
            var options = new CurlOptions { ForceBinary = true };
            var clone = options.Clone();
            clone.ForceBinary.Should().BeTrue();
        }

        /// <summary>
        /// Clone must create independent copies of BinaryContentTypes.
        /// </summary>
        [Fact]
        public void Clone_CopiesBinaryContentTypes()
        {
            var options = new CurlOptions();
            options.BinaryContentTypes.Add("application/x-test");

            var clone = options.Clone();

            clone.BinaryContentTypes.Should().Contain("application/x-test");

            // Modifying clone should not affect original
            clone.BinaryContentTypes.Add("application/x-other");
            options.BinaryContentTypes.Should().NotContain("application/x-other");
        }

        /// <summary>
        /// Clone must create independent copies of TextContentTypes.
        /// </summary>
        [Fact]
        public void Clone_CopiesTextContentTypes()
        {
            var options = new CurlOptions();
            options.TextContentTypes.Add("application/x-custom-text");

            var clone = options.Clone();

            clone.TextContentTypes.Should().Contain("application/x-custom-text");

            // Modifying clone should not affect original
            clone.TextContentTypes.Add("application/x-other-text");
            options.TextContentTypes.Should().NotContain("application/x-other-text");
        }

        #endregion

        #region CurlRequestBuilder Fluent API

        /// <summary>
        /// AsBinary() on the builder should set ForceBinary.
        /// </summary>
        [Fact]
        public void Builder_AsBinary_SetsForceBinaryInOptions()
        {
            var builder = CurlRequestBuilder.Get("https://example.com/file.xlsx")
                .AsBinary();

            var options = builder.GetOptions();

            options.ForceBinary.Should().BeTrue("AsBinary() should set ForceBinary to true");
        }

        /// <summary>
        /// WithBinaryContentType() on the builder should add to BinaryContentTypes.
        /// </summary>
        [Fact]
        public void Builder_WithBinaryContentType_AddsToList()
        {
            var builder = CurlRequestBuilder.Get("https://example.com/data")
                .WithBinaryContentType("application/x-custom-binary");

            var options = builder.GetOptions();

            options.BinaryContentTypes.Should().Contain("application/x-custom-binary");
        }

        /// <summary>
        /// WithTextContentType() on the builder should add to TextContentTypes.
        /// </summary>
        [Fact]
        public void Builder_WithTextContentType_AddsToList()
        {
            var builder = CurlRequestBuilder.Get("https://example.com/data")
                .WithTextContentType("application/x-custom-text");

            var options = builder.GetOptions();

            options.TextContentTypes.Should().Contain("application/x-custom-text");
        }

        /// <summary>
        /// Multiple content type overrides can be chained.
        /// </summary>
        [Fact]
        public void Builder_MultipleBinaryContentTypes_AllAdded()
        {
            var builder = CurlRequestBuilder.Get("https://example.com/data")
                .WithBinaryContentType("application/x-format1")
                .WithBinaryContentType("application/x-format2")
                .WithTextContentType("application/x-text1");

            var options = builder.GetOptions();

            options.BinaryContentTypes.Should().HaveCount(2);
            options.TextContentTypes.Should().HaveCount(1);
        }

        #endregion

        #region Audio/Video/Font Types

        /// <summary>
        /// Audio types must be treated as binary.
        /// </summary>
        [Theory]
        [InlineData("audio/mpeg")]
        [InlineData("audio/wav")]
        [InlineData("audio/ogg")]
        public async Task ExecuteAsync_AudioContentTypes_TreatedAsBinary(string contentType)
        {
            var binaryData = new byte[] { 0xFF, 0xFB, 0x90, 0x00 };
            SetupMockResponse(binaryData, contentType);

            var options = new CurlOptions { Url = "https://example.com/audio" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull($"{contentType} must be binary");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Video types must be treated as binary.
        /// </summary>
        [Theory]
        [InlineData("video/mp4")]
        [InlineData("video/webm")]
        [InlineData("video/quicktime")]
        public async Task ExecuteAsync_VideoContentTypes_TreatedAsBinary(string contentType)
        {
            var binaryData = new byte[] { 0x00, 0x00, 0x00, 0x18 };
            SetupMockResponse(binaryData, contentType);

            var options = new CurlOptions { Url = "https://example.com/video" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull($"{contentType} must be binary");
            result.IsBinary.Should().BeTrue();
        }

        /// <summary>
        /// Font types must be treated as binary.
        /// </summary>
        [Theory]
        [InlineData("font/woff")]
        [InlineData("font/woff2")]
        [InlineData("font/ttf")]
        public async Task ExecuteAsync_FontContentTypes_TreatedAsBinary(string contentType)
        {
            var binaryData = new byte[] { 0x77, 0x4F, 0x46, 0x46 };
            SetupMockResponse(binaryData, contentType);

            var options = new CurlOptions { Url = "https://example.com/font" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull($"{contentType} must be binary");
            result.IsBinary.Should().BeTrue();
        }

        #endregion

        #region Binary Data Integrity

        /// <summary>
        /// Verifies that binary data passes through without any byte modification.
        /// This is the critical test - if bytes are modified, files are corrupted.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_BinaryData_PreservesExactBytes()
        {
            // Create data with bytes that would be corrupted by text encoding
            // including null bytes, high bytes, and invalid UTF-8 sequences
            var binaryData = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                binaryData[i] = (byte)i;
            }

            SetupMockResponse(binaryData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var options = new CurlOptions { Url = "https://example.com/test.xlsx" };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            result.BinaryData.Should().NotBeNull();
            result.BinaryData.Length.Should().Be(256, "all 256 bytes must be preserved");
            for (int i = 0; i < 256; i++)
            {
                result.BinaryData[i].Should().Be((byte)i, $"byte at position {i} must be preserved exactly");
            }
        }

        #endregion
    }
}
