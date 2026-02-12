using System;
using System.Net.Http;
using System.Text;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for --aws-sigv4 flag support (AWS Signature Version 4 authentication).
    /// Covers parsing, signing, and integration with the command parser.
    /// See: https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    [Trait("Category", TestCategories.Unit)]
    public class AwsSigV4Tests
    {
        private readonly CommandParser _parser;

        public AwsSigV4Tests()
        {
            _parser = new CommandParser();
        }

        #region AwsSigV4Config.Parse tests

        [Fact]
        public void AwsSigV4Config_Parse_AllComponents()
        {
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");

            config.Provider1.Should().Be("aws");
            config.Provider2.Should().Be("amz");
            config.Region.Should().Be("us-east-1");
            config.Service.Should().Be("s3");
        }

        [Fact]
        public void AwsSigV4Config_Parse_ProviderOnly_DefaultsProvider2ToProvider1()
        {
            var config = AwsSigV4Config.Parse("aws");

            config.Provider1.Should().Be("aws");
            config.Provider2.Should().Be("aws");
            config.Region.Should().BeEmpty();
            config.Service.Should().BeEmpty();
        }

        [Fact]
        public void AwsSigV4Config_Parse_TwoProviders()
        {
            var config = AwsSigV4Config.Parse("aws:amz");

            config.Provider1.Should().Be("aws");
            config.Provider2.Should().Be("amz");
            config.Region.Should().BeEmpty();
            config.Service.Should().BeEmpty();
        }

        [Fact]
        public void AwsSigV4Config_Parse_ThreeComponents()
        {
            var config = AwsSigV4Config.Parse("aws:amz:eu-west-1");

            config.Provider1.Should().Be("aws");
            config.Provider2.Should().Be("amz");
            config.Region.Should().Be("eu-west-1");
            config.Service.Should().BeEmpty();
        }

        [Fact]
        public void AwsSigV4Config_Parse_GcpProvider()
        {
            var config = AwsSigV4Config.Parse("gcp:goog:us-central1:storage");

            config.Provider1.Should().Be("gcp");
            config.Provider2.Should().Be("goog");
            config.Region.Should().Be("us-central1");
            config.Service.Should().Be("storage");
        }

        [Fact]
        public void AwsSigV4Config_Parse_CustomProvider()
        {
            // Any arbitrary provider works (curl supports this)
            var config = AwsSigV4Config.Parse("middleearth:gondor:shire:hobbiton");

            config.Provider1.Should().Be("middleearth");
            config.Provider2.Should().Be("gondor");
            config.Region.Should().Be("shire");
            config.Service.Should().Be("hobbiton");
        }

        [Fact]
        public void AwsSigV4Config_Parse_EmptyProvider2_DefaultsToProvider1()
        {
            var config = AwsSigV4Config.Parse("gcp::us-central1:storage");

            config.Provider1.Should().Be("gcp");
            config.Provider2.Should().Be("gcp");
            config.Region.Should().Be("us-central1");
            config.Service.Should().Be("storage");
        }

        [Fact]
        public void AwsSigV4Config_Parse_NullValue_ThrowsArgumentException()
        {
            Action act = () => AwsSigV4Config.Parse(null);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AwsSigV4Config_Parse_EmptyValue_ThrowsArgumentException()
        {
            Action act = () => AwsSigV4Config.Parse("");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AwsSigV4Config_Parse_WhitespaceValue_ThrowsArgumentException()
        {
            Action act = () => AwsSigV4Config.Parse("  ");
            act.Should().Throw<ArgumentException>();
        }

        #endregion

        #region CommandParser --aws-sigv4 flag parsing

        [Fact]
        public void Parse_AwsSigV4Flag_SetsConfig()
        {
            var command = "curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u \"AKID:SECRET\" https://s3.amazonaws.com/bucket";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("aws");
            options.AwsSigV4.Provider2.Should().Be("amz");
            options.AwsSigV4.Region.Should().Be("us-east-1");
            options.AwsSigV4.Service.Should().Be("s3");
        }

        [Fact]
        public void Parse_AwsSigV4Flag_WithCredentials_ParsesBoth()
        {
            var command = "curl --aws-sigv4 \"aws:amz:us-east-1:execute-api\" --user \"AKIAIOSFODNN7EXAMPLE:wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY\" https://api.execute-api.us-east-1.amazonaws.com/stage";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.Credentials.Should().NotBeNull();
            options.Credentials.UserName.Should().Be("AKIAIOSFODNN7EXAMPLE");
            options.Credentials.Password.Should().Be("wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");
        }

        [Fact]
        public void Parse_AwsSigV4Flag_WithData_ParsesCorrectly()
        {
            var command = "curl --aws-sigv4 \"aws:amz:us-east-1:dynamodb\" -u \"AKID:SECRET\" -d '{\"TableName\":\"test\"}' -H \"Content-Type: application/json\" https://dynamodb.us-east-1.amazonaws.com";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Service.Should().Be("dynamodb");
            options.Data.Should().Be("{\"TableName\":\"test\"}");
            options.Method.Should().Be("POST");
        }

        [Fact]
        public void Parse_AwsSigV4Flag_GcpProvider_ParsesCorrectly()
        {
            var command = "curl --aws-sigv4 \"gcp:goog:us-central1:storage\" -u \"KEY:SECRET\" https://storage.googleapis.com/bucket/object";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("gcp");
            options.AwsSigV4.Provider2.Should().Be("goog");
        }

        [Fact]
        public void Parse_AwsSigV4Flag_MinimalConfig_ParsesCorrectly()
        {
            var command = "curl --aws-sigv4 aws -u \"AKID:SECRET\" https://s3.amazonaws.com/bucket";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("aws");
            options.AwsSigV4.Provider2.Should().Be("aws");
        }

        [Fact]
        public void Parse_AwsSigV4Flag_WithEqualsSignSyntax_ParsesCorrectly()
        {
            var command = "curl --aws-sigv4=aws:amz:us-east-1:s3 -u AKID:SECRET https://s3.amazonaws.com/bucket";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("aws");
            options.AwsSigV4.Region.Should().Be("us-east-1");
            options.AwsSigV4.Service.Should().Be("s3");
        }

        [Fact]
        public void Parse_AwsSigV4Flag_WithAllCommonFlags_ParsesCorrectly()
        {
            var command = "curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u \"AKID:SECRET\" -H \"x-amz-security-token: SESSION_TOKEN\" -L -v https://s3.us-east-1.amazonaws.com/my-bucket/my-key";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.Credentials.Should().NotBeNull();
            options.Headers.Should().ContainKey("x-amz-security-token");
            options.Headers["x-amz-security-token"].Should().Be("SESSION_TOKEN");
            options.FollowLocation.Should().BeTrue();
            options.Verbose.Should().BeTrue();
        }

        [Fact]
        public void Parse_AwsSigV4Flag_CustomProvider_MiddleEarth()
        {
            var command = "curl --aws-sigv4 \"middleearth:gondor:shire:hobbiton\" -u \"frodo:ring\" https://api.middleearth.example.com/quest";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("middleearth");
            options.AwsSigV4.Provider2.Should().Be("gondor");
            options.AwsSigV4.Region.Should().Be("shire");
            options.AwsSigV4.Service.Should().Be("hobbiton");
        }

        [Fact]
        public void Parse_WithoutAwsSigV4_AwsSigV4IsNull()
        {
            var command = "curl https://example.com";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().BeNull();
        }

        #endregion

        #region AwsSigV4Signer signing tests

        [Fact]
        public void SignRequest_AddsAuthorizationHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/my-bucket");
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY", timestamp);

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.ToString().Should().StartWith("AWS4-HMAC-SHA256 Credential=AKIAIOSFODNN7EXAMPLE/20240115/us-east-1/s3/aws4_request");
            request.Headers.Authorization.ToString().Should().Contain("SignedHeaders=");
            request.Headers.Authorization.ToString().Should().Contain("Signature=");
        }

        [Fact]
        public void SignRequest_AddsDateHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/my-bucket");
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "AKID", "SECRET", timestamp);

            request.Headers.Contains("x-amz-date").Should().BeTrue();
            request.Headers.GetValues("x-amz-date").Should().Contain("20240115T120000Z");
        }

        [Fact]
        public void SignRequest_AddsContentSha256Header()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/my-bucket");
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "AKID", "SECRET", timestamp);

            request.Headers.Contains("x-amz-content-sha256").Should().BeTrue();
        }

        [Fact]
        public void SignRequest_GcpProvider_UsesCorrectHeaderPrefix()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://storage.googleapis.com/bucket/object");
            var config = AwsSigV4Config.Parse("gcp:goog:us-central1:storage");
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "AKID", "SECRET", timestamp);

            request.Headers.Authorization.ToString().Should().StartWith("GCP4-HMAC-SHA256");
            request.Headers.Contains("x-goog-date").Should().BeTrue();
            request.Headers.Contains("x-goog-content-sha256").Should().BeTrue();
        }

        [Fact]
        public void SignRequest_CustomProvider_UsesCorrectAlgorithm()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.middleearth.example.com/quest");
            var config = AwsSigV4Config.Parse("middleearth:gondor:shire:hobbiton");
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "frodo", "ring", timestamp);

            request.Headers.Authorization.ToString().Should().StartWith("MIDDLEEARTH4-HMAC-SHA256");
            request.Headers.Contains("x-gondor-date").Should().BeTrue();
            request.Headers.Contains("x-gondor-content-sha256").Should().BeTrue();
            request.Headers.Authorization.ToString().Should().Contain("shire/hobbiton/middleearth4_request");
        }

        [Fact]
        public void SignRequest_WithBody_IncludesPayloadHash()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://dynamodb.us-east-1.amazonaws.com/");
            request.Content = new StringContent("{\"TableName\":\"test\"}", Encoding.UTF8, "application/json");
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:dynamodb");
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "AKID", "SECRET", timestamp);

            // The content-sha256 header should contain a hash (not empty string hash)
            var contentHash = string.Join("", request.Headers.GetValues("x-amz-content-sha256"));
            contentHash.Should().NotBeEmpty();
            // SHA-256 of empty string is different from SHA-256 of the JSON body
            contentHash.Should().NotBe("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        }

        [Fact]
        public void SignRequest_DeterministicSignature()
        {
            // Same inputs should always produce the same signature
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");

            var request1 = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/bucket");
            AwsSigV4Signer.SignRequest(request1, config, "AKID", "SECRET", timestamp);

            var request2 = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/bucket");
            AwsSigV4Signer.SignRequest(request2, config, "AKID", "SECRET", timestamp);

            request1.Headers.Authorization.ToString().Should().Be(request2.Headers.Authorization.ToString());
        }

        [Fact]
        public void SignRequest_DifferentTimestamps_ProduceDifferentSignatures()
        {
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");

            var request1 = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/bucket");
            AwsSigV4Signer.SignRequest(request1, config, "AKID", "SECRET",
                new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc));

            var request2 = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/bucket");
            AwsSigV4Signer.SignRequest(request2, config, "AKID", "SECRET",
                new DateTime(2024, 1, 16, 12, 0, 0, DateTimeKind.Utc));

            request1.Headers.Authorization.ToString().Should().NotBe(request2.Headers.Authorization.ToString());
        }

        [Fact]
        public void SignRequest_DifferentKeys_ProduceDifferentSignatures()
        {
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");

            var request1 = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/bucket");
            AwsSigV4Signer.SignRequest(request1, config, "AKID1", "SECRET1", timestamp);

            var request2 = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/bucket");
            AwsSigV4Signer.SignRequest(request2, config, "AKID2", "SECRET2", timestamp);

            request1.Headers.Authorization.ToString().Should().NotBe(request2.Headers.Authorization.ToString());
        }

        [Fact]
        public void SignRequest_NullRequest_ThrowsArgumentNullException()
        {
            var config = AwsSigV4Config.Parse("aws:amz");
            Action act = () => AwsSigV4Signer.SignRequest(null, config, "AKID", "SECRET");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SignRequest_NullConfig_ThrowsArgumentNullException()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
            Action act = () => AwsSigV4Signer.SignRequest(request, null, "AKID", "SECRET");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SignRequest_EmptyAccessKey_ThrowsArgumentNullException()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
            var config = AwsSigV4Config.Parse("aws:amz");
            Action act = () => AwsSigV4Signer.SignRequest(request, config, "", "SECRET");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SignRequest_EmptySecretKey_ThrowsArgumentNullException()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
            var config = AwsSigV4Config.Parse("aws:amz");
            Action act = () => AwsSigV4Signer.SignRequest(request, config, "AKID", "");
            act.Should().Throw<ArgumentNullException>();
        }

        #endregion

        #region Region and service extraction from URL

        [Fact]
        public void ExtractRegion_S3Regional_ReturnsRegion()
        {
            var uri = new Uri("https://s3.us-east-1.amazonaws.com/bucket");
            var region = AwsSigV4Signer.ExtractRegion(uri);
            region.Should().Be("us-east-1");
        }

        [Fact]
        public void ExtractRegion_DynamoDB_ReturnsRegion()
        {
            var uri = new Uri("https://dynamodb.eu-west-1.amazonaws.com/");
            var region = AwsSigV4Signer.ExtractRegion(uri);
            region.Should().Be("eu-west-1");
        }

        [Fact]
        public void ExtractRegion_ApiGateway_ReturnsRegion()
        {
            var uri = new Uri("https://abc123.execute-api.ap-southeast-2.amazonaws.com/stage");
            var region = AwsSigV4Signer.ExtractRegion(uri);
            region.Should().Be("ap-southeast-2");
        }

        [Fact]
        public void ExtractRegion_NoRegionInUrl_DefaultsToUsEast1()
        {
            var uri = new Uri("https://example.com/api");
            var region = AwsSigV4Signer.ExtractRegion(uri);
            region.Should().Be("us-east-1");
        }

        [Fact]
        public void ExtractService_S3_ReturnsS3()
        {
            var uri = new Uri("https://s3.us-east-1.amazonaws.com/bucket");
            var service = AwsSigV4Signer.ExtractService(uri);
            service.Should().Be("s3");
        }

        [Fact]
        public void ExtractService_DynamoDB_ReturnsDynamodb()
        {
            var uri = new Uri("https://dynamodb.eu-west-1.amazonaws.com/");
            var service = AwsSigV4Signer.ExtractService(uri);
            service.Should().Be("dynamodb");
        }

        #endregion

        #region CurlOptions.Clone with AwsSigV4

        [Fact]
        public void CurlOptions_Clone_PreservesAwsSigV4()
        {
            var options = new CurlOptions
            {
                AwsSigV4 = AwsSigV4Config.Parse("aws:amz:us-east-1:s3"),
                Url = "https://s3.amazonaws.com"
            };

            var clone = options.Clone();

            clone.AwsSigV4.Should().NotBeNull();
            clone.AwsSigV4.Provider1.Should().Be("aws");
            clone.AwsSigV4.Region.Should().Be("us-east-1");
        }

        [Fact]
        public void CurlOptions_Clone_NullAwsSigV4_StaysNull()
        {
            var options = new CurlOptions { Url = "https://example.com" };

            var clone = options.Clone();

            clone.AwsSigV4.Should().BeNull();
        }

        #endregion

        #region Real-world scenarios

        [Fact]
        public void Parse_AwsS3ListBucket_FullCommand()
        {
            // Real-world: List S3 bucket contents
            var command = @"curl --aws-sigv4 ""aws:amz:us-east-1:s3"" --user ""AKIAIOSFODNN7EXAMPLE:wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY"" https://my-bucket.s3.us-east-1.amazonaws.com/?list-type=2";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("aws");
            options.AwsSigV4.Service.Should().Be("s3");
            options.Credentials.Should().NotBeNull();
            options.Url.Should().Contain("list-type=2");
        }

        [Fact]
        public void Parse_AwsApiGateway_PostRequest()
        {
            // Real-world: POST to API Gateway
            var command = @"curl -X POST --aws-sigv4 ""aws:amz:eu-central-1:execute-api"" --user ""AKID:SECRET"" -d '{""action"":""invoke""}' -H ""Content-Type: application/json"" https://abc123.execute-api.eu-central-1.amazonaws.com/prod/myfunction";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Region.Should().Be("eu-central-1");
            options.AwsSigV4.Service.Should().Be("execute-api");
            options.Method.Should().Be("POST");
            options.Data.Should().Contain("invoke");
        }

        [Fact]
        public void Parse_AwsWithSecurityToken_ParsesTokenHeader()
        {
            // Using temporary credentials with a session token
            var command = @"curl --aws-sigv4 ""aws:amz:us-east-1:s3"" --user ""AKID:SECRET"" -H ""x-amz-security-token: FwoGZXIvYXdzEB0a"" https://s3.amazonaws.com/bucket";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.Headers.Should().ContainKey("x-amz-security-token");
            options.Headers["x-amz-security-token"].Should().Be("FwoGZXIvYXdzEB0a");
        }

        [Fact]
        public void Parse_GcpCloudStorage_FullCommand()
        {
            var command = @"curl --aws-sigv4 ""gcp:goog:us-central1:storage"" -u ""key:secret"" https://storage.googleapis.com/my-bucket/my-object";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("gcp");
            options.AwsSigV4.Provider2.Should().Be("goog");
            options.AwsSigV4.Region.Should().Be("us-central1");
            options.AwsSigV4.Service.Should().Be("storage");
        }

        [Fact]
        public void SignRequest_AwsApiGateway_ProducesValidAuth()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://abc123.execute-api.eu-central-1.amazonaws.com/prod/test");
            request.Content = new StringContent("{\"key\":\"value\"}", Encoding.UTF8, "application/json");
            var config = AwsSigV4Config.Parse("aws:amz:eu-central-1:execute-api");
            var timestamp = new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "AKID123", "SECRETKEY456", timestamp);

            var auth = request.Headers.Authorization.ToString();
            auth.Should().StartWith("AWS4-HMAC-SHA256");
            auth.Should().Contain("Credential=AKID123/20240615/eu-central-1/execute-api/aws4_request");
            auth.Should().Contain("SignedHeaders=");
            auth.Should().Contain("Signature=");
        }

        [Fact]
        public void SignRequest_WithQueryParameters_SortsAndSigns()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://s3.us-east-1.amazonaws.com/bucket?prefix=foo&delimiter=/&max-keys=100");
            var config = AwsSigV4Config.Parse("aws:amz:us-east-1:s3");
            var timestamp = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

            AwsSigV4Signer.SignRequest(request, config, "AKID", "SECRET", timestamp);

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.ToString().Should().StartWith("AWS4-HMAC-SHA256");
        }

        #endregion

        #region Line continuation and shell variants

        [Fact]
        public void Parse_AwsSigV4_UnixLineContinuation()
        {
            var command = "curl \\\n--aws-sigv4 \"aws:amz:us-east-1:s3\" \\\n-u \"AKID:SECRET\" \\\nhttps://s3.amazonaws.com/bucket";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("aws");
        }

        [Fact]
        public void Parse_AwsSigV4_WindowsLineContinuation()
        {
            var command = "curl ^\r\n--aws-sigv4 \"aws:amz:us-east-1:s3\" ^\r\n-u \"AKID:SECRET\" ^\r\nhttps://s3.amazonaws.com/bucket";

            var options = _parser.Parse(command);

            options.AwsSigV4.Should().NotBeNull();
            options.AwsSigV4.Provider1.Should().Be("aws");
        }

        #endregion
    }
}
