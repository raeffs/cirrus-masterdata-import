using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace Cirrus.Import.Masterdata.Cirrus.Products
{
    class ProductApi
    {
        private readonly ApiOptions config;

        public ProductApi(ApiOptions config)
        {
            this.config = config;
        }

        public async Task<long> AddOrUpdateAsync(Product product)
        {
            var add = string.IsNullOrWhiteSpace(product.Id) || product.Id == "0";
            ProductDetailViewModel dto;
            JObject untypedDto = null;

            if (add)
            {
                dto = new ProductDetailViewModel
                {
                    Properties = new ProductDetailViewModelProperties
                    {
                        Id = "0"
                    },
                    Lists = new ProductDetailViewModelLists
                    {
                        Mappings = new List<Mapping>
                        {
                            new Mapping { Key = product.ExternalKey, Value = product.ExternalId }
                        }
                    }
                };
            }
            else
            {
                untypedDto = await this.GetClient()
                    .AppendPathSegment("api/vme/v1/viewmodel/MdmProducts")
                    .AppendPathSegment(product.Id)
                    .GetJsonAsync<JObject>();
                dto = untypedDto.ToObject<ProductDetailViewModel>();
            }

            var update = add
                || dto.Properties.Name != product.Name
                || dto.Properties.Number != product.UniqueId
                || dto.Properties.ExternalId != product.UniqueId
                || dto.Properties.Price != product.Price
                || dto.Properties.Picture != product.Picture
                || !dto.Properties.MinAges.ContainsReferences(product.MinAgesForYouthProtection)
                || !dto.Properties.Unit.ContainsReference(product.UnitId)
                || !dto.Properties.Tax.ContainsReference(product.TaxId)
                || !dto.Properties.ProductGroup.ContainsReference(product.GroupId)
                || !dto.Lists.ProductAssortments.ContainsReference(product.AssortmentId)
                || !dto.Lists.Barcodes.ContainsCode(product.Barcode)
                || !untypedDto.ContainsAllCategories(product.RootCategoryId, product.CategoryIds);

            if (!update)
            {
                return long.Parse(dto.Properties.Id);
            }

            dto.Properties.Name = product.Name;
            dto.Properties.Number = product.UniqueId;
            dto.Properties.ExternalId = product.UniqueId;
            dto.Properties.Price = product.Price;
            dto.Properties.Picture = product.Picture;
            dto.Properties.MinAges = Reference.ListFrom(product.MinAgesForYouthProtection);
            dto.Properties.Unit = Reference.ListFrom(product.UnitId);
            dto.Properties.Tax = Reference.ListFrom(product.TaxId);
            dto.Properties.ProductGroup = Reference.ListFrom(product.GroupId);
            dto.Lists.ProductAssortments = Reference.ListFrom(product.AssortmentId);
            dto.Lists.Barcodes = ProductBarcode.ListFrom(product.Barcode);

            untypedDto = JObject.FromObject(dto);
            untypedDto.SetCategories(product.RootCategoryId, product.CategoryIds);

            var response = await this.GetClient()
                .AppendPathSegment("api/vme/v1/viewmodel/MdmProducts")
                .PostJsonAsync(untypedDto)
                .ReceiveJson<ProductDetailViewModel>();

            if (!response.IsValid)
            {
                throw new ViewModelValidationException();
            }

            return long.Parse(response.Properties.Id);
        }

        public async Task<List<Mapping>> GetMappingsAsync(string key, List<string> values)
        {
            values = values.Distinct().ToList();

            var mappings = new List<Mapping>();
            PagedList<Mapping> response = null;

            do
            {
                response = await this.GetClient()
                    .AppendPathSegment("api/mdm/v1/products/mappings")
                    .SetQueryParam("keys", key)
                    .SetQueryParam("values", string.Join(',', values))
                    .SetQueryParam("pageSize", 100)
                    .SetQueryParam("currentPage", response?.NextPage ?? 1)
                    .GetJsonAsync<PagedList<Mapping>>();

                mappings.AddRange(response.Items);
            }
            while (response.HasMore);

            return mappings;
        }

        private IFlurlRequest GetClient()
        {
            return this.config.Endpoint
                .WithOAuthBearerToken(this.config.Token);
        }
    }
}
