using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.TheCocktailDb
{
    class TheCocktailDbProvider : ExternalProvider
    {
        private IReadOnlyList<Assortment> assortments;

        public string Key => "the-cocktail-db";

        public async Task<List<Assortment>> GetAssortmentsAsync()
        {
            if (this.assortments != null)
            {
                return this.assortments.ToList();
            }

            var result = await this.GetClient()
                .AppendPathSegment("list.php")
                .SetQueryParam("c", "list")
                .GetJsonAsync<CollectionDto<AssortmentDto>>();

            var assortments = result.Items
                .Select(x => new Assortment
                {
                    ExternalKey = this.Key,
                    ExternalId = x.Name
                })
                .ToList();

            this.assortments = assortments;
            return this.assortments.ToList();
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            foreach (var assortment in await this.GetAssortmentsAsync())
            {
                var result = await this.GetClient()
                    .AppendPathSegment("filter.php")
                    .SetQueryParam("c", assortment.ExternalId)
                    .GetJsonAsync<CollectionDto<ProductSummaryDto>>();

                yield return result.Items
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id.ToString(),
                        Name = x.Name,
                        ExternalAssortmentId = assortment.ExternalId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, 15),
                        Picture = x.Picture
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
