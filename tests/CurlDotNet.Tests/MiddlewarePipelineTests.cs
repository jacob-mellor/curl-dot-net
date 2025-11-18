using System;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Middleware;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for CurlMiddlewarePipeline to achieve 100% code coverage.
    /// Tests pipeline building, execution, and context management.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class MiddlewarePipelineTests
    {
        #region Pipeline Construction Tests

        [Fact]
        public void Constructor_InitializesEmptyPipeline()
        {
            // Arrange
            Func<CurlContext, Task<CurlResult>> handler = async (ctx) => new CurlResult();

            // Act
            var pipeline = new CurlMiddlewarePipeline(handler);

            // Assert
            pipeline.Should().NotBeNull();
            pipeline.Count.Should().Be(0);
        }

        [Fact]
        public void Use_AddsMiddlewareToPipeline()
        {
            // Arrange
            var pipeline = new CurlMiddlewarePipeline(async (ctx) => new CurlResult());
            var middleware = new TestMiddleware();

            // Act
            var result = pipeline.Use(middleware);

            // Assert
            result.Should().BeSameAs(pipeline); // Fluent return
            pipeline.Count.Should().Be(1);
        }

        [Fact]
        public void Use_WithFunc_AddsMiddlewareToPipeline()
        {
            // Arrange
            var pipeline = new CurlMiddlewarePipeline(async (ctx) => new CurlResult());

            // Act
            var result = pipeline.Use(async (context, next) =>
            {
                return await next();
            });

            // Assert
            result.Should().BeSameAs(pipeline);
            pipeline.Count.Should().Be(1);
        }

        [Fact]
        public void Clear_RemovesAllMiddleware()
        {
            // Arrange
            var pipeline = new CurlMiddlewarePipeline(async (ctx) => new CurlResult());
            pipeline.Use(new TestMiddleware());
            pipeline.Use(new TestMiddleware());

            // Act
            pipeline.Clear();

            // Assert
            pipeline.Count.Should().Be(0);
        }

        #endregion

        #region Pipeline Execution Tests

        [Fact]
        public async Task ExecuteAsync_WithEmptyPipeline_ExecutesHandler()
        {
            // Arrange
            var handlerExecuted = false;
            var pipeline = new CurlMiddlewarePipeline(async (ctx) =>
            {
                handlerExecuted = true;
                return new CurlResult { Body = "Success" };
            });
            var context = new CurlContext { Options = new CurlOptions { Url = "https://api.example.com" } };

            // Act
            var result = await pipeline.ExecuteAsync(context);

            // Assert
            handlerExecuted.Should().BeTrue();
            result.Body.Should().Be("Success");
        }

        [Fact]
        public async Task ExecuteAsync_WithMiddleware_ExecutesInOrder()
        {
            // Arrange
            var executionOrder = "";
            var pipeline = new CurlMiddlewarePipeline(async (ctx) =>
            {
                executionOrder += "H";
                return new CurlResult();
            });

            pipeline.Use(async (context, next) =>
            {
                executionOrder += "1";
                var result = await next();
                executionOrder += "4";
                return result;
            });

            pipeline.Use(async (context, next) =>
            {
                executionOrder += "2";
                var result = await next();
                executionOrder += "3";
                return result;
            });

            var context = new CurlContext { Options = new CurlOptions { Url = "https://api.example.com" } };

            // Act
            await pipeline.ExecuteAsync(context);

            // Assert
            executionOrder.Should().Be("12H34");
        }

        [Fact]
        public async Task ExecuteAsync_MiddlewareCanShortCircuit()
        {
            // Arrange
            var pipeline = new CurlMiddlewarePipeline(async (ctx) => await Task.FromResult(new CurlResult()));
            var secondMiddlewareExecuted = false;

            pipeline.Use(async (context, next) =>
            {
                // Short-circuit without calling next
                return await Task.FromResult(new CurlResult { Body = "Short-circuited" });
            });

            pipeline.Use(async (context, next) =>
            {
                secondMiddlewareExecuted = true;
                return await next();
            });

            var context = new CurlContext { Options = new CurlOptions { Url = "https://api.example.com" } };

            // Act
            var result = await pipeline.ExecuteAsync(context);

            // Assert
            secondMiddlewareExecuted.Should().BeFalse();
            result.Body.Should().Be("Short-circuited");
        }

        // Note: ExecuteAsync cannot throw for missing handler since it's provided in constructor

        #endregion

        #region Builder Tests

        [Fact]
        public void CreateBuilder_ReturnsNewBuilder()
        {
            // Act
            var builder = CurlMiddlewarePipeline.CreateBuilder();

            // Assert
            builder.Should().NotBeNull();
            builder.Should().BeOfType<CurlMiddlewarePipelineBuilder>();
        }

        [Fact]
        public void Builder_Use_AddsMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();
            var middleware = new TestMiddleware();

            // Act
            var result = builder.Use(middleware);

            // Assert
            result.Should().BeSameAs(builder); // Fluent return
        }

        [Fact]
        public void Builder_UseLogging_AddsLoggingMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();
            var logMessages = "";

            // Act
            var result = builder.UseLogging(msg => logMessages += msg);

            // Assert
            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void Builder_UseRetry_AddsRetryMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();

            // Act
            var result = builder.UseRetry(3, TimeSpan.FromSeconds(1));

            // Assert
            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void Builder_UseTiming_AddsTimingMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();

            // Act
            var result = builder.UseTiming();

            // Assert
            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void Builder_UseCaching_AddsCachingMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();

            // Act
            var result = builder.UseCaching(TimeSpan.FromMinutes(5));

            // Assert
            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void Builder_UseAuthentication_AddsAuthMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();

            // Act
            var result = builder.UseAuthentication(async (ctx) => await Task.FromResult("token123"));

            // Assert
            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void Builder_UseRateLimit_AddsRateLimitMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();

            // Act
            var result = builder.UseRateLimit(60);

            // Assert
            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void Builder_WithHandler_SetsHandler()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder();
            Func<CurlContext, Task<CurlResult>> handler = async (ctx) => await Task.FromResult(new CurlResult());

            // Act
            var result = builder.WithHandler(handler);

            // Assert
            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void Builder_Build_CreatesPipelineWithMiddleware()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder()
                .UseLogging(msg => { })
                .UseRetry(3)
                .WithHandler(async (ctx) => await Task.FromResult(new CurlResult()));

            // Act
            var pipeline = builder.Build();

            // Assert
            pipeline.Should().NotBeNull();
            pipeline.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Builder_Build_ThrowsIfNoHandler()
        {
            // Arrange
            var builder = CurlMiddlewarePipeline.CreateBuilder()
                .UseLogging(msg => { });
            // No handler set

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        #endregion

        #region CurlContext Tests

        [Fact]
        public void CurlContext_InitializesProperties()
        {
            // Arrange
            var options = new CurlOptions { Url = "https://api.example.com" };

            // Act
            var context = new CurlContext { Options = options };

            // Assert
            context.Options.Should().BeSameAs(options);
            context.Properties.Should().NotBeNull();
            context.Properties.Should().BeEmpty();
        }

        [Fact]
        public void CurlContext_WithProperty_AddsProperty()
        {
            // Arrange
            var context = new CurlContext { Options = new CurlOptions() };

            // Act
            var result = context.WithProperty("key", "value");

            // Assert
            result.Should().BeSameAs(context); // Fluent return
            context.Properties["key"].Should().Be("value");
        }

        [Fact]
        public void CurlContext_GetProperty_RetrievesValue()
        {
            // Arrange
            var context = new CurlContext { Options = new CurlOptions() };
            context.WithProperty("key", 42);

            // Act
            var value = context.GetProperty<int>("key");

            // Assert
            value.Should().Be(42);
        }

        [Fact]
        public void CurlContext_GetProperty_ReturnsDefaultForMissingKey()
        {
            // Arrange
            var context = new CurlContext { Options = new CurlOptions() };

            // Act
            var value = context.GetProperty<int>("missing");

            // Assert
            value.Should().Be(0); // Default for int
        }

        [Fact]
        public void CurlContext_GetProperty_ReturnsDefaultForWrongType()
        {
            // Arrange
            var context = new CurlContext { Options = new CurlOptions() };
            context.WithProperty("key", "string value");

            // Act
            var value = context.GetProperty<int>("key");

            // Assert
            value.Should().Be(0); // Default when cast fails
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Pipeline_CompleteFlow_ExecutesSuccessfully()
        {
            // Arrange
            var pipeline = CurlMiddlewarePipeline.CreateBuilder()
                .UseLogging(msg => { })
                .UseTiming()
                .UseRetry(2)
                .WithHandler(async (ctx) => await Task.FromResult(new CurlResult { Body = "Success" }))
                .Build();

            var context = new CurlContext { Options = new CurlOptions { Url = "https://api.example.com" } };

            // Act
            var result = await pipeline.ExecuteAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Body.Should().Be("Success");
        }

        [Fact]
        public async Task Pipeline_MultipleMiddleware_ShareContext()
        {
            // Arrange
            var pipeline = new CurlMiddlewarePipeline(async (ctx) =>
            {
                var step2 = ctx.GetProperty<bool>("step2");
                return await Task.FromResult(new CurlResult { Body = step2.ToString() });
            });

            pipeline.Use(async (context, next) =>
            {
                context.WithProperty("step1", true);
                return await next();
            });

            pipeline.Use(async (context, next) =>
            {
                var step1 = context.GetProperty<bool>("step1");
                context.WithProperty("step2", step1);
                return await next();
            });

            var context = new CurlContext { Options = new CurlOptions() };

            // Act
            var result = await pipeline.ExecuteAsync(context);

            // Assert
            result.Body.Should().Be("True");
        }

        #endregion

        #region Test Helpers

        private class TestMiddleware : ICurlMiddleware
        {
            public int ExecutionCount { get; private set; }

            public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
            {
                ExecutionCount++;
                return await next();
            }
        }

        #endregion
    }
}