using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.Cirrus.Groups
{
    class GroupApi
    {
        private readonly ApiOptions config;

        public GroupApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task<List<CustomMapping<Group>>> GetMappingsAsync()
        {
            return new List<CustomMapping<Group>>
            {
                new CustomMapping<Group> { Value = Group.Default, Id = "1" },
            };
        }

        private IFlurlRequest GetClient()
        {
            return this.config.Endpoint
                .WithOAuthBearerToken(this.config.Token);
        }
    }
}
