using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;
using Polly;

namespace Cirrus.Import.Masterdata.Cirrus
{
    abstract class BaseApi<TModel> : MappedApi<string>
        where TModel : BaseModel
    {
        private readonly ApiOptions options;

        public BaseApi(ApiOptions options)
        {
            this.options = options;
        }

        public async Task AddOrUpdateAsync(IEnumerable<TModel> models)
        {
            var key = models.Select(x => x.ExternalKey).Distinct().Single();
            await this.GetMappingsAsync(key, models.Select(x => x.ExternalId));
            foreach (var model in models)
            {
                try
                {
                    await this.RetryPolicy.ExecuteAsync(async () =>
                    {
                        var id = await this.AddOrUpdateAsync(model);
                        this.AddMapping(new Mapping<string> { Id = id, Key = key, Value = model.ExternalId });
                    });
                }
                catch (Exception e)
                {
                    await Console.Error.WriteLineAsync($"Failed to add or update ({key}, {model.ExternalId})");
                    await Console.Error.WriteLineAsync(e.Message);
                }
            }
        }

        protected abstract Task<string> AddOrUpdateAsync(TModel model);

        protected override async Task ProcessDuplicatesAsync(Mapping<string> key, IEnumerable<Mapping<string>> duplicates)
        {
            await Console.Out.WriteLineAsync($"Found duplicate mappings ({key.Key}, {key.Value})");
            try
            {
                await this.RetryPolicy.ExecuteAsync(() => this.DeleteAsync(duplicates));
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync($"Failed to delete ({key}, {string.Join(';', duplicates.Select(x => x.Value))})");
                await Console.Error.WriteLineAsync(e.Message);
            }
        }

        protected virtual Task DeleteAsync(IEnumerable<Mapping<string>> toDelete) => Task.CompletedTask;

        protected IFlurlRequest GetClient()
        {
            return this.options.Endpoint
                .WithOAuthBearerToken(this.options.Token);
        }

        private AsyncPolicy RetryPolicy => Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                this.options.RetryIntervalsInSeconds.Select(x => TimeSpan.FromSeconds(x)),
                async (exception, timeSpan, attempt, context) =>
                {
                    await Console.Out.WriteLineAsync($"Execution failed, attemt {attempt}, retrying in {timeSpan.TotalSeconds} seconds");
                    await Console.Out.WriteLineAsync($"-> {exception.Message}");
                });
    }
}
