using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.Cirrus.Assortments
{
    class AssortmentApi
    {
        private readonly ApiOptions config;

        public AssortmentApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task<long> AddOrUpdateAsync(Assortment assortment)
        {
            var add = string.IsNullOrWhiteSpace(assortment.Id) || assortment.Id == "0";
            AssortmentDetailViewModel dto;

            if (add)
            {
                dto = new AssortmentDetailViewModel
                {
                    Properties = new AssortmentDetailViewModelProperties
                    {
                        Id = "0"
                    }
                };
            }
            else
            {
                dto = await this.GetClient()
                    .AppendPathSegment("api/vme/v1/viewmodel/MdmProductAssortments")
                    .AppendPathSegment(assortment.Id)
                    .GetJsonAsync<AssortmentDetailViewModel>();
            }

            var update = add
                || dto.Properties.Name != assortment.Name
                || dto.Properties.ExternalId != assortment.UniqueId;

            if (!update)
            {
                return long.Parse(dto.Properties.Id);
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

            return long.Parse(response.Properties.Id);
        }

        public async Task<List<Mapping>> GetMappingsAsync(string key, List<string> values)
        {
            values = values.Distinct().ToList();

            var namesOfInterest = values.Select(x => $"{x} ({key})").ToList();

            var response = await this.GetClient()
                .AppendPathSegment("api/vme/v1/viewmodel/MdmProductAssortments")
                .SetQueryParam("pageSize", 100)
                .GetJsonAsync<ListViewModel<AssortmentListViewModel>>();

            return response.Data
                .Where(x => namesOfInterest.Contains(x.Name))
                .Select(x => new Mapping
                {
                    Id = x.Id,
                    Key = key,
                    Value = values.Single(y => x.Name == $"{y} ({key})")
                })
                .ToList();
        }

        private IFlurlRequest GetClient()
        {
            return this.config.Endpoint
                .WithOAuthBearerToken(this.config.Token);
        }
    }
}
