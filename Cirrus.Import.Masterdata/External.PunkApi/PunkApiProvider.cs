using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.PunkApi
{
    class PunkApiProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Beer";
        private readonly string RootCategoryId = "Beer";
        private readonly PunkApiOptions options;

        public bool Enabled => this.options.Enabled;

        public string Key => "punk-api";

        public PunkApiProvider(PunkApiOptions options)
        {
            this.options = options;
        }

        public Task<List<Assortment>> GetAssortmentsAsync()
        {
            return Task.FromResult(new List<Assortment>
            {
                new Assortment
                {
                    ExternalKey = this.Key,
                    ExternalId = this.AssortmentId
                }
            });
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return Task.FromResult(new List<Category>
            {
                new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = this.RootCategoryId
                }
            });
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            List<BeerDto> response;
            bool hasMore;
            var pageSize = 80;
            var page = 1;

            do
            {
                response = await this.GetClient()
                    .SetQueryParam("page", page)
                    .SetQueryParam("per_page", pageSize)
                    .GetJsonAsync<List<BeerDto>>();

                hasMore = response.Count == pageSize;
                page++;

                yield return response
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id.ToString(),
                        Name = x.Name,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, 10),
                        Picture = x.Picture,
                        ExternalCategoryId = this.RootCategoryId
                    })
                    .ToList();
            }
            while (hasMore);
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://api.punkapi.com/v2/beers");
        }
    }
}
