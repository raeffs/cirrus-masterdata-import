using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.Cirrus.Categories
{
    class CategoryApi
    {
        private readonly ApiOptions config;
        private readonly HashSet<Mapping> knownMappings = new HashSet<Mapping>();
        private readonly Dictionary<string, string> childToParentMap = new Dictionary<string, string>();

        public CategoryApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task<long> AddOrUpdateAsync(Category category)
        {
            var add = string.IsNullOrWhiteSpace(category.Id) || category.Id == "0";
            CategoryDetailViewModel dto;

            if (add)
            {
                dto = new CategoryDetailViewModel();
            }
            else
            {
                dto = await this.GetClient()
                    .AppendPathSegment("api/vme/v1/viewmodel/MdmProductCategories")
                    .AppendPathSegment(category.Id)
                    .GetJsonAsync<CategoryDetailViewModel>();
            }

            var update = add
                || dto.Properties.Name != category.Name
                || dto.Properties.ExternalId != category.UniqueId
                || (!category.IsChild && dto.Properties.MaxOneNodePerProductAssignable != !category.AllowsMultipleAssignments)
                || (category.IsChild && !dto.Properties.Parent.ContainsReference(this.knownMappings.Find(category, x => x.ExternalParentId).Single()));

            if (!update)
            {
                this.knownMappings.Add(new Mapping
                {
                    Id = dto.Properties.Id,
                    Key = category.ExternalKey,
                    Value = category.ExternalId
                });
                if (category.IsChild)
                {
                    this.childToParentMap[dto.Properties.Id] = dto.Properties.Parent.Select(x => x.Id).Single();
                }
                return long.Parse(dto.Properties.Id);
            }

            dto.Properties.Name = category.Name;
            dto.Properties.ExternalId = category.UniqueId;
            dto.Properties.MaxOneNodePerProductAssignable = !category.AllowsMultipleAssignments;

            var response = await this.GetClient()
                .AppendPathSegment("api/vme/v1/viewmodel/MdmProductCategories")
                .SetQueryParam("parentId", category.IsChild ? this.knownMappings.Find(category, x => x.ExternalParentId).Single() : null)
                .PostJsonAsync(dto)
                .ReceiveJson<CategoryDetailViewModel>();

            if (!response.IsValid)
            {
                throw new ViewModelValidationException();
            }

            this.knownMappings.Add(new Mapping
            {
                Id = response.Properties.Id,
                Key = category.ExternalKey,
                Value = category.ExternalId
            });
            if (category.IsChild)
            {
                this.childToParentMap[response.Properties.Id] = response.Properties.Parent.Select(x => x.Id).Single();
            }
            return long.Parse(response.Properties.Id);
        }

        public async Task<List<Mapping>> GetMappingsAsync(string key, List<string> values)
        {
            values = values.Distinct().ToList();

            if (values.All(x => this.knownMappings.Any(y => y.Value == x)))
            {
                return this.knownMappings.Where(x => values.Any(y => x.Value == y)).ToList();
            }

            var mappings = new List<Mapping>();
            var newMappings = new List<Mapping>();
            var namesOfInterest = values.Select(x => $"{x} ({key})").ToList();
            TreeViewModel<CategoryTreeViewModel> response = null;

            do
            {
                var expandedIds = response?.Data?.Select(x => x.Id).Where(x => mappings.Any(y => y.Id == x)) ?? new List<string>();

                response = await this.GetClient()
                    .AppendPathSegment("api/vme/v1/viewmodel/tree/MdmProductCategories")
                    .SetQueryParam("expandedIds", string.Join(',', expandedIds))
                    .GetJsonAsync<TreeViewModel<CategoryTreeViewModel>>();

                newMappings = response.Data
                    .Where(x => namesOfInterest.Contains(x.Name) && !mappings.Any(y => y.Id == x.Id))
                    .Select(x => new Mapping
                    {
                        Id = x.Id,
                        Key = key,
                        Value = values.Single(y => x.Name == $"{y} ({key})")
                    })
                    .ToList();

                mappings.AddRange(newMappings);
            }
            while (newMappings.Any());

            this.knownMappings.AddRange(newMappings);

            return mappings;
        }

        public async Task<string> GetRootCategoryId(string childCategoryId)
        {
            if (this.childToParentMap.ContainsKey(childCategoryId))
            {
                return await this.GetRootCategoryId(this.childToParentMap[childCategoryId]);
            }
            else
            {
                return childCategoryId;
            }
        }

        private IFlurlRequest GetClient()
        {
            return this.config.Endpoint
                .WithOAuthBearerToken(this.config.Token);
        }
    }
}
