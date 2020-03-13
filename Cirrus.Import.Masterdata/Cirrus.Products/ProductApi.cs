using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Cirrus.Assortments;
using Cirrus.Import.Masterdata.Cirrus.Categories;
using Cirrus.Import.Masterdata.Cirrus.Groups;
using Cirrus.Import.Masterdata.Cirrus.Taxes;
using Cirrus.Import.Masterdata.Cirrus.Units;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace Cirrus.Import.Masterdata.Cirrus.Products
{
    class ProductApi : BaseApi<string>
    {
        private readonly ApiOptions config;
        private readonly UnitApi unitApi;
        private readonly TaxApi taxApi;
        private readonly GroupApi groupApi;
        private readonly AssortmentApi assortmentApi;
        private readonly CategoryApi categoryApi;

        public ProductApi(
            ApiOptions config,
            UnitApi unitApi,
            TaxApi taxApi,
            GroupApi groupApi,
            AssortmentApi assortmentApi,
            CategoryApi categoryApi)
        {
            this.config = config;
            this.unitApi = unitApi;
            this.taxApi = taxApi;
            this.groupApi = groupApi;
            this.assortmentApi = assortmentApi;
            this.categoryApi = categoryApi;
        }

        public async Task AddOrUpdateAsync(IEnumerable<Product> products)
        {
            var key = products.Select(x => x.ExternalKey).Distinct().Single();
            await this.GetMappingsAsync(key, products.Select(x => x.ExternalId));
            foreach (var product in products)
            {
                var id = await this.AddOrUpdateAsync(product);
                this.AddMapping(new Mapping<string> { Id = id, Key = key, Value = product.ExternalId });
            }
        }

        protected override async Task<IEnumerable<Mapping<string>>> LoadMappingsAsync(string key, IEnumerable<string> values)
        {
            var mappings = new List<Mapping<string>>();
            PagedList<Mapping<string>> response = null;

            do
            {
                response = await this.GetClient()
                    .AppendPathSegment("api/mdm/v1/products/mappings")
                    .SetQueryParam("keys", key)
                    .SetQueryParam("values", string.Join(',', values))
                    .SetQueryParam("pageSize", 100)
                    .SetQueryParam("currentPage", response?.NextPage ?? 1)
                    .GetJsonAsync<PagedList<Mapping<string>>>();

                mappings.AddRange(response.Items);
            }
            while (response.HasMore);

            return mappings;
        }

        private async Task<string> AddOrUpdateAsync(Product product)
        {
            var id = await this.GetMappingAsync(product.ExternalKey, product.ExternalId);
            var add = string.IsNullOrWhiteSpace(id);
            ProductDetailViewModel dto;
            JObject untypedDto = null;

            if (add)
            {
                dto = new ProductDetailViewModel
                {
                    Lists = new ProductDetailViewModelLists
                    {
                        Mappings = new List<Mapping<string>>
                        {
                            new Mapping<string> { Key = product.ExternalKey, Value = product.ExternalId }
                        }
                    }
                };
            }
            else
            {
                untypedDto = await this.GetClient()
                    .AppendPathSegment("api/vme/v1/viewmodel/MdmProducts")
                    .AppendPathSegment(id)
                    .GetJsonAsync<JObject>();
                dto = untypedDto.ToObject<ProductDetailViewModel>();
            }

            var unitId = await this.unitApi.GetMappingAsync(product.ExternalKey, product.ExternalUnit);
            var taxId = await this.taxApi.GetMappingAsync(product.ExternalKey, product.ExternalTax);
            var groupId = await this.groupApi.GetMappingAsync(product.ExternalKey, product.ExternalGroup);
            var assortmentId = await this.assortmentApi.GetMappingAsync(product.ExternalKey, product.ExternalAssortmentId);
            var categoryIds = (await this.categoryApi.GetMappingsAsync(product.ExternalKey, product.ExternalCategoryIds)).Select(x => x.Id);
            var rootCategoryId = await this.categoryApi.GetRootCategoryId(categoryIds.First());

            var update = add
                || dto.Properties.Name != product.Name
                || dto.Properties.Number != product.UniqueId
                || dto.Properties.ExternalId != product.UniqueId
                || dto.Properties.Price != product.Price
                || dto.Properties.Picture != product.Picture
                || !dto.Properties.MinAges.ContainsReferences(product.MinAgesForYouthProtection)
                || !dto.Properties.Unit.ContainsReference(unitId)
                || !dto.Properties.Tax.ContainsReference(taxId)
                || !dto.Properties.ProductGroup.ContainsReference(groupId)
                || !dto.Lists.ProductAssortments.ContainsReference(assortmentId)
                || !dto.Lists.Barcodes.ContainsCode(product.Barcode)
                || !untypedDto.ContainsAllCategories(rootCategoryId, categoryIds);

            if (!update)
            {
                return dto.Properties.Id;
            }

            dto.Properties.Name = product.Name;
            dto.Properties.Number = product.UniqueId;
            dto.Properties.ExternalId = product.UniqueId;
            dto.Properties.Price = product.Price;
            dto.Properties.Picture = product.Picture;
            dto.Properties.MinAges = Reference.ListFrom(product.MinAgesForYouthProtection);
            dto.Properties.Unit = Reference.ListFrom(unitId);
            dto.Properties.Tax = Reference.ListFrom(taxId);
            dto.Properties.ProductGroup = Reference.ListFrom(groupId);
            dto.Lists.ProductAssortments = Reference.ListFrom(assortmentId);
            dto.Lists.Barcodes = ProductBarcode.ListFrom(product.Barcode);

            untypedDto = JObject.FromObject(dto);
            untypedDto.SetCategories(rootCategoryId, categoryIds);

            var response = await this.GetClient()
                .AppendPathSegment("api/vme/v1/viewmodel/MdmProducts")
                .PostJsonAsync(untypedDto)
                .ReceiveJson<ProductDetailViewModel>();

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
