using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using CurlDotNet.Middleware;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Middleware error messages say what happened, how to fix it and where to read more, and no raw
    /// .NET exception (NullReferenceException, TaskCanceledException, SemaphoreSlim's
    /// ArgumentOutOfRangeException) escapes the middleware API.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class DocsMiddlewareTests
    {
        private const string DocsLink = "See https://github.com/jacob-mellor/curl-dot-net/";

        private static Task<CurlResult> Ok(CurlContext ctx) => Task.FromResult(new CurlResult { StatusCode = 200, Body = "ok" });

        [Fact]
        public void Build_WithoutHandler_SaysHowToFix()
        {
            var builder = CurlMiddlewarePipeline.CreateBuilder().UseTiming();

            Action act = () => builder.Build();

            act.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Contain("1 middleware registered").And.Contain("WithHandler(").And.Contain(DocsLink);
        }

        [Fact]
        public void NullArguments_ThrowArgumentNullExceptionWithFixAndLink()
        {
            var pipeline = new CurlMiddlewarePipeline(Ok);
            var builder = CurlMiddlewarePipeline.CreateBuilder();

            Action[] acts =
            {
                () => new CurlMiddlewarePipeline(null),
                () => pipeline.Use((ICurlMiddleware)null),
                () => pipeline.Use((Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>>)null),
                () => builder.Use((ICurlMiddleware)null),
                () => builder.Use((Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>>)null),
                () => new AuthenticationMiddleware(null),
                () => new RequestModifierMiddleware(null),
                () => new ResponseModifierMiddleware(null),
            };

            foreach (var act in acts)
            {
                act.Should().Throw<ArgumentNullException>().Which.Message.Should().Contain(DocsLink);
            }
        }

        [Fact]
        public async Task ExecuteAsync_NullContext_ThrowsArgumentNullException()
        {
            var pipeline = new CurlMiddlewarePipeline(Ok);

            Func<Task> act = () => pipeline.ExecuteAsync(null);

            (await act.Should().ThrowAsync<ArgumentNullException>()).Which.Message.Should().Contain("new CurlContext").And.Contain(DocsLink);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void RateLimit_NotPositive_ThrowsArgumentOutOfRangeWithTheValue(int perSecond)
        {
            Action act = () => new RateLimitMiddleware(perSecond);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Which.Message.Should().Contain($"requestsPerSecond = {perSecond}").And.Contain("Pass 1 or more").And.Contain(DocsLink);
        }

        [Fact]
        public void Retry_NegativeDelay_ThrowsAtConstruction()
        {
            Action act = () => new RetryMiddleware(2, TimeSpan.FromSeconds(-1));

            act.Should().Throw<ArgumentOutOfRangeException>().Which.Message.Should().Contain("TimeSpan.Zero").And.Contain(DocsLink);
        }

        [Fact]
        public async Task Retry_Exhausted_SaysUrlAttemptsLastErrorAndFix()
        {
            var middleware = new RetryMiddleware(1, TimeSpan.Zero);
            var context = new CurlContext { Command = "curl -u bob:secret https://api.example.com/v1/items?key=abc" };
            var calls = 0;

            Func<Task> act = () => middleware.ExecuteAsync(context, () =>
            {
                calls++;
                throw new CurlCouldntConnectException("api.example.com", 443, "Connection refused", false, context.Command);
            });

            var ex = (await act.Should().ThrowAsync<CurlRetryException>()).Which;
            calls.Should().Be(2);
            ex.InnerException.Should().BeOfType<CurlCouldntConnectException>();
            ex.WhatHappened.Should().Contain("Failed after 1 retries").And.Contain("https://api.example.com/v1/items")
                .And.Contain("2 attempts").And.Contain("Connection refused");
            ex.Message.Should().NotContain("secret").And.NotContain("key=abc");
            ex.Suggestions.Should().NotBeEmpty();
            ex.Message.Should().Contain(DocsLink);
        }

        [Fact]
        public async Task Retry_NegativeMaxRetries_SaysTheRequestWasNeverAttempted()
        {
            var middleware = new RetryMiddleware(-1);

            Func<Task> act = () => middleware.ExecuteAsync(new CurlContext { Command = "curl https://api.example.com" }, () => Ok(null));

            (await act.Should().ThrowAsync<CurlRetryException>()).Which.Message.Should().Contain("never attempted").And.Contain(DocsLink);
        }

        [Fact]
        public async Task Retry_CancelledDuringBackoff_IsAbortedByCallback42()
        {
            using var cts = new CancellationTokenSource();
            var middleware = new RetryMiddleware(3, TimeSpan.FromSeconds(30));
            var context = new CurlContext { Command = "curl https://api.example.com", CancellationToken = cts.Token };

            Func<Task> act = () => middleware.ExecuteAsync(context, () =>
            {
                cts.Cancel();
                return Task.FromResult(new CurlResult { StatusCode = 503 });
            });

            var ex = (await act.Should().ThrowAsync<CurlAbortedByCallbackException>()).Which;
            ex.CurlErrorCode.Should().Be(42);
            ex.InnerException.Should().BeAssignableTo<OperationCanceledException>();
            ex.Message.Should().Contain("HTTP 503").And.Contain(DocsLink);
        }

        [Fact]
        public async Task RateLimit_CancelledWhileWaiting_IsAbortedByCallback42()
        {
            using var cts = new CancellationTokenSource();
            var middleware = new RateLimitMiddleware(1);
            await middleware.ExecuteAsync(new CurlContext(), () => Ok(null));
            cts.Cancel();

            Func<Task> act = () => middleware.ExecuteAsync(new CurlContext { CancellationToken = cts.Token }, () => Ok(null));

            (await act.Should().ThrowAsync<CurlAbortedByCallbackException>()).Which.Message.Should().Contain("1 requests per second");
        }

        [Fact]
        public async Task Authentication_ContextWithoutCommand_SetsHeaderWithoutThrowing()
        {
            var middleware = new AuthenticationMiddleware(_ => Task.FromResult("abc"));
            var context = new CurlContext { Options = new CurlOptions { Url = "https://api.example.com" } };

            await middleware.ExecuteAsync(context, () => Ok(context));

            context.Options.Headers["Authorization"].Should().Be("Bearer abc");
            context.Command.Should().BeNull();
        }

        [Fact]
        public void GetProperty_StoredNullReadAsValueType_ReturnsDefault()
        {
            var context = new CurlContext().WithProperty("n", null).WithProperty("i", 5);

            context.GetProperty<int>("n").Should().Be(0);
            context.GetProperty<long>("i").Should().Be(0, "values are cast, not converted");
            context.GetProperty<int>("i").Should().Be(5);
            context.GetProperty<string>("missing").Should().BeNull();
        }
    }
}
