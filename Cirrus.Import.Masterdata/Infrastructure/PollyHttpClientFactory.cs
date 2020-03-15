using System.Net.Http;
using Flurl.Http.Configuration;

namespace Cirrus.Import.Masterdata.Infrastructure
{
    class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        private readonly ApiOptions options;

        public PollyHttpClientFactory(ApiOptions options)
        {
            this.options = options;
        }

        public override HttpMessageHandler CreateMessageHandler()
        {
            return new PolicyHandler(this.options)
            {
                InnerHandler = base.CreateMessageHandler()
            };
        }
    }
}
