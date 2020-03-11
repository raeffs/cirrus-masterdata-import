using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.Cirrus.Units
{
    class UnitApi
    {
        private readonly ApiOptions config;
        private readonly Dictionary<string, Unit> mapping = new Dictionary<string, Unit>
        {
            { "piece", Unit.Piece },
            { "box", Unit.Box },
            { "polybag", Unit.Polybag },
            { "litre", Unit.Litre },
            { "kilogram", Unit.Kilogram }
        };

        public UnitApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task<List<CustomMapping<Unit>>> GetMappingsAsync()
        {
            var response = await this.GetClient()
                .AppendPathSegment("api/mdm/v1/units/mappings")
                .SetQueryParam("keys", "import")
                .SetQueryParam("values", string.Join(',', this.mapping.Keys))
                .GetJsonAsync<PagedList<Mapping>>();

            var result = response.Items
                .Select(x => new CustomMapping<Unit>
                {
                    Id = x.Id,
                    Value = this.mapping[x.Value]
                })
                .ToList();

            if (result.Count != this.mapping.Count)
            {
                throw new Exception("Units are missing or duplicate");
            }

            return result;
        }

        private IFlurlRequest GetClient()
        {
            return this.config.Endpoint
                .WithOAuthBearerToken(this.config.Token);
        }
    }
}
