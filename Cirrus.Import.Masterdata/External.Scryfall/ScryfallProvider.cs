using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.Scryfall
{
    class ScryfallProvider : ExternalProvider
    {
        public string Key => "scryfall";

        public async Task<List<Assortment>> GetAssortmentsAsync()
        {
            return new List<Assortment>
            {
                new Assortment
                {
                    ExternalKey = this.Key,
                    ExternalId = "Magic The Gathering Cards"
                }
            };
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            CollectionDto<ProductDto> result = null;

            do
            {
                result = await this.GetClient()
                    .AppendPathSegment("cards")
                    .SetQueryParam("page", result?.NextPage ?? 1)
                    .GetJsonAsync<CollectionDto<ProductDto>>();

                yield return result.Data
                    // the API does not allow us to filter, so we do it afterwards
                    // we are only intrested in the english cards, as the other languages are just duplicates
                    .Where(x => x.Language == "en")
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id,
                        Name = x.Name,
                        ExternalAssortmentId = "Magic The Gathering Cards",
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, this.GetMaxPrice(x.Rarity)),
                        Picture = x.Picture
                    })
                    .ToList();
            }
            while (result.HasMore);
        }

        private int GetMaxPrice(Rarity rarity) => rarity switch
        {
            Rarity.Common => 1,
            Rarity.Uncommon => 5,
            Rarity.Rare => 20,
            _ => 100
        };

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://api.scryfall.com/");
        }
    }
}
