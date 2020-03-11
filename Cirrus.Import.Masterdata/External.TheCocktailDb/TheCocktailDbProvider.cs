using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.TheCocktailDb
{
    class TheCocktailDbProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Cocktails";
        private readonly string RootCategoryId = "Cocktails";
        private readonly TheCocktailDbOptions options;
        private IReadOnlyList<Category> categories;

        public bool Enabled => this.options.Enabled;

        public string Key => "the-cocktail-db";

        public TheCocktailDbProvider(TheCocktailDbOptions options)
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
                .AppendPathSegment("list.php")
                .SetQueryParam("c", "list")
                .GetJsonAsync<CollectionDto<CategoryDto>>();

            categories.AddRange(result.Items
                .Select(x => new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = x.Name,
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

                var result = await this.GetClient()
                    .AppendPathSegment("filter.php")
                    .SetQueryParam("c", category.ExternalId)
                    .GetJsonAsync<CollectionDto<CocktailSummaryDto>>();

                yield return result.Items
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
                        Price = Price.FromId(x.Id, 15),
                        Picture = x.Picture,
                        ExternalCategoryId = category.ExternalId
                    })
                    .ToList();
            }
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://www.thecocktaildb.com/api/json/v1/1");
        }
    }
}
