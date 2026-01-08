using System.Net;
using Polly;
using Polly.Timeout;

namespace UsersService.API.Policies;

public static class PollyPolicies
{
    // Retry with exponential backoff + jitter
    public static IAsyncPolicy<HttpResponseMessage> RetryPolicy =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry =>
                    TimeSpan.FromSeconds(Math.Pow(2, retry)) +
                    TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                onRetry: (outcome, timespan, retry, context) =>
                {
                    // log retry
                });

    // Circuit Breaker
    public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, breakDelay) => { },
                onReset: () => { },
                onHalfOpen: () => { });

    // Timeout
    public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy =>
        Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(5),
            TimeoutStrategy.Optimistic);

    // Fallback
    public static IAsyncPolicy<HttpResponseMessage> FallbackPolicy =>
        Policy<HttpResponseMessage>
            .Handle<Exception>()
            .FallbackAsync(
                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("Service temporarily unavailable")
                });

    // Bulkhead (protect thread pool)
    public static IAsyncPolicy<HttpResponseMessage> BulkheadPolicy =>
        Policy.BulkheadAsync<HttpResponseMessage>(
            maxParallelization: 50,
            maxQueuingActions: 25,
            onBulkheadRejectedAsync: _ =>
            {
                // log rejection
                return Task.CompletedTask;
            });

    // Policy Wrap (execution order matters)
    public static IAsyncPolicy<HttpResponseMessage> ResiliencePolicy =>
        Policy.WrapAsync(
            FallbackPolicy,
            BulkheadPolicy,
            RetryPolicy,
            CircuitBreakerPolicy,
            TimeoutPolicy
        );
}
