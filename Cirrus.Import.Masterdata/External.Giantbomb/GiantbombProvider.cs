using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.Giantbomb
{
    class GiantbombProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Games";
        private readonly string RootCategoryId = "Games";
        private readonly string HardwareCategoryId = "Hardware";
        private readonly GiantbombOptions options;
        private IReadOnlyList<Category> categories;
        private IReadOnlyList<Product> products;

        public bool Enabled => this.options.Enabled;

        public string Key => "giantbomb";

        public GiantbombProvider(GiantbombOptions options)
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

        public async Task<List<Category>> GetCategoriesAsync()
        {
            if (this.categories != null)
            {
                return this.categories.ToList();
            }

            var categories = new List<Category>
            {
                new Category { ExternalKey = this.Key, ExternalId = this.RootCategoryId },
                new Category { ExternalKey = this.Key, ExternalId = this.HardwareCategoryId, ExternalParentId = this.RootCategoryId }
            };
            var products = new List<Product>();

            CollectionDto<PlatformDto> result = null;

            do
            {
                result = await this.GetClient()
                    .AppendPathSegment("platforms")
                    .SetQueryParam("offset", result?.NextOffset ?? 0)
                    .GetJsonAsync<CollectionDto<PlatformDto>>();

                categories.AddRange(result.Results
                    .Select(x => new Category
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Name,
                        ExternalParentId = this.RootCategoryId
                    })
                    .ToList());

                products.AddRange(result.Results
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Guid,
                        Name = x.Name,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Guid),
                        Price = x.Price != null ? Price.From(x.Price) : Price.FromId(x.Guid, 500),
                        Picture = x.Picture,
                        ExternalCategoryId = this.HardwareCategoryId
                    })
                    .ToList());
            }
            while (result.HasMore);

            this.categories = categories;
            this.products = products;
            return this.categories.ToList();
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            yield return this.products.ToList();

            CollectionDto<GameDto> result = null;

            do
            {
                result = await this.GetClient()
                    .AppendPathSegment("games")
                    .SetQueryParam("offset", result?.NextOffset ?? 0)
                    .GetJsonAsync<CollectionDto<GameDto>>();

                yield return result.Results
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Guid,
                        Name = x.Name,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Guid),
                        Price = Price.FromId(x.Guid, 100),
                        Picture = x.Picture,
                        ExternalCategoryId = x.Platforms.Select(y => y.Name).FirstOrDefault()
                    })
                    .ToList();
            }
            while (result.HasMore);
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://www.giantbomb.com/api")
                .SetQueryParam("api_key", this.options.ApiKey)
                .SetQueryParam("format", "json");
        }
    }
}
