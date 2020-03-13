using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.Cirrus.Assortments
{
    class AssortmentApi : BaseApi<string>
    {
        private readonly ApiOptions config;

        public AssortmentApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task AddOrUpdateAsync(IEnumerable<Assortment> assortments)
        {
            var key = assortments.Select(x => x.ExternalKey).Distinct().Single();
            await this.GetMappingsAsync(key, assortments.Select(x => x.ExternalId));
            foreach (var assortment in assortments)
            {
                var id = await this.AddOrUpdateAsync(assortment);
                this.AddMapping(new Mapping<string> { Id = id, Key = key, Value = assortment.ExternalId });
            }
        }

        protected override async Task<IEnumerable<Mapping<string>>> LoadMappingsAsync(string key, IEnumerable<string> values)
        {
            var namesOfInterest = values.Select(x => $"{x} ({key})").ToList();

            var response = await this.GetClient()
                .AppendPathSegment("api/vme/v1/viewmodel/MdmProductAssortments")
                .SetQueryParam("pageSize", 100)
                .GetJsonAsync<ListViewModel<AssortmentListViewModel>>();

            return response.Data
                .Where(x => namesOfInterest.Contains(x.Name))
                .Select(x => new Mapping<string>
                {
                    Id = x.Id,
                    Key = key,
                    Value = values.Single(y => x.Name == $"{y} ({key})")
                })
                .ToList();
        }

        private async Task<string> AddOrUpdateAsync(Assortment assortment)
        {
            var id = await this.GetMappingAsync(assortment.ExternalKey, assortment.ExternalId);
            var add = string.IsNullOrWhiteSpace(id);
            AssortmentDetailViewModel dto;

            if (add)
            {
                dto = new AssortmentDetailViewModel();
            }
            else
            {
                dto = await this.GetClient()
                    .AppendPathSegment("api/vme/v1/viewmodel/MdmProductAssortments")
                    .AppendPathSegment(id)
                    .GetJsonAsync<AssortmentDetailViewModel>();
            }

            var update = add
                || dto.Properties.Name != assortment.Name
                || dto.Properties.ExternalId != assortment.UniqueId;

            if (!update)
            {
                return dto.Properties.Id;
            }

            dto.Properties.Name = assortment.Name;
            dto.Properties.ExternalId = assortment.UniqueId;

            var response = await this.GetClient()
                .AppendPathSegment("api/vme/v1/viewmodel/MdmProductAssortments")
                .PostJsonAsync(dto)
                .ReceiveJson<AssortmentDetailViewModel>();

            if (!response.IsValid)
            {
                throw new ViewModelValidationException();
            }

            return response.Properties.Id;
        }

        private IFlurlRequest GetClient()
        {
            return this.config.Endpoint
                .WithOAuthBearerToken(this.config.Token);
        }
    }
}
