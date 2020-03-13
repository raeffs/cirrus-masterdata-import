using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.Cirrus.Categories
{
    class CategoryApi : BaseApi<string>
    {
        private readonly ApiOptions config;
        private readonly Dictionary<string, string> childToParentMap = new Dictionary<string, string>();

        public CategoryApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task AddOrUpdateAsync(IEnumerable<Category> categories)
        {
            var key = categories.Select(x => x.ExternalKey).Distinct().Single();
            await this.GetMappingsAsync(key, categories.Select(x => x.ExternalId));
            foreach (var category in categories)
            {
                var id = await this.AddOrUpdateAsync(category);
                this.AddMapping(new Mapping<string> { Id = id, Key = key, Value = category.ExternalId });
                this.childToParentMap[id] = await this.GetMappingAsync(key, category.ExternalParentId);
            }
        }

        public async Task<string> GetRootCategoryId(string childCategoryId)
        {
            if (this.childToParentMap.ContainsKey(childCategoryId))
            {
                var parent = this.childToParentMap[childCategoryId];
                return string.IsNullOrWhiteSpace(parent) ? childCategoryId : await this.GetRootCategoryId(parent);
            }
            else
            {
                return childCategoryId;
            }
        }

        protected override async Task<IEnumerable<Mapping<string>>> LoadMappingsAsync(string key, IEnumerable<string> values)
        {
            var mappings = new List<Mapping<string>>();
            var newMappings = new List<Mapping<string>>();
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
                    .Select(x => new Mapping<string>
                    {
                        Id = x.Id,
                        Key = key,
                        Value = values.Single(y => x.Name == $"{y} ({key})")
                    })
                    .ToList();

                mappings.AddRange(newMappings);
            }
            while (newMappings.Any());

            return mappings;
        }

        private async Task<string> AddOrUpdateAsync(Category category)
        {
            var id = await this.GetMappingAsync(category.ExternalKey, category.ExternalId);
            var add = string.IsNullOrWhiteSpace(id);
            CategoryDetailViewModel dto;

            if (add)
            {
                dto = new CategoryDetailViewModel();
            }
            else
            {
                dto = await this.GetClient()
                    .AppendPathSegment("api/vme/v1/viewmodel/MdmProductCategories")
                    .AppendPathSegment(id)
                    .GetJsonAsync<CategoryDetailViewModel>();
            }

            var update = add
                || dto.Properties.Name != category.Name
                || dto.Properties.ExternalId != category.UniqueId
                || (!category.IsChild && dto.Properties.MaxOneNodePerProductAssignable != !category.AllowsMultipleAssignments)
                || (category.IsChild && !dto.Properties.Parent.ContainsReference(await this.GetMappingAsync(category.ExternalKey, category.ExternalParentId)));

            if (!update)
            {
                return dto.Properties.Id;
            }

            dto.Properties.Name = category.Name;
            dto.Properties.ExternalId = category.UniqueId;
            dto.Properties.MaxOneNodePerProductAssignable = !category.AllowsMultipleAssignments;

            var response = await this.GetClient()
                .AppendPathSegment("api/vme/v1/viewmodel/MdmProductCategories")
                .SetQueryParam("parentId", category.IsChild ? await this.GetMappingAsync(category.ExternalKey, category.ExternalParentId) : null)
                .PostJsonAsync(dto)
                .ReceiveJson<CategoryDetailViewModel>();

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
