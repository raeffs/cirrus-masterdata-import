using System.Net.Http;
using Flurl.Http.Configuration;

namespace Cirrus.Import.Masterdata.Infrastructure
{
    class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new PolicyHandler
            {
                InnerHandler = base.CreateMessageHandler()
            };
        }
    }
}
