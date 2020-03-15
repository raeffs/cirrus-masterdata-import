using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace Cirrus.Import.Masterdata.Infrastructure
{
    class PolicyHandler : DelegatingHandler
    {
        private readonly ApiOptions options;

        public PolicyHandler(ApiOptions options)
        {
            this.options = options;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return this.PolicyStrategy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }

        private AsyncPolicy<HttpResponseMessage> PolicyStrategy => this.RetryPolicy;

        private AsyncPolicy<HttpResponseMessage> TimeoutPolicy => Policy
            .TimeoutAsync<HttpResponseMessage>(this.options.TimeoutInSeconds, (context, timeSpan, task) =>
            {
                Console.WriteLine($"Timeout policy fired after {timeSpan.Seconds} seconds");
                return Task.CompletedTask;
            });

        private AsyncPolicy<HttpResponseMessage> RetryPolicy => Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode
                && (r.RequestMessage.Method == HttpMethod.Get || r.RequestMessage.Method == HttpMethod.Delete))
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                this.options.RetryIntervalsInSeconds.Select(x => TimeSpan.FromSeconds(x)),
                (delegateResult, timeSpan, attempt, context) =>
                {
                    Console.WriteLine($"Retry policy fired, attempt {attempt}");
                });
    }
}
