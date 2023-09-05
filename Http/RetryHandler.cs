using System.Net;
using Microsoft.SemanticKernel.Reliability;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Wrap;

namespace Aydex.SemanticKernel.NL2EF.Http;

public class RetryHandlerFactory : IDelegatingHandlerFactory
{
    public DelegatingHandler Create(ILoggerFactory? loggerFactory)
    {
        return new RetryHandler();
    }
}

public class RetryHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await GetRetryPolicy().ExecuteAsync(async () =>
        {
            var response = await base.SendAsync(request, cancellationToken);

            return response;
        });
    }

    private static AsyncPolicyWrap<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy.WrapAsync(
            Policy
                .HandleResult<HttpResponseMessage>((response) => response.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 1,
                    sleepDurationProvider: (retryCount, result, context) => result.Result.Headers.RetryAfter?.Delta
                        ?? result.Result.Headers.RetryAfter?.Date?.Subtract(DateTimeOffset.UtcNow)
                        ?? TimeSpan.FromSeconds(3),
                    onRetryAsync: (result, withDuration, retryCount, context) => Task.CompletedTask),
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 3)));
    }
}
