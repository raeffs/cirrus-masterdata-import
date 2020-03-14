using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace Cirrus.Import.Masterdata.Infrastructure
{
    static class Policies
    {
        public static AsyncPolicyWrap<HttpResponseMessage> PolicyStrategy => Policy.WrapAsync(RetryPolicy, TimeoutPolicy);

        private static AsyncTimeoutPolicy<HttpResponseMessage> TimeoutPolicy => Policy.TimeoutAsync<HttpResponseMessage>(3, (context, timeSpan, task) =>
            {
                Console.WriteLine($"Timeout policy fired after {timeSpan.Seconds} seconds");
                return Task.CompletedTask;
            });

        private static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy => Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                },
                (delegateResult, retryCount) =>
                {
                    Console.WriteLine($"Retry policy fired, attempt {retryCount}");
                });
    }
}
