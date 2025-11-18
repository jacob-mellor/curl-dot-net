using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Comprehensive tests for BuiltInMiddleware to achieve 100% code coverage.
    /// Tests all built-in middleware implementations: Logging, Retry, Timing, Caching, Authentication, RateLimit, and Modifiers.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class BuiltInMiddlewareTests
    {
        #region LoggingMiddleware Tests

        [Fact]
        public async Task LoggingMiddleware_Success_LogsRequestAndResponse()
        {
            // Arrange
            var logMessages = new List<string>();
            var middleware = new LoggingMiddleware(msg => logMessages.Add(msg));
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.Delay(10);
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            result.StatusCode.Should().Be(200);
            logMessages.Should().HaveCount(2);
            logMessages[0].Should().Contain("[CURL] Executing:");
            logMessages[0].Should().Contain("https://api.example.com");
            logMessages[1].Should().Contain("[CURL] Success:");
            logMessages[1].Should().Contain("Status=200");
            logMessages[1].Should().Contain("Time=");
        }

        [Fact]
        public async Task LoggingMiddleware_Failure_LogsException()
        {
            // Arrange
            var logMessages = new List<string>();
            var middleware = new LoggingMiddleware(msg => logMessages.Add(msg));
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await middleware.ExecuteAsync(context, async () =>
                {
                    await Task.Delay(10);
                    throw new InvalidOperationException("Test error");
                });
            });

            logMessages.Should().HaveCount(2);
            logMessages[0].Should().Contain("[CURL] Executing:");
            logMessages[1].Should().Contain("[CURL] Failed:");
            logMessages[1].Should().Contain("Test error");
            logMessages[1].Should().Contain("Time=");
        }

        [Fact]
        public async Task LoggingMiddleware_DefaultLogger_UsesConsole()
        {
            // Arrange
            var middleware = new LoggingMiddleware(); // Uses Console.WriteLine by default
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            result.StatusCode.Should().Be(200);
        }

        #endregion

        #region RetryMiddleware Tests

        [Fact]
        public async Task RetryMiddleware_SuccessOnFirstAttempt_DoesNotRetry()
        {
            // Arrange
            var middleware = new RetryMiddleware(3);
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };
            var attemptCount = 0;

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                attemptCount++;
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            result.StatusCode.Should().Be(200);
            attemptCount.Should().Be(1);
        }

        [Fact]
        public async Task RetryMiddleware_ServerError_Retries()
        {
            // Arrange
            var middleware = new RetryMiddleware(3, TimeSpan.FromMilliseconds(10));
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };
            var attemptCount = 0;

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                attemptCount++;
                await Task.CompletedTask;
                if (attemptCount < 3)
                {
                    return new CurlResult { StatusCode = 500 };
                }
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            result.StatusCode.Should().Be(200);
            attemptCount.Should().Be(3);
        }

        [Fact]
        public async Task RetryMiddleware_TooManyRequests_Retries()
        {
            // Arrange
            var middleware = new RetryMiddleware(2, TimeSpan.FromMilliseconds(10));
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };
            var attemptCount = 0;

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                attemptCount++;
                await Task.CompletedTask;
                if (attemptCount == 1)
                {
                    return new CurlResult { StatusCode = 429 }; // Too Many Requests
                }
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            result.StatusCode.Should().Be(200);
            attemptCount.Should().Be(2);
        }

        [Fact]
        public async Task RetryMiddleware_RetryableException_Retries()
        {
            // Arrange
            var middleware = new RetryMiddleware(3, TimeSpan.FromMilliseconds(10));
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };
            var attemptCount = 0;

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                attemptCount++;
                await Task.CompletedTask;
                if (attemptCount < 3)
                {
                    throw new CurlOperationTimeoutException(30, "curl https://api.example.com");
                }
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            result.StatusCode.Should().Be(200);
            attemptCount.Should().Be(3);
        }

        [Fact]
        public async Task RetryMiddleware_ExceedsMaxRetries_ThrowsCurlRetryException()
        {
            // Arrange
            var middleware = new RetryMiddleware(2, TimeSpan.FromMilliseconds(10));
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CurlRetryException>(async () =>
            {
                await middleware.ExecuteAsync(context, async () =>
                {
                    await Task.CompletedTask;
                    throw new CurlOperationTimeoutException(30, "curl https://api.example.com");
                });
            });

            ex.Message.Should().Contain("Failed after 2 retries");
            ex.Command.Should().Be("curl https://api.example.com");
            ex.RetryCount.Should().Be(2);
        }

        [Fact]
        public async Task RetryMiddleware_NonRetryableException_DoesNotRetry()
        {
            // Arrange
            var middleware = new RetryMiddleware(3);
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };
            var attemptCount = 0;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await middleware.ExecuteAsync(context, async () =>
                {
                    attemptCount++;
                    await Task.CompletedTask;
                    throw new InvalidOperationException("Non-retryable error");
                });
            });

            attemptCount.Should().Be(1);
        }

        #endregion

        #region TimingMiddleware Tests

        [Fact]
        public async Task TimingMiddleware_AddsTimingInfo()
        {
            // Arrange
            var middleware = new TimingMiddleware();
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.Delay(50);
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            result.Timings.Should().NotBeNull();
            result.Timings.Total.Should().BeGreaterThan(0);
            context.Properties.Should().ContainKey("TimingStart");
            context.Properties.Should().ContainKey("TimingEnd");
            context.Properties.Should().ContainKey("TimingDuration");
        }

        [Fact]
        public async Task TimingMiddleware_PreservesExistingTimings()
        {
            // Arrange
            var middleware = new TimingMiddleware();
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.Delay(10);
                return new CurlResult
                {
                    StatusCode = 200,
                    Timings = new CurlTimings { PreTransfer = 5.0 }
                };
            });

            // Assert
            result.Timings.Should().NotBeNull();
            result.Timings.PreTransfer.Should().Be(5.0);
            result.Timings.Total.Should().BeGreaterThan(0);
        }

        #endregion

        #region CachingMiddleware Tests

        [Fact]
        public async Task CachingMiddleware_CachesGetRequests()
        {
            // Arrange
            CachingMiddleware.ClearCache();
            var middleware = new CachingMiddleware(TimeSpan.FromSeconds(10));
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions
                {
                    Url = "https://api.example.com",
                    Method = "GET"
                }
            };
            var executionCount = 0;

            // Act
            var result1 = await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult
                {
                    StatusCode = 200,
                    Body = "Response 1"
                };
            });

            var result2 = await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult
                {
                    StatusCode = 200,
                    Body = "Response 2"
                };
            });

            // Assert
            executionCount.Should().Be(1); // Second call should use cache
            result1.Body.Should().Be("Response 1");
            result2.Body.Should().Be("Response 1"); // Cached result
        }

        [Fact]
        public async Task CachingMiddleware_DoesNotCachePostRequests()
        {
            // Arrange
            CachingMiddleware.ClearCache();
            var middleware = new CachingMiddleware();
            var context = new CurlContext
            {
                Command = "curl -X POST https://api.example.com",
                Options = new CurlOptions
                {
                    Url = "https://api.example.com",
                    Method = "POST"
                }
            };
            var executionCount = 0;

            // Act
            await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            executionCount.Should().Be(2); // Both calls should execute
        }

        [Fact]
        public async Task CachingMiddleware_ExpiredCache_RefreshesData()
        {
            // Arrange
            CachingMiddleware.ClearCache();
            var middleware = new CachingMiddleware(TimeSpan.FromMilliseconds(50));
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions
                {
                    Url = "https://api.example.com",
                    Method = "GET"
                }
            };
            var executionCount = 0;

            // Act
            await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            await Task.Delay(100); // Wait for cache to expire

            await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            executionCount.Should().Be(2); // Second call should not use expired cache
        }

        [Fact]
        public async Task CachingMiddleware_DoesNotCacheFailedRequests()
        {
            // Arrange
            CachingMiddleware.ClearCache();
            var middleware = new CachingMiddleware();
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions
                {
                    Url = "https://api.example.com",
                    Method = "GET"
                }
            };
            var executionCount = 0;

            // Act
            await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 500 }; // Failed request
            });

            await middleware.ExecuteAsync(context, async () =>
            {
                executionCount++;
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            executionCount.Should().Be(2); // Failed request should not be cached
        }

        [Fact]
        public async Task CachingMiddleware_CleanupExpiredEntries()
        {
            // Arrange
            CachingMiddleware.ClearCache();
            var middleware = new CachingMiddleware(TimeSpan.FromMilliseconds(50));

            // Create many cache entries to trigger cleanup
            for (int i = 0; i < 110; i++)
            {
                var context = new CurlContext
                {
                    Command = $"curl https://api.example.com/{i}",
                    Options = new CurlOptions
                    {
                        Url = $"https://api.example.com/{i}",
                        Method = "GET"
                    }
                };

                await middleware.ExecuteAsync(context, async () =>
                {
                    await Task.CompletedTask;
                    return new CurlResult { StatusCode = 200, Body = $"Response {i}" };
                });
            }

            // Assert
            // Cleanup should have been triggered when count exceeded 100
            // This test verifies the cleanup path is exercised
        }

        #endregion

        #region AuthenticationMiddleware Tests

        [Fact]
        public async Task AuthenticationMiddleware_AddsAuthHeader()
        {
            // Arrange
            var middleware = new AuthenticationMiddleware(async ctx =>
            {
                await Task.CompletedTask;
                return "test-token";
            });
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            context.Options.Headers.Should().ContainKey("Authorization");
            context.Options.Headers["Authorization"].Should().Be("Bearer test-token");
            context.Command.Should().Contain("Authorization: Bearer test-token");
        }

        [Fact]
        public async Task AuthenticationMiddleware_NullToken_DoesNotAddHeader()
        {
            // Arrange
            var middleware = new AuthenticationMiddleware(async ctx =>
            {
                await Task.CompletedTask;
                return null;
            });
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            context.Options.Headers.Should().BeEmpty();
            context.Command.Should().Be("curl https://api.example.com");
        }

        [Fact]
        public async Task AuthenticationMiddleware_EmptyToken_DoesNotAddHeader()
        {
            // Arrange
            var middleware = new AuthenticationMiddleware(async ctx =>
            {
                await Task.CompletedTask;
                return "";
            });
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            context.Options.Headers.Should().BeEmpty();
        }

        [Fact]
        public async Task AuthenticationMiddleware_NullOptions_CreatesOptions()
        {
            // Arrange
            var middleware = new AuthenticationMiddleware(async ctx =>
            {
                await Task.CompletedTask;
                return "token";
            });
            var context = new CurlContext
            {
                Command = "curl https://api.example.com",
                Options = null
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            context.Options.Should().NotBeNull();
            context.Options.Headers.Should().ContainKey("Authorization");
        }

        [Fact]
        public void AuthenticationMiddleware_NullTokenProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AuthenticationMiddleware(null));
        }

        #endregion

        #region RateLimitMiddleware Tests

        [Fact]
        public async Task RateLimitMiddleware_AllowsRequestsWithinLimit()
        {
            // Arrange
            var middleware = new RateLimitMiddleware(5); // 5 requests per second
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };
            var requestCount = 0;

            // Act
            var tasks = new List<Task<CurlResult>>();
            for (int i = 0; i < 3; i++)
            {
                tasks.Add(middleware.ExecuteAsync(context, async () =>
                {
                    Interlocked.Increment(ref requestCount);
                    await Task.CompletedTask;
                    return new CurlResult { StatusCode = 200 };
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            requestCount.Should().Be(3);
        }

        [Fact]
        public async Task RateLimitMiddleware_DelaysRequestsOverLimit()
        {
            // Arrange
            var middleware = new RateLimitMiddleware(2); // 2 requests per second
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };

            // Act
            var startTime = DateTime.UtcNow;
            var tasks = new List<Task<CurlResult>>();

            // Fire 3 requests rapidly (more than the 2/sec limit)
            for (int i = 0; i < 3; i++)
            {
                tasks.Add(middleware.ExecuteAsync(context, async () =>
                {
                    await Task.CompletedTask;
                    return new CurlResult { StatusCode = 200 };
                }));
            }

            await Task.WhenAll(tasks);
            var duration = DateTime.UtcNow - startTime;

            // Assert
            // The third request should be delayed
            duration.TotalMilliseconds.Should().BeGreaterThan(500); // At least some delay for rate limiting
        }

        [Fact]
        public async Task RateLimitMiddleware_CleansOldRequests()
        {
            // Arrange
            var middleware = new RateLimitMiddleware(2);
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" },
                CancellationToken = CancellationToken.None
            };

            // Act
            // First batch of requests
            for (int i = 0; i < 2; i++)
            {
                await middleware.ExecuteAsync(context, async () =>
                {
                    await Task.CompletedTask;
                    return new CurlResult { StatusCode = 200 };
                });
            }

            // Wait for the window to pass
            await Task.Delay(1100);

            // Second batch should not be delayed
            var startTime = DateTime.UtcNow;
            for (int i = 0; i < 2; i++)
            {
                await middleware.ExecuteAsync(context, async () =>
                {
                    await Task.CompletedTask;
                    return new CurlResult { StatusCode = 200 };
                });
            }
            var duration = DateTime.UtcNow - startTime;

            // Assert
            duration.TotalMilliseconds.Should().BeLessThan(500); // Should not be delayed
        }

        #endregion

        #region RequestModifierMiddleware Tests

        [Fact]
        public async Task RequestModifierMiddleware_ModifiesContext()
        {
            // Arrange
            var middleware = new RequestModifierMiddleware(ctx =>
            {
                ctx.Command = "modified command";
                if (ctx.Options == null)
                    ctx.Options = new CurlOptions();
                ctx.Options.Url = "https://modified.example.com";
            });
            var context = new CurlContext
            {
                Command = "original command",
                Options = new CurlOptions { Url = "https://original.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.CompletedTask;
                return new CurlResult { StatusCode = 200 };
            });

            // Assert
            context.Command.Should().Be("modified command");
            context.Options.Url.Should().Be("https://modified.example.com");
        }

        [Fact]
        public void RequestModifierMiddleware_NullModifier_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RequestModifierMiddleware(null));
        }

        #endregion

        #region ResponseModifierMiddleware Tests

        [Fact]
        public async Task ResponseModifierMiddleware_ModifiesResponse()
        {
            // Arrange
            var middleware = new ResponseModifierMiddleware(async result =>
            {
                await Task.CompletedTask;
                result.StatusCode = 201;
                result.Body = "Modified response";
                return result;
            });
            var context = new CurlContext
            {
                Options = new CurlOptions { Url = "https://api.example.com" }
            };

            // Act
            var result = await middleware.ExecuteAsync(context, async () =>
            {
                await Task.CompletedTask;
                return new CurlResult
                {
                    StatusCode = 200,
                    Body = "Original response"
                };
            });

            // Assert
            result.StatusCode.Should().Be(201);
            result.Body.Should().Be("Modified response");
        }

        [Fact]
        public void ResponseModifierMiddleware_NullModifier_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ResponseModifierMiddleware(null));
        }

        #endregion
    }
}