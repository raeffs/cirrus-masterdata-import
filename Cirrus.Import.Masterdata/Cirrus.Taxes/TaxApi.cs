using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.Cirrus.Taxes
{
    class TaxApi
    {
        private readonly ApiOptions config;

        public TaxApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task<List<CustomMapping<Tax>>> GetMappingsAsync()
        {
            return new List<CustomMapping<Tax>>
            {
                new CustomMapping<Tax> { Value = Tax.None, Id = "1" },
                new CustomMapping<Tax> { Value = Tax.Default, Id = "3" },
                new CustomMapping<Tax> { Value = Tax.Reduced, Id = "2" },
                new CustomMapping<Tax> { Value = Tax.TakeAway, Id = "2" },
                new CustomMapping<Tax> { Value = Tax.Fuel, Id = "85" },
            };
        }

        private IFlurlRequest GetClient()
        {
            return this.config.Endpoint
                .WithOAuthBearerToken(this.config.Token);
        }
    }
}
