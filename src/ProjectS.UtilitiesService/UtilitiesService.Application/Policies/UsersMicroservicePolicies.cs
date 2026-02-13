using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace UtilitiesService.Application.Policies;

public class UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger, PollyPolicies pollyPolicies)
{
    private readonly ILogger<UsersMicroservicePolicies> _logger = logger;
    private readonly PollyPolicies _pollyPolicies = pollyPolicies;

    public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
        var retryPolicy = _pollyPolicies.GetRetryPolicy(5);
        var circuitBreakerPolicy = _pollyPolicies.GetCircuitBreakerPolicy(3, TimeSpan.FromMinutes(2));
        var timeoutPolicy = _pollyPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(5));

        AsyncPolicyWrap<HttpResponseMessage> wrappedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
        return wrappedPolicy;
    }
}
