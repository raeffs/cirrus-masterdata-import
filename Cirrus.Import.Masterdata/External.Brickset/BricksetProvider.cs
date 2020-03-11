using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.Brickset
{
    class BricksetProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Lego";
        private readonly string RootCategoryId = "Lego";
        private readonly BricksetOptions options;
        private IReadOnlyList<Category> categories;

        public bool Enabled => this.options.Enabled;

        public string Key => "brickset";

        public BricksetProvider(BricksetOptions options)
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
                new Category { ExternalKey = this.Key, ExternalId = this.RootCategoryId }
            };

            var result = await this.GetClient()
                .AppendPathSegment("getThemes")
                .GetJsonFromXmlAsync<ThemeCollectionDto>();

            categories.AddRange(result.ArrayOfThemes.Themes
                .Select(x => new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = x.Theme,
                    ExternalParentId = this.RootCategoryId
                })
                .ToList());

            this.categories = categories;
            return this.categories.ToList();
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            foreach (var category in await this.GetCategoriesAsync())
            {
                if (!category.IsChild)
                {
                    continue;
                }

                SetCollectionDto result;
                bool hasMore;
                var pageSize = 100;
                var page = 1;

                do
                {
                    result = await this.GetClient()
                        .AppendPathSegment("getSets")
                        .SetQueryParam("theme", category.ExternalId)
                        .SetQueryParam("pageSize", 100)
                        .SetQueryParam("pageNumber", page)
                        .SetQueryParam("userHash", string.Empty)
                        .SetQueryParam("query", string.Empty)
                        .SetQueryParam("subtheme", string.Empty)
                        .SetQueryParam("setNumber", string.Empty)
                        .SetQueryParam("year", string.Empty)
                        .SetQueryParam("owned", string.Empty)
                        .SetQueryParam("wanted", string.Empty)
                        .SetQueryParam("orderBy", string.Empty)
                        .SetQueryParam("userName", string.Empty)
                        .GetJsonFromXmlAsync<SetCollectionDto>();

                    hasMore = result.ArrayOfSets.Sets.Count == pageSize;
                    page++;

                    yield return result.ArrayOfSets.Sets
                        .Select(x => new Product
                        {
                            ExternalKey = this.Key,
                            ExternalId = x.SetId,
                            Name = x.Name,
                            ExternalAssortmentId = this.AssortmentId,
                            ExternalUnit = Unit.Piece,
                            ExternalTax = Tax.Default,
                            ExternalGroup = Group.Default,
                            Barcode = Barcode.FromId(this.Key, x.SetId),
                            Price = Price.From(x.Price, 200),
                            Picture = x.Picture,
                            ExternalCategoryId = category.ExternalId
                        })
                        .ToList();
                }
                while (hasMore);
            }
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://brickset.com/api/v2.asmx")
                .SetQueryParam("apiKey", this.options.ApiKey);
        }
    }
}
