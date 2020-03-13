using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.Fono
{
    class FonoProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Mobilephones";
        private readonly string RootCategoryId = "Mobilephones";
        private readonly FonoOptions options;
        private IReadOnlyList<Category> categories;

        public bool Enabled => this.options.Enabled;

        public string Key => "fono";

        public FonoProvider(FonoOptions options)
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
                .AppendPathSegment("getlatest")
                .GetJsonAsync<List<DeviceDto>>();

            categories.AddRange(result
                .Where(x => !string.IsNullOrWhiteSpace(x.Brand))
                .Select(x => x.Brand)
                .Distinct()
                .Select(x => new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = x,
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
                    .AppendPathSegment("getlatest")
                    .SetQueryParam("brand", category.ExternalId)
                    .GetJsonAsync<List<DeviceDto>>();

                yield return result
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.DeviceName,
                        Name = x.DeviceName,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.DeviceName),
                        Price = Price.FromId(x.DeviceName, 1000),
                        ExternalCategoryIds = new List<string> { category.ExternalId }
                    })
                    .ToList();
            }
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://fonoapi.freshpixl.com/v1")
                .SetQueryParam("token", this.options.ApiKey);
        }
    }
}
