using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlResultTests
    {
        [Fact]
        public void ParseJson_ShouldDeserializePayload()
        {
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "{\"name\":\"Ada\",\"role\":\"admin\"}"
            };

            var payload = result.ParseJson<Dictionary<string, string>>();

            payload["name"].Should().Be("Ada");
            payload["role"].Should().Be("admin");
        }

        [Fact]
        public void EnsureSuccess_ShouldThrowWhenStatusIsError()
        {
            var result = new CurlResult
            {
                StatusCode = 502,
                Body = "Bad gateway"
            };

            Action act = () => result.EnsureSuccess();

            var exception = act.Should().Throw<CurlDotNet.Core.CurlHttpException>().Which;
            exception.StatusCode.Should().Be(502);
            exception.ResponseBody.Should().Be("Bad gateway");
        }

        [Fact]
        public void EnsureContains_ShouldValidateSubstring()
        {
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "operation completed successfully"
            };

            result.EnsureContains("completed").Should().BeSameAs(result);
            Action act = () => result.EnsureContains("failed");
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void SaveToFile_ShouldPersistText()
        {
            var path = Path.GetTempFileName();
            try
            {
                var result = new CurlResult
                {
                    StatusCode = 200,
                    Body = "hello world"
                };

                result.SaveToFile(path);
                File.ReadAllText(path).Should().Be("hello world");
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void SaveToFile_ShouldPersistBinary()
        {
            var path = Path.Combine(Path.GetTempPath(), $"curltest_{Guid.NewGuid():N}.bin");
            try
            {
                var bytes = new byte[] { 0x01, 0x02, 0x03 };
                var result = new CurlResult
                {
                    StatusCode = 200,
                    BinaryData = bytes
                };

                result.SaveToFile(path);
                File.ReadAllBytes(path).Should().BeEquivalentTo(bytes);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public async Task SaveToFileAsync_ShouldPersistText()
        {
            var path = Path.Combine(Path.GetTempPath(), $"curltest_{Guid.NewGuid():N}.txt");
            try
            {
                var result = new CurlResult
                {
                    StatusCode = 200,
                    Body = "async-data"
                };

                await result.SaveToFileAsync(path);
                File.ReadAllText(path).Should().Be("async-data");
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
