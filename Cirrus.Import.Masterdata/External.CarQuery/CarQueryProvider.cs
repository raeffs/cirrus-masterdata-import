using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.CarQuery
{
    class CarQueryProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Cars";
        private readonly string RootCategoryId = "Cars";
        private readonly CarQueryOptions options;
        private IReadOnlyList<Category> categories;

        public bool Enabled => this.options.Enabled;

        public string Key => "car-query";

        public CarQueryProvider(CarQueryOptions options)
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
                .SetQueryParam("cmd", "getMakes")
                .GetJsonAsync<MakeCollectionDto>();

            categories.AddRange(result.Makes
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
                                        .SetQueryParam("cmd", "getTrims")
                                        .SetQueryParam("make", category.ExternalId.Replace(' ', '-'))
                                        .GetJsonAsync<CarCollectionDto>();

                yield return result.Trims
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id,
                        Name = x.FullName,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, 2000000),
                        ExternalCategoryIds = new List<string> { category.ExternalId }
                    })
                    .ToList();
            }
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://www.carqueryapi.com/api/0.3/");
        }
    }
}
