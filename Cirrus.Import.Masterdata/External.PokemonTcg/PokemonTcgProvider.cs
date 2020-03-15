using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.PokemonTcg
{
    class PokemonTcgProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Pokemon TCG Cards";
        private readonly string RootCategoryId = "Pokemon TCG Cards";
        private readonly PokemonTcgOptions options;
        private IReadOnlyList<Category> categories;

        public bool Enabled => this.options.Enabled;

        public string Key => "pokemon-tcg";

        public PokemonTcgProvider(PokemonTcgOptions options)
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
                .AppendPathSegment("sets")
                .GetJsonAsync<SetCollectionDto>();

            categories.AddRange(result.Sets
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
            CardCollectionDto response;
            bool hasMore;
            var pageSize = 200;
            var nextPage = 1;

            do
            {
                response = await this.GetClient()
                    .AppendPathSegment("cards")
                    .SetQueryParam("page", nextPage)
                    .SetQueryParam("pageSize", pageSize)
                    .GetJsonAsync<CardCollectionDto>();

                hasMore = response.Cards.Count == pageSize;
                nextPage++;

                yield return response.Cards
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id,
                        Name = x.Name,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, this.GetMaxPrice(x.Rarity)),
                        Picture = x.Picture,
                        ExternalCategoryIds = new List<string> { x.Set }
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
