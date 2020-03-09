using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.PokemonTcg
{
    class PokemonTcgProvider : ExternalProvider
    {
        public string Key => "pokemon-tcg";

        public async Task<List<Assortment>> GetAssortmentsAsync()
        {
            return new List<Assortment>
            {
                new Assortment
                {
                    ExternalKey = this.Key,
                    ExternalId = "Pokemon TCG Cards"
                }
            };
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            CollectionDto<ProductDto> response;
            bool hasMore;
            var pageSize = 200;
            var nextPage = 1;

            do
            {
                response = await this.GetClient()
                    .AppendPathSegment("cards")
                    .SetQueryParam("page", nextPage)
                    .SetQueryParam("pageSize", pageSize)
                    .GetJsonAsync<CollectionDto<ProductDto>>();

                hasMore = response.Items.Count == pageSize;
                nextPage++;

                yield return response.Items
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id,
                        Name = x.Name,
                        ExternalAssortmentId = "Pokemon TCG Cards",
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, this.GetMaxPrice(x.Rarity)),
                        Picture = x.Picture
                    })
                    .ToList();
            }
            while (hasMore);
        }

        private int GetMaxPrice(string rarity) => rarity switch
        {
            null => 1,
            "" => 1,
            "Common" => 1,
            "Uncommon" => 5,
            "Rare" => 20,
            _ => 100
        };

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://api.pokemontcg.io/v1");
        }
    }
}
